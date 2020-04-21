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

        void Init(Hero hero);
        bool Activate(Hero hero);
        bool Deactivate(Hero hero);
    }
}