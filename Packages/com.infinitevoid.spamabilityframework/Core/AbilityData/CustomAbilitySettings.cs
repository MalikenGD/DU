using System;
using UnityEngine;

namespace InfiniteVoid.SpamFramework.Core.AbilityData
{
    /// <summary>
    /// Extend this to add your own ability data to abilities.
    /// You can get this data back by calling <see cref="AbilityBaseSO.GetCustomAbilityData{T}"/> with the type(s) you've created.
    /// </summary>
    public abstract class CustomAbilitySettings : ScriptableObject { }
}