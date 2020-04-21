using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.CampaignSystem;

namespace TraitsExpanded
{
    public class TraitsExpandedSubModule : MBSubModuleBase
    {
	    private readonly TraitSystemBehavior traitSystemBehavior = new TraitSystemBehavior();

	    protected override void OnApplicationTick(float dt)
	    {
		    traitSystemBehavior.Tick();
	    }

	    /* Executed when game is loaded or initially started */
		protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
		{
			if (game.GameType is Campaign)
			{
				CampaignGameStarter gameInitializer = (CampaignGameStarter)gameStarterObject;
				this.AddBehaviors(gameInitializer);
			}
		}
		
		/* The Behaviors we add */
		private void AddBehaviors(CampaignGameStarter gameStarterObject)
		{
			gameStarterObject.AddBehavior(traitSystemBehavior);
		}
	}
}