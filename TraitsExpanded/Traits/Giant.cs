using System;
using System.Linq;
using System.Reflection;
using TaleWorlds.CampaignSystem;
using TaleWorlds.Core;
using TaleWorlds.Engine;
using TaleWorlds.Library;
using TaleWorlds.MountAndBlade;
using TaleWorlds.MountAndBlade.View;

namespace TraitsExpanded.Traits
{
    public class Giant : MBSubModuleBase, ITrait
    {
        public string Name { get; } = "Giant";
        
        public string Description { get; } = "You are quite bigger than your average person";
        
        public int Cost { get; } = 5;
        
        public string IconPath { get; } = "";

        public bool IsActive { get; private set; }
        
        public CharacterObject CurrentCharacter { get; private set; }

        private bool enlarged;
        
        public Giant(CharacterObject character)
        {
            CurrentCharacter = character;
            InformationManager.DisplayMessage(new InformationMessage("Trait has been initialized for character: "));
        }

        public bool Activate(CharacterObject character)
        {
            if (character == null || IsActive) return false;
            
            CurrentCharacter = character;
            IsActive = true;
            CampaignEvents.MissionTickEvent.AddNonSerializedListener(this, ChangeCharacterBuild);
            CampaignEvents.OnMissionEndedEvent.AddNonSerializedListener(this, mission => { enlarged = false; });
            InformationManager.DisplayMessage(new InformationMessage("Trait has been activated for character: " + character.Name ));
            return true;
        }

        private void ChangeCharacterBuild(float tick)
        {
            if (IsActive && !enlarged) //NOT DONE, CAN'T FIGURE IT OUT
            {
                if (Mission.Current != null && !enlarged && Mission.Current.Agents.Any(agent => agent.Name == CurrentCharacter.ToString()))
                {
                    
                    
                    Agent agent = Mission.Current.Agents
                        .First(agent1 => agent1.Name == CurrentCharacter.Name.ToString());

                    if (agent.AgentVisuals == null) return;
                    //var t = typeof ( TaleWorlds.MountAndBlade.View.CharacterSpawner);
                    //var assembly = Assembly.Load(t.Assembly.GetName());
                    //string s = t.Assembly.FullName.ToString();
                    //CharacterSpawner spawner = 
                    //if (assembly == null) return;
                   // var obj = agent.AgentVisuals.GetType().GetMethod("GetFrame")
                       // .GetType().GetMethod("Scale");
                    
                    //obj.Invoke()
                    agent.AgentVisuals.GetFrame().Scale(new Vec3(20f, 20f, 20f, 1f));
                    agent.UpdateAgentProperties();
                    agent.UpdateAgentStats();
                    agent.UpdateCustomDrivenProperties();
                    agent.UpdateWeapons();
                        enlarged = true;
                        Util.LogMessage("Activating Giant Trait");

                }
            }
        }
        private Assembly GetAssemblyByName(string name)
        {
            return AppDomain.CurrentDomain.GetAssemblies().
                SingleOrDefault(assembly => assembly.GetName().Name == name);
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