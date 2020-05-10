using TaleWorlds.Engine.GauntletUI;
using TaleWorlds.Engine.Screens;
using TaleWorlds.Library;

namespace TraitsExpanded.GUI
{
    class TraitSelectionGauntletScreen : ScreenBase
    {
		private GauntletLayer _gauntletLayer;
		private TraitSelectionViewModel _viewModel;

		public TraitSelectionGauntletScreen()
		{
		}

		protected override void OnInitialize()
		{
			base.OnInitialize();
			this._viewModel = new TraitSelectionViewModel();

			// Add and configure layers
			this._gauntletLayer = new GauntletLayer(1, "GauntletLayer");
			this._gauntletLayer.LoadMovie("TraitSelection", this._viewModel);
			this._gauntletLayer.InputRestrictions.SetInputRestrictions(true, InputUsageMask.All);

			base.AddLayer(this._gauntletLayer);
		}

		protected override void OnFinalize()
		{
			// Cleanup
			base.OnFinalize();
			base.RemoveLayer(this._gauntletLayer);

			this._gauntletLayer = null;
			this._viewModel = null;
		}
	}
}
