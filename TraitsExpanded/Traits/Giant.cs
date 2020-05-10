using System.Linq;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;

namespace TraitsExpanded.Traits
{
    public class Giant : MBSubModuleBase, ITrait
    {
        public string Name { get; set; } = "Giant";
        public string Desc { get; set; } = "You are quite bigger than your average person";
        public int Cost { get; set; } = 5;
        public string IconPath { get; set; } = "";

        public bool isActive { get; set; }
        public CharacterObject currentCharacter { get; set; }

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
            
            currentCharacter = character;
            isActive = true;
            CampaignEvents.MissionTickEvent.AddNonSerializedListener(this, ChangeCharacterBuild);
            CampaignEvents.OnMissionEndedEvent.AddNonSerializedListener(this, mission => { enlarged = false; });
            InformationManager.DisplayMessage(new InformationMessage("Trait has been activated for character: " + character?.Name ));
            return true;
        }

        private void ChangeCharacterBuild(float tick)
        {
            if (isActive)
            {
                if (Mission.Current != null && !enlarged && Mission.Current.Agents.Any(agent => agent.Name == currentCharacter.ToString()))
                {
                    Util.LogMessage("Activating Giant Trait");

                    Agent agent = Mission.Current.Agents
                        .First(agent1 => agent1.Character.Id == currentCharacter.Id);

                    
                    enlarged = true;
                    
                }
            }
        }
        
        public bool Deactivate(CharacterObject character)
        {
            isActive = false;
            enlarged = false;
            InformationManager.DisplayMessage(new InformationMessage("Trait has been deactivated for character: " + character?.Name ));
            return true;
        }
    }
}