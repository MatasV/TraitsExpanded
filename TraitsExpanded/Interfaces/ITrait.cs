using TaleWorlds.CampaignSystem;

namespace TraitsExpanded
{
    /// <summary>
    /// Public interface for Trait implementations
    /// </summary>
    public interface ITrait
    {
        string Name { get; set; }

        string Description { get; set; }

        int Cost { get; set; }

        string IconPath { get; set; }

        bool IsActive { get; set; }

        CharacterObject CurrentCharacter { get; set; }

        void Init();

        bool Activate(CharacterObject character);

        bool Deactivate(CharacterObject character);
    }
}
