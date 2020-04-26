using System;
using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TraitsExpanded.TraitSets;

namespace TraitsExpanded
{
    public class TraitSystemBehavior : CampaignBehaviorBase
    {
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
