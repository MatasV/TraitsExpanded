using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.CampaignSystem;
using TaleWorlds.CampaignSystem.SandBox.CampaignBehaviors;
using TraitsExpanded.TraitSets;

namespace TraitsExpanded
{ 
	public class TraitsExpandedSubModule : MBSubModuleBase
    {
	    private TraitSystemBehavior traitSystemBehavior;

	    private CampaignGameStarter gameStarter;
	    
	    protected override void OnApplicationTick(float dt)
	    {
		    if (gameStarter != null && gameStarter.CampaignBehaviors.Contains(traitSystemBehavior))
		    {
			    traitSystemBehavior.Tick();
		    }
	    }

	    /// <summary>
	    /// Executed when game is loaded or initially started
	    /// </summary>
	    /// <param name="game"></param>
	    /// <param name="gameStarterObject"></param>
		protected override void OnGameStart(Game game, IGameStarter gameStarterObject)
		{
			if (game.GameType is Campaign)
			{
				CampaignGameStarter gameInitializer = (CampaignGameStarter)gameStarterObject;
				this.AddBehaviors(gameInitializer);
			}
		}
		
	    /// <summary>
	    /// The Behaviors we add
	    /// </summary>
		private void AddBehaviors(CampaignGameStarter gameStarterObject)
		{
			traitSystemBehavior = new TraitSystemBehavior();
			gameStarter = gameStarterObject;
			gameStarterObject.AddBehavior(traitSystemBehavior);
		}
	}
	
}