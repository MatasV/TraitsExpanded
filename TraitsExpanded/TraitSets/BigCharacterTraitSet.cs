using System;
using System.Collections.Generic;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade.Diamond;
using TraitsExpanded.Traits;

namespace TraitsExpanded.TraitSets
{
    public class BigCharacterTraitSet : TraitSet
    {
        public override string Name { get; } = "Character Size";
        public override string Id { get; } = "BigCharacterTraitSet";

        public BigCharacterTraitSet(CharacterObject character) : base(character)
        {
            Traits.Add(new Giant(character));
        }
        

        public override void MissionTick(MissionStatusInfo statusInfo)
        {
        }

        public override void CampaignTick(CampaignStatusInfo statusInfo)
        {
        }

        public override void GameTick(GameStatusInfo statusInfo)
        {
        }
    }
}