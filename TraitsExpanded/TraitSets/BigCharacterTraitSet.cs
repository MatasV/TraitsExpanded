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
        public override string ID { get; set; } = "BigCharacterTraitSet";

        public BigCharacterTraitSet(CharacterObject characterObject)
        {
            Init(characterObject);
        }
        
        public override void Init(CharacterObject character)
        {
            base.Init(character);
            Traits.Add(new Giant(character));
            CurrentCharacter = character;
            if (CurrentCharacter != null)
            {
                Util.LogMessage("BigCharacterTraitSet initialized for a given character!" + character.Name + " Activating!");
                Activate();
            }
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