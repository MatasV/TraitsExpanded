using System.Collections.Generic;
using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.CampaignBehaviors;
using TaleWorlds.Core;
using TaleWorlds.Engine;

namespace TraitsExpanded
{
    public class TraitSystemBehavior : CampaignBehaviorBase
    {
        public static readonly List<ITraitSet> AllTraitSets = new List<ITraitSet>();
        public static Dictionary<CharacterObject, List<ITraitSet>> CharacterTraitInfo = new Dictionary<CharacterObject, List<ITraitSet>>();

        public delegate void CampaignTickCall(CampaignStatusInfo statusInfo);

        public static CampaignTickCall OnCampaignTick;

        public delegate void MissionTickCall(MissionStatusInfo statusInfo);

        public static MissionTickCall OnMissionTick;

        public delegate void GameTickCall(Game statusInfo);

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
        }

        public void Tick()
        {
            if (MissionTickTimer.Check(Time.ApplicationTime))
            {
                OnMissionTick.Invoke(new MissionStatusInfo());
                MissionTickTimer.Reset(Time.ApplicationTime, MissionTickTime);
            }
            if (CampaignTickTimer.Check(Time.ApplicationTime))
            {
                OnCampaignTick.Invoke(new CampaignStatusInfo());
                CampaignTickTimer.Reset(Time.ApplicationTime, CampaignTickTime);
            }
            if (GameTickTimer.Check(Time.ApplicationTime))
            {
                OnGameTick.Invoke(Game.Current);
                GameTickTimer.Reset(Time.ApplicationTime, MissionTickTime);
            }
        }

        public void CheckRequirements()
        {
            
        }

        private void ValidateHeroList(CampaignStatusInfo statusInfo)
        {
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
                    characterObjects.Add(character);
                    CharacterTraitInfo.Add(character, new List<ITraitSet>());
                }
            }

            foreach (var oldCharacterObject in CharacterTraitInfo.Keys
                .Where(oldCharacterObject => characterObjects
                    .All(freshCharacterObject => oldCharacterObject != freshCharacterObject)))
            {
                //should save the character traits at this point since we will be removing them from the active Character List
                CharacterTraitInfo.Remove(oldCharacterObject);
            }
        }
        
        private bool AddTraitSet(CharacterObject character, ITraitSet traitSet)
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
            return (from traitSet in AllTraitSets from 
            trait in traitSet.Traits where trait.Cost >= 0 select trait).ToList();
        }

        public List<ITrait> GetAllNegativeTraits()
        {
            return (from traitSet in AllTraitSets from 
                trait in traitSet.Traits where trait.Cost < 0 select trait).ToList();
        }

        public void RegisterTraitSet(ITraitSet set, List<CharacterObject> charactersToGiveTraitTo = null)
        {
            if (!AllTraitSets.Contains(set))
            {
                AllTraitSets.Add(set);
                if (charactersToGiveTraitTo == null) return;
                
                foreach (var character in charactersToGiveTraitTo)
                {
                    if(CharacterTraitInfo.TryGetValue(character, out var currentSet))
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

        public override void RegisterEvents()
        {
            throw new System.NotImplementedException();
        }

        public override void SyncData(IDataStore dataStore)
        {
            throw new System.NotImplementedException();
        }
    }
}