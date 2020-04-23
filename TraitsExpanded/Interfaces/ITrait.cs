using TaleWorlds.CampaignSystem;

namespace TraitsExpanded
{
    //Interface for the Trait class
    public interface ITrait
    {
        string Name { get; set; }
        string Desc { get; set; }
        int Cost { get; set; }
        string IconPath { get; set; }
        bool isActive { get; set; }
        CharacterObject currentCharacter { get; set; }
        void Init();
        bool Activate(CharacterObject character);
        bool Deactivate(CharacterObject character);

       
    }
}