using System;
using TaleWorlds.CampaignSystem;
using TraitsExpanded.Traits;

namespace TraitsExpanded.TraitSets
{
    public class BigCharacterTraitSet : TraitSet
    {
        public override Guid Id => new Guid("7f7d6c9c-7034-4cb8-95fd-acdc94738b65");

        public override string Name => "BigCharacter";

        public override int Version => 1;

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
                Util.LogMessage("{0} initialized for a given character! {1} Activating!", this.Name, character.Name);
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
