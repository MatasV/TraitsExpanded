using TaleWorlds.CampaignSystem;

namespace TraitsExpanded
{
    /// <summary>
    /// Public interface for Trait implementations
    /// </summary>
    public interface ITrait
    {
        string Name { get;}
        
        string Description { get; }
        
        int Cost { get; }
        
        string IconPath { get; }
        
        bool IsActive { get; }
        
        CharacterObject CurrentCharacter { get; }
        
        bool Activate(CharacterObject character);
        
        bool Deactivate(CharacterObject character);
    }
    
}