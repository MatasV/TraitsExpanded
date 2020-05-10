using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TraitsExpanded.TraitSets;

namespace TraitsExpanded
{
    public class TraitSystemBehavior : CampaignBehaviorBase
    {
        private const string NewTraitFileFormat = "{0}_traits_new.json";

        private const string CurrentTraitFileFormat = "{0}_traits.json";

        private const string BackupTraitFileFormat = "{0}_traits_backup.json";

        private const int Version = 1;

        public delegate TraitSet TraitSetDeserializer(CharacterObject character, int version, bool isActive, int currentTraitIndex, string customDataJson);

        public static readonly Dictionary<Guid, TraitSetDeserializer> RegisteredTraitSetDeserializers = new Dictionary<Guid, TraitSetDeserializer>();

        public static readonly List<TraitSet> AllTraitSets = new List<TraitSet>();

        public static Dictionary<CharacterObject, List<TraitSet>> CharacterTraitInfo =
            new Dictionary<CharacterObject, List<TraitSet>>();

        public delegate void CampaignTickCall(CampaignStatusInfo statusInfo);

        public static CampaignTickCall OnCampaignTick;

        public delegate void MissionTickCall(MissionStatusInfo statusInfo);

        public static MissionTickCall OnMissionTick;

        public delegate void GameTickCall(GameStatusInfo statusInfo);

        public static GameTickCall OnGameTick;

        private static float MissionTickTime { get; } = 1f;

        private static float CampaignTickTime { get; } = 1f;

        private static float GameTickTime { get; } = 1f;

        private Timer MissionTickTimer { get; } = new Timer(Time.ApplicationTime, MissionTickTime, false);

        private Timer CampaignTickTimer { get; } = new Timer(Time.ApplicationTime, CampaignTickTime, false);

        private Timer GameTickTimer { get; } = new Timer(Time.ApplicationTime, GameTickTime, false);

        public TraitSystemBehavior()
        {
            Init();
        }

        private void Init()
        {
            OnCampaignTick += ValidateHeroList;
            CampaignEvents.OnGameLoadedEvent.AddNonSerializedListener(this, starter =>
              {
                  GivePlayerTraitSet(new BigCharacterTraitSet(null));
               });
        }

        private void OnGameStart(CampaignGameStarter starter)
        {
            
        }

        private void PopulateTraitSets()
        {
            AllTraitSets.Add(new BigCharacterTraitSet(null));
        }

        public void Tick()
        {
            if (MissionTickTimer.Check(Time.ApplicationTime))
            {
                OnMissionTick?.Invoke(new MissionStatusInfo());
                MissionTickTimer.Reset(Time.ApplicationTime, MissionTickTime);
            }

            if (CampaignTickTimer.Check(Time.ApplicationTime))
            {
                OnCampaignTick?.Invoke(new CampaignStatusInfo());
                CampaignTickTimer.Reset(Time.ApplicationTime, CampaignTickTime);
            }

            if (GameTickTimer.Check(Time.ApplicationTime))
            {
                OnGameTick?.Invoke(new GameStatusInfo());
                GameTickTimer.Reset(Time.ApplicationTime, MissionTickTime);
            }
        }

        /// <summary>
        /// Save all trait data (from CharacterTraitInfo) to a file.
        /// </summary>
        /// <remarks>
        /// Uses a three-step rename procedure to allow recovery in case the save process is interrupted.
        /// </remarks>
        /// <param name="statusInfo">Information about the current campaign.</param>
        private void SaveTraitData(CampaignStatusInfo statusInfo)
        {
            string campaignSaveFilePath = ""; // todo: fetch this
            string newTraitFilePath = string.Format(NewTraitFileFormat, System.IO.Path.GetFileNameWithoutExtension(campaignSaveFilePath));
            string currentTraitFilePath = string.Format(CurrentTraitFileFormat, System.IO.Path.GetFileNameWithoutExtension(campaignSaveFilePath));
            string backupTraitFilePath = string.Format(BackupTraitFileFormat, System.IO.Path.GetFileNameWithoutExtension(campaignSaveFilePath));

            if (!File.Exists(currentTraitFilePath))
            {
                Util.LogMessage("Current trait save file '{0}' is missing during SaveTraitData. May have been deleted or renamed outside of the game.", currentTraitFilePath);

                if (File.Exists(backupTraitFilePath))
                {
                    File.Move(backupTraitFilePath, currentTraitFilePath);
                }
            }

            if (File.Exists(newTraitFilePath))
            {
                Util.LogMessage("New trait save file '{0}' already exists. Files may have been modified outside of the game.", newTraitFilePath);

                File.Delete(newTraitFilePath);
            }

            if (File.Exists(backupTraitFilePath))
            {
                Util.LogMessage("Backup trait save file '{0}' already exists. Files may have been modified outside of the game.", backupTraitFilePath);

                File.Delete(backupTraitFilePath);
            }

            using (var file = new FileStream(newTraitFilePath, FileMode.CreateNew, FileAccess.Write))
            {
                using (var writer = new Utf8JsonWriter(file, new JsonWriterOptions { Indented = true }))
                {
                    writer.WriteStartObject();
                    writer.WriteNumber("version", Version); // todo: use constants for all property names

                    writer.WriteStartArray("characters");

                    foreach (var character in statusInfo.CurrentCampaign.Characters)
                    {
                        List<TraitSet> characterTraitSets;

                        bool res = CharacterTraitInfo.TryGetValue(character, out characterTraitSets);
                        if (res)
                        {
                            writer.WriteStartObject();
                            writer.WriteString("name", character.Name.ToString());

                            writer.WriteStartArray("traitsets");

                            foreach (var traitSet in characterTraitSets)
                            {
                                writer.WriteStartObject();

                                writer.WriteString("id", traitSet.Id.ToString());
                                writer.WriteNumber("version", traitSet.Version);
                                writer.WriteBoolean("isactive", traitSet.IsActive);
                                writer.WriteNumber("currentindex", traitSet.CurrentTraitIndex);

                                writer.WriteStartObject("customdata");
                                traitSet.SerializeCustomState(writer);
                                writer.WriteEndObject();

                                writer.WriteEndObject();
                            }
                        }
                    }

                    writer.WriteEndArray();

                    writer.WriteEndObject();
                }
            }

            File.Move(currentTraitFilePath, backupTraitFilePath);
            File.Move(newTraitFilePath, currentTraitFilePath);
            File.Delete(backupTraitFilePath);
        }

        /// <summary>
        /// Read all saved trait data, and populate CharacterTraitInfo with the CharacterObject to TraitSet mappings. Uses RegisteredTraitSetDeserializers to instantiate the traitsets, so must be called after all registration is complete.
        /// </summary>
        private void LoadTraitData()
        {
            string campaignSaveFilePath = ""; // todo: fetch this
            string newTraitFilePath = string.Format(NewTraitFileFormat, System.IO.Path.GetFileNameWithoutExtension(campaignSaveFilePath));
            string currentTraitFilePath = string.Format(CurrentTraitFileFormat, System.IO.Path.GetFileNameWithoutExtension(campaignSaveFilePath));
            string backupTraitFilePath = string.Format(BackupTraitFileFormat, System.IO.Path.GetFileNameWithoutExtension(campaignSaveFilePath));

            if (File.Exists(newTraitFilePath))
            {
                File.Delete(newTraitFilePath);
                // todo: add footer to new file, to allow it to be loaded if it was fully written but the rename procedure was interrupted. make sure it isn't loaded if it's an older file.
            }

            if (!File.Exists(currentTraitFilePath))
            {
                Util.LogMessage("Current trait save file '{0}' is missing during LoadTraitData. May have been deleted or renamed outside of the game, saving may have been interrupted, or this is the first save for this campaign.", currentTraitFilePath);

                if (File.Exists(backupTraitFilePath))
                {
                    Util.LogMessage("Restoring backup trait save file '{0}'", backupTraitFilePath);
                    File.Move(backupTraitFilePath, currentTraitFilePath);
                }
                else
                {
                    Util.LogMessage("No current or backup trait save file, skipping loading traits.");
                    return; // no traits to load
                }
            }

            byte[] saveFileContents = File.ReadAllBytes(currentTraitFilePath);
            using (var document = JsonDocument.Parse(saveFileContents, new JsonDocumentOptions { AllowTrailingCommas = true, CommentHandling = JsonCommentHandling.Skip }))
            {
                var versionElement = document.RootElement.GetProperty("version");
                Util.LogMessage("Loading trait data, written by TraitsExtended version {0}", versionElement.GetInt32());

                var charactersElement = document.RootElement.GetProperty("characters");
                foreach (var characterElement in charactersElement.EnumerateArray())
                {
                    // todo: look up character based on name
                    CharacterObject character = new CharacterObject();

                    if (!CharacterTraitInfo.ContainsKey(character))
                    {
                        CharacterTraitInfo[character] = new List<TraitSet>();
                    }

                    var traitSets = CharacterTraitInfo[character];

                    foreach (var traitSetElement in characterElement.GetProperty("traitsets").EnumerateArray())
                    {
                        Guid id = traitSetElement.GetProperty("id").GetGuid();

                        if (!RegisteredTraitSetDeserializers.ContainsKey(id))
                        {
                            Util.LogMessage("Skipping loading unregistered traitset with id {0} and name {1}. Was a mod previously providing the trait uninstalled?", id, traitSetElement.GetProperty("name").GetString());
                        }
                        else
                        {
                            int version = traitSetElement.GetProperty("version").GetInt32();
                            bool isActive = traitSetElement.GetProperty("isactive").GetBoolean();
                            int currentIndex = traitSetElement.GetProperty("currentindex").GetInt32();
                            string customData = traitSetElement.GetProperty("customdata").GetString();

                            traitSets.Add(RegisteredTraitSetDeserializers[id](character, version, isActive, currentIndex, customData));
                        }
                    }
                }
            }
        }

        private void ValidateHeroList(CampaignStatusInfo statusInfo)
        {
            if (statusInfo.CurrentCampaign?.Characters == null ||
                statusInfo.CurrentCampaign.Characters.Count < 1) return;

            var characterObjects = new List<CharacterObject>();


            foreach (var character in statusInfo.CurrentCampaign.Characters)
            {
                if (CharacterTraitInfo.ContainsKey(character))
                {
                    characterObjects.Add(character);
                }
                else
                {
                    //LoadCharacterTraitData(character);
                    //adding this hero to the List manually for now, otherwise, the addition to the list should be moved to loadCharacterTraitData and GenerateCharacterTraits
                    if (!character.IsHero) continue;
                    
                    characterObjects.Add(character);
                    
                    if (character.IsPlayerCharacter)
                    {
                        CharacterTraitInfo.Add(character, new List<TraitSet>());
                        GivePlayerTraitSet(new BigCharacterTraitSet(null));
                    }
                    else
                    {
                        CharacterTraitInfo.Add(character, new List<TraitSet>());
                    }
                }
            }

            foreach (var oldCharacterObject in CharacterTraitInfo.Keys
                .Where(oldCharacterObject => characterObjects
                    .All(freshCharacterObject => oldCharacterObject != freshCharacterObject)))
            {
                //should save the character traits at this point since we will be removing them from the active Character List
                CharacterTraitInfo.Remove(oldCharacterObject);
            }

            foreach (var character in CharacterTraitInfo)
            {
                //InformationManager.DisplayMessage(new InformationMessage(character.Key?.ToString()));
            }
        }

        private bool AddTraitSet(CharacterObject character, TraitSet traitSet)
        {
            return true;
        }

        private bool AddTrait(CharacterObject character, ITrait traitSet)
        {
            return true;
        }

        private bool LoadCharacterTraitData(CharacterObject character)
        {
            //if the data exists, load it and return true, else call GenerateCharacterTraits()

            return true;
        }

        private bool SaveCharacterTraitData(CharacterObject character)
        {
            return true;
        }

        private void GenerateCharacterTraits(CharacterObject character)
        {
            //generate traits and add them to the CharacterTraitInfo
        }

        public List<ITrait> GetAllPositiveTraits()
        {
            return (from traitSet in AllTraitSets
                from
                    trait in traitSet.Traits
                where trait.Cost >= 0
                select trait).ToList();
        }

        public List<ITrait> GetAllNegativeTraits()
        {
            return (from traitSet in AllTraitSets
                from
                    trait in traitSet.Traits
                where trait.Cost < 0
                select trait).ToList();
        }

        public void RegisterTraitSet(TraitSet set, List<CharacterObject> charactersToGiveTraitTo = null)
        {
            if (!AllTraitSets.Contains(set))
            {
                AllTraitSets.Add(set);
                if (charactersToGiveTraitTo == null) return;

                foreach (var character in charactersToGiveTraitTo)
                {
                    if (CharacterTraitInfo.TryGetValue(character, out var currentSet))
                    {
                        if (!currentSet.Contains(set))
                        {
                            currentSet.Add(set);
                        }
                    }
                    else
                    {
                        //generate a new Character with traits or return that the hero does not exist
                    }
                }
            }
        }

        public void GivePlayerTraitSet(TraitSet traitSet)
        {
            try
            {
                if (!CharacterTraitInfo.TryGetValue(CharacterObject.PlayerCharacter, out var playerTraits))
                {
                    Util.LogMessage("Failed to give Player a trait, because I failed to find the list of Player's traits");
                    return;
                }

                if (!playerTraits.Contains(traitSet))
                {
                    Util.LogMessage($"The Player does not contain traitSet: {traitSet.Id}, adding to his traitSet list");
                    traitSet.Init(CharacterObject.PlayerCharacter);
                    playerTraits.Add(traitSet);
                }
                Util.LogMessage($"The Player already has the traitSet: {traitSet.Id}, will not be adding" );
            }
            catch (Exception e)
            {
                Util.LogMessage("Failed to give Player a trait " + e);
            }
        }

        public override void RegisterEvents()
        {
            Util.LogMessage("TraitSystem Behavior initialized");
            PopulateTraitSets();
        }

        public override void SyncData(IDataStore dataStore)
        {
        }
    }
}
