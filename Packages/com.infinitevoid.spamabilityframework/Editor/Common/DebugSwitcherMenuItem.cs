using UnityEditor;

namespace InfiniteVoid.SpamFramework.Editor.Common
{
    public static class DebugSwitcherMenuItem
    {
        private const string SPAM_DEBUG_SYMBOL = "SPAM_DEBUG";

#if SPAM_DEBUG
        [MenuItem("Tools/SPAM Framework/Disable debug mode", isValidateFunction: false, priority: 501)]
        static void DisableSpamDebug()
        {
            BuildTargetGroup group = EditorUserBuildSettings.selectedBuildTargetGroup;
            string symbols = PlayerSettings.GetScriptingDefineSymbolsForGroup(group);
            symbols = symbols.Replace(SPAM_DEBUG_SYMBOL, string.Empty);
            PlayerSettings.SetScriptingDefineSymbolsForGroup(group, symbols);
        }

        [MenuItem("Tools/SPAM Framework/Disable debug mode", true)]
        static bool ValidateDisableSpamDebug()
        {
            BuildTargetGroup group = EditorUserBuildSettings.selectedBuildTargetGroup;
            var defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(group);
            return defines.Contains(SPAM_DEBUG_SYMBOL);
        }
#else
        [MenuItem("Tools/SPAM Framework/Enable debug mode", isValidateFunction: false, priority: 500)]
        static void EnableSpamDebug()
        {
            BuildTargetGroup group = EditorUserBuildSettings.selectedBuildTargetGroup;
            PlayerSettings.SetScriptingDefineSymbolsForGroup(group, SPAM_DEBUG_SYMBOL);
        }

        [MenuItem("Tools/SPAM Framework/Enable debug mode", true)]
        static bool ValidateEnableSpamDebug()
        {
            BuildTargetGroup group = EditorUserBuildSettings.selectedBuildTargetGroup;
            var defines = PlayerSettings.GetScriptingDefineSymbolsForGroup(group);
            return !defines.Contains(SPAM_DEBUG_SYMBOL);
        }
#endif
    }
}