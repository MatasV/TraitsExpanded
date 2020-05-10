using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.CampaignBehaviors;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.MountAndBlade;
using TraitsExpanded.TraitSets;

namespace TraitsExpanded
{
    public class TraitSystemBehavior : CampaignBehaviorBase
    {
        private static readonly List<TraitSet> AllTraitSets = new List<TraitSet>();

        private static readonly Dictionary<CharacterObject, List<TraitSet>> CharacterTraitInfo =
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
            OnCampaignTick += ValidateAndPopulateHeroList;
        }

        private static void PopulateTraitSets()
        {
            AllTraitSets.Add(new BigCharacterTraitSet(null));
        }

        public void Tick()
        {
            if (MissionTickTimer.Check(Time.ApplicationTime) && Mission.Current != null)
            {
                OnMissionTick?.Invoke(new MissionStatusInfo());
                MissionTickTimer.Reset(Time.ApplicationTime, MissionTickTime);
            }

            if (CampaignTickTimer.Check(Time.ApplicationTime) && Campaign.Current != null)
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


        private void ValidateAndPopulateHeroList(CampaignStatusInfo statusInfo) //gets called every Campaign Tick (1 sec for now)
        {
            if (statusInfo.CurrentCampaign?.Characters == null ||
                statusInfo.CurrentCampaign.Characters.Count < 1) return;

            var characterObjects = new List<CharacterObject>(); //creating a List of characters to keep track of what characters are present this tick


            foreach (var character in statusInfo.CurrentCampaign.Characters) //going through every character currently present in the game
            {
                if (CharacterTraitInfo.ContainsKey(character)) //if this character is present in the current character list
                {
                    characterObjects.Add(character); //add the character to the most recent list (local)
                }
                else //if this character is not present in the current character list
                {
                    //LoadCharacterTraitData(character);
                    //adding this hero to the List manually for now, otherwise, the addition to the list should be moved to loadCharacterTraitData and GenerateCharacterTraits
                    if (!character.IsHero) continue; //if the character is not a hero, don't need to do anything.
                    
                    characterObjects.Add(character); //add the character to the most recent list (local)
                    
                    if (character.IsPlayerCharacter) //if the character is a player, add him to the List (global) and give him desired traits 
                    {
                        CharacterTraitInfo.Add(character, new List<TraitSet>());
                        GivePlayerTraitSet(new BigCharacterTraitSet(null));
                    }
                    else //add the character to the list (global)
                    {
                        CharacterTraitInfo.Add(character, new List<TraitSet>());
                    }
                }
            }

            foreach (var oldCharacterObject in CharacterTraitInfo.Keys 
                //comparing the local list of the most recent characters to the global list. 
                //If the global list has something that the local list does not, that means that the character is no longer available and needs to be saved and removed from the current List (global)
                .Where(oldCharacterObject => characterObjects
                    .All(freshCharacterObject => oldCharacterObject != freshCharacterObject)))
            {
                //should save the character traits at this point since we will be removing them from the active Character List
                CharacterTraitInfo.Remove(oldCharacterObject);
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
            var traits = new List<ITrait>();
            foreach (var traitSet in AllTraitSets)
            {
                traits.AddRange(traitSet.GetPositiveTraits());
            }
            return traits;
        }

        public List<ITrait> GetAllNegativeTraits()
        {
            var traits = new List<ITrait>();
            foreach (var traitSet in AllTraitSets)
            {
                traits.AddRange(traitSet.GetNegativeTraits());
            }
            return traits;
        }

        //registering a trait set
        public void RegisterTraitSet(TraitSet set, List<CharacterObject> charactersToGiveTraitTo = null) 
        {
            if (!AllTraitSets.Contains(set)) // if the trait set is not already registered
            {
                AllTraitSets.Add(set); //add it 
                if (charactersToGiveTraitTo == null) return; // if no characters need this trait, don't do anything

                foreach (var character in charactersToGiveTraitTo) //if you find a character, give him the trait set, 
                {
                    if (CharacterTraitInfo.TryGetValue(character, out var currentSet))
                    {
                        if (!currentSet.Contains(set))
                        {
                            var traitSet = (TraitSet)Activator.CreateInstance(set.GetType());
                            traitSet.Activate(character);
                            currentSet.Add(traitSet);
                        }
                    }
                    else
                    {
                        //generate a new Character with traits or return that the hero does not exist
                    }
                }
            }
        }

        public static void GivePlayerTraitSet(TraitSet traitSet) //give player a traitset
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
                    playerTraits.Add(traitSet);
                    traitSet.Activate(CharacterObject.PlayerCharacter);
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
            CampaignEvents.OnGameLoadedEvent.AddNonSerializedListener(this, starter =>
            {
                GivePlayerTraitSet(new BigCharacterTraitSet(null));
            });
            PopulateTraitSets();
        }

        public override void SyncData(IDataStore dataStore)
        {
        }
    }
}