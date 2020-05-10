using TaleWorlds.Core;

namespace TraitsExpanded
{
    public static class Util
    {
        public static void LogMessage(string message)
        {
            InformationManager.DisplayMessage(new InformationMessage(message));
        }

        public static void LogMessage(string message, params object[] args)
        {
            InformationManager.DisplayMessage(new InformationMessage(string.Format(message, args)));
        }
    }
}
