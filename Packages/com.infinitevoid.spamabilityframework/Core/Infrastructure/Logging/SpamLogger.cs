using System;
using System.Diagnostics;
using UnityEngine;
using Object = UnityEngine.Object;

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
        // kept as variable to avoid GC and spamming the log when possible
        private static string _lastLog;
        public static ISpamLogger Instance => _instance ??= new UnityDebugSpamLogger();

        public static void SetLogger(ISpamLogger logger) => _instance = logger;
        
        
        public static void LogError(string text, Object context = null)
        {
            if(_lastLog == text) return;
            _lastLog = text;
            Instance.LogError(text, context);
        }
        
        public static void LogError(string module, string text, Object context = null)
        {
            if(_lastLog == text) return;
            _lastLog = text;
            Instance.LogError($"{module} {text}", context);
        }
        
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
        
        [Conditional("SPAM_DEBUG")] 
        public static void LogDebug(bool condition, string module, Func<string> text, Object context = null)
        {
            if (!condition) return;
            LogDebug(module, text?.Invoke(), context);
        }

        [Conditional("DEVELOPMENT_BUILD"), Conditional("UNITY_EDITOR"), Conditional("SPAM_DEBUG")] 
        public static void LogWarning(string text, Object context = null)
        {
        #if UNITY_EDITOR || DEVELOPMENT_BUILD || SPAM_DEBUG
            if(_lastLog == text) return;
            _lastLog = text;
            Instance.LogWarning(text, context);
        #endif
        }
        
        [Conditional("DEVELOPMENT_BUILD"), Conditional("UNITY_EDITOR"), Conditional("SPAM_DEBUG")] 
        public static void LogWarning(bool condition, string text, Object context = null)
        {
        #if UNITY_EDITOR || DEVELOPMENT_BUILD || SPAM_DEBUG
            if (!condition) return;
            LogWarning(text, context);
        #endif
        }


        [Conditional("UNITY_EDITOR")] 
        public static void EditorOnlyLog(string text, GameObject context = null)
        {
        #if UNITY_EDITOR
            Instance.LogDebug(text, context);
        #endif
        }
        
        [Conditional("UNITY_EDITOR")] 
        public static void EditorOnlyWarning(bool condition, string text, GameObject context = null)
        {
        #if UNITY_EDITOR
            if (!condition) return;
            Instance.LogWarning(text, context);
        #endif
        }
        
        [Conditional("UNITY_EDITOR")] 
        public static void EditorOnlyWarning(string text, GameObject context = null)
        {
        #if UNITY_EDITOR
            Instance.LogWarning(text, context);
        #endif
        }
        
        [Conditional("UNITY_EDITOR")] 
        public static void EditorOnlyLog(bool condition, string text, GameObject context = null)
        {
        #if UNITY_EDITOR
            if (!condition) return;
            Instance.LogDebug(text, context);
        #endif
        }
        
        [Conditional("UNITY_EDITOR")] 
        public static void EditorOnlyLog(bool condition, Func<string> text, GameObject context = null)
        {
        #if UNITY_EDITOR
            if (!condition) return;
            Instance.LogDebug(text?.Invoke(), context);
        #endif
        }

        
        [Conditional("UNITY_EDITOR")] 
        public static void EditorOnlyErrorLog(bool condition, string text, Object context = null)
        {
        #if UNITY_EDITOR
            if (!condition) return;
            Instance.LogError(text, context);
        #endif
        }
        
        [Conditional("UNITY_EDITOR")] 
        public static void EditorOnlyErrorLog(string text, Object context = null)
        {
        #if UNITY_EDITOR
            Instance.LogError(text, context);
        #endif
        }
        
        [Conditional("UNITY_EDITOR")] 
        public static void EditorOnlyErrorLog(string module, string text, Object context = null)
        {
        #if UNITY_EDITOR
            Instance.LogError($"{module} {text}", context);
        #endif
        }
    }
}
