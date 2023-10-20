using InfiniteVoid.SpamFramework.Core.ExternalSystems;
using UnityEngine;

namespace InfiniteVoid.SpamFramework.Core.Extensions
{
    public static class TransformExtensions
    {
        /// <summary>
        /// Tries to get the component T in parents or children of the given transform 
        /// </summary>
        /// <param name="t"></param>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetComponentInHierarchy<T>(this Transform t) where T : IAbilityTarget
        {
            T component = t.GetComponentInParent<T>();
            return component != null ? component : t.GetComponentInChildren<T>();
        }
    }
}