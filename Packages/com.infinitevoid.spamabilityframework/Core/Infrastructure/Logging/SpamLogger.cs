using System.Diagnostics;
using UnityEngine;

namespace InfiniteVoid.SpamFramework.Core.Infrastructure.Logging
{
    /// <summary>
    /// Main logger for SPAM framework.
    /// This is a facade/wrapper that abstracts away the actual logging behaviour
    /// so it can be configured globally and stripped at build
    /// </summary>
    public class SpamLogger 
    {
        private static ISpamLogger _instance;
        // kept as variable to avoid GC when possible
        private static string _lastLog;
        public static ISpamLogger Instance => _instance ??= new UnityDebugSpamLogger();

        public static void SetLogger(ISpamLogger logger) => _instance = logger;
        
        [Conditional("SPAM_DEBUG")] 
        public static void LogDebug(string module, string text, Object context = null)
        {
            if(_lastLog == text) return;
            _lastLog = text;
            Instance.LogDebug($"{module} {text}", context);
        }
        
        [Conditional("SPAM_DEBUG")] 
        public static void LogDebug(bool condition, string module, string text, Object context = null)
        {
            if (!condition) return;
            LogDebug(module, text, context);
        }


        [Conditional("DEVELOPMENT_BUILD"), Conditional("UNITY_EDITOR"), Conditional("SPAM_DEBUG")] 
        public static void LogWarning(string text, Object context = null)
        {
            if(_lastLog == text) return;
            _lastLog = text;
            Instance.LogWarning(text, context);
        }
        
        [Conditional("DEVELOPMENT_BUILD"), Conditional("UNITY_EDITOR"), Conditional("SPAM_DEBUG")] 
        public static void LogWarning(bool condition, string text, Object context = null)
        {
            if (!condition) return;
            LogWarning(text, context);
        }


        public static void LogError(string text, Object context = null)
        {
            if(_lastLog == text) return;
            _lastLog = text;
            Instance.LogError(text, context);
        }

        [Conditional("UNITY_EDITOR")] 
        public static void EditorOnlyLog(string text, GameObject context = null)
        {
            Instance.LogDebug(text, context);
        }
        
        [Conditional("UNITY_EDITOR")] 
        public static void EditorOnlyLog(bool condition, string text, GameObject context = null)
        {
            if (!condition) return;
            Instance.LogDebug(text, context);
        }
        
        [Conditional("UNITY_EDITOR")] 
        public static void EditorOnlyErrorLog(bool condition, string text, GameObject context = null)
        {
            if (!condition) return;
            Instance.LogError(text, context);
        }

    }
}
