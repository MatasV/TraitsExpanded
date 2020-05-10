using TaleWorlds.Core;
using TaleWorlds.MountAndBlade;
using TaleWorlds.CampaignSystem;


namespace TraitsExpanded
{
	public class TraitsExpandedSubModule : MBSubModuleBase
    {
	    private TraitSystemBehavior traitSystemBehavior;

	    protected override void OnApplicationTick(float dt)
	    {
		    traitSystemBehavior?.Tick();
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
			gameStarterObject.AddBehavior(traitSystemBehavior);
		}

		/* Currently just for testing the GUI */
		protected override void OnSubModuleLoad()
		{
			base.OnSubModuleLoad();
			Module.CurrentModule.AddInitialStateOption(new InitialStateOption("TraitsExpandedGUI", new TextObject("TE GUI test", null), 9998, delegate ()
			{
				ScreenManager.PushScreen(new TraitSelectionGauntletScreen());
			}, false));
		}
	}
}
