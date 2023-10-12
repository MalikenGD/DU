using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace InfiniteVoid.SpamFramework.Core.Infrastructure.Logging
{
    internal class UnityDebugSpamLogger : ISpamLogger
    {
        public void LogDebug(string text, Object context = null)
        {
            Debug.Log(text, context);
        }
        
        public void LogWarning(string text, Object context = null)
        {
            Debug.LogWarning(text, context);
        }

        public void LogError(string text, Object context = null)
        {
            Debug.LogError(text, context);
        }
    }
}