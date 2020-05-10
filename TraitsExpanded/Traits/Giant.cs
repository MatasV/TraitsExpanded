using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace TraitsExpanded.Traits
{
    public class Giant : MBSubModuleBase, ITrait
    {
        public string Name { get; set; } = "Giant";

        public string Description { get; set; } = "You are quite bigger than your average person";

        public int Cost { get; set; } = 5;

        public string IconPath { get; set; } = "";

        public bool IsActive { get; set; }

        public CharacterObject CurrentCharacter { get; set; }

        private bool enlarged;
        
        public Giant(CharacterObject character)
        {
            Init();
        }
        
        public void Init()
        {
            InformationManager.DisplayMessage(new InformationMessage("Trait has been initialized for character: "));
        }

        public bool Activate(CharacterObject character)
        {
            if (character == null) return false;
            
            CurrentCharacter = character;
            IsActive = true;
            CampaignEvents.MissionTickEvent.AddNonSerializedListener(this, ChangeCharacterBuild);
            CampaignEvents.OnMissionEndedEvent.AddNonSerializedListener(this, mission => { enlarged = false; });
            InformationManager.DisplayMessage(new InformationMessage("Trait has been activated for character: " + character?.Name ));
            return true;
        }

        private void ChangeCharacterBuild(float tick)
        {
            if (IsActive)
            {
                if (Mission.Current != null && !enlarged && Mission.Current.Agents.Any(agent => agent.Name == CurrentCharacter.ToString()))
                {
                    Util.LogMessage("Activating Giant Trait");

                    Agent agent = Mission.Current.Agents
                        .First(agent1 => agent1.Character.Id == CurrentCharacter.Id);

                    
                    enlarged = true;
                    
                }
            }
        }
        
        public bool Deactivate(CharacterObject character)
        {
            IsActive = false;
            enlarged = false;
            InformationManager.DisplayMessage(new InformationMessage("Trait has been deactivated for character: " + character?.Name ));
            return true;
        }
    }
}
