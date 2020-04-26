using TaleWorlds.CampaignSystem;
using TraitsExpanded.Traits;

namespace TraitsExpanded.TraitSets
{
    public class BigCharacterTraitSet : TraitSet
    {
        public override string Id { get; set; } = "BigCharacterTraitSet";

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
                Util.LogMessage("BigCharacterTraitSet initialized for a given character! {0} Activating!", character.Name);
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
