using System.Diagnostics;
using UnityEngine;

namespace InfiniteVoid.SpamFramework.Core.Infrastructure.Logging
{
    public interface ISpamLogger
    {
        void LogDebug(string text, Object context = null);
        void LogWarning(string text, Object context = null);
        void LogError(string text, Object context = null);
    }
}