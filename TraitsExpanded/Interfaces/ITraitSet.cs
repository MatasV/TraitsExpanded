using System.Collections.Generic;
using TaleWorlds.CampaignSystem;

namespace TraitsExpanded
{
    public interface ITraitSet
    {
        List<ITrait> Traits { get; set; }
        string ID { get; set; }
        int CurrentTraitIndex { get; set; }
        NPCRestrictionEnum NPCRestriction { get; set; }
        bool IsActive { get; set; }
        Hero CurrentHero { get; set; }
    }
}