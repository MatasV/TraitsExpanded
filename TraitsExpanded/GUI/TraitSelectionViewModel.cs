using TaleWorlds.Engine.Screens;
using TaleWorlds.Library;
using TaleWorlds.Localization;

namespace TraitsExpanded.GUI
{
    class TraitSelectionViewModel : ViewModel
    {
        public override void RefreshValues()
        {
            base.RefreshValues();
        }

		#region Callbacks

		private void ExecuteDone()
		{
			ScreenManager.PopScreen();
		}

		private void ExecuteCancel()
		{
			ScreenManager.PopScreen();
		}

		#endregion

	}
}
