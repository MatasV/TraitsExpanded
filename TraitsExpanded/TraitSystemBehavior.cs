using System.Collections.Generic;

using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.CampaignBehaviors;
using TaleWorlds.Core;
using TaleWorlds.Engine;

namespace TraitsExpanded
{
    public class TraitSystemBehavior : CampaignBehaviorBase
    {
        public static readonly List<ITraitSet> AllTraitSets = new List<ITraitSet>();
        public static Dictionary<Hero, List<ITraitSet>> HeroTraitInfo = new Dictionary<Hero, List<ITraitSet>>();

        public delegate void CampaignTickCall(CampaignStatusInfo statusInfo);

        public static CampaignTickCall OnCampaignTick;

        public delegate void MissionTickCall(MissionStatusInfo statusInfo);

        public static MissionTickCall OnMissionTick;

        private static float MissionTickTime { get; set; } = 1f;
        private static float CampaignTickTime { get; set; } = 1f;
        private static float GameTickTime { get; set; } = 1f;

        private Timer MissionTickTimer { get; } = new Timer(Time.ApplicationTime, MissionTickTime, false);
        private Timer CampaignTickTimer { get; } = new Timer(Time.ApplicationTime, CampaignTickTime, false);
        private Timer GameTickTimer { get; } = new Timer(Time.ApplicationTime, GameTickTime, false);
        public TraitSystemBehavior()
        {
            Init();
        }

        private static void Init()
        {
            
        }

        public static void Tick()
        {
            
        }

        public void CheckRequirements()
        {
            
        }
        
        
        
        private bool AddTrait(Hero hero, ITraitSet traitSet)
        {
            return true;
        }

        private bool LoadHeroTraitData(Hero hero)
        {
            return true;
        }

        private bool SaveHeroTraitData(Hero hero)
        {
            return true;
        }

        private void GenerateHeroTraits(Hero hero)
        {
        }

        public List<ITrait> GetAllPositiveTraits()
        {
            return new List<ITrait>();
        }

        public List<ITrait> GetAllNegativeTraits()
        {
            return new List<ITrait>();
        }

        private void TickMission()
        {
        }

        private void TickCampaign()
        {
        }

        public void RegisterTraitSet(ITraitSet set, List<Hero> heroesToGiveTraitTo = null)
        {
            if (!AllTraitSets.Contains(set))
            {
                AllTraitSets.Add(set);
                if (heroesToGiveTraitTo != null)
                {
                    foreach (var hero in heroesToGiveTraitTo)
                    {
                        if(HeroTraitInfo.TryGetValue(hero, out var currentSet))
                        {
                            if (!currentSet.Contains(set))
                            {
                                currentSet.Add(set);
                            }
                        }
                        else
                        {
                            //generate a new Hero with traits or return that the hero does not exist
                        }
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