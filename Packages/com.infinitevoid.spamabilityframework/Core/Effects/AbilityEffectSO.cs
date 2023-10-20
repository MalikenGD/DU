using System;
using InfiniteVoid.SpamFramework.Core.AbilityData;
using InfiniteVoid.SpamFramework.Core.Components;
using InfiniteVoid.SpamFramework.Core.ExternalSystems;
using InfiniteVoid.SpamFramework.Core.Infrastructure;
using InfiniteVoid.SpamFramework.Core.Infrastructure.Logging;
using InfiniteVoid.SpamFramework.Core.Utils;
using UnityEngine;

namespace InfiniteVoid.SpamFramework.Core.Effects
{
    /// <summary>
    /// Base for all ability effects, allowing them to be handled generically
    /// </summary>
    [Serializable]
    public abstract class AbilityEffectSO : ScriptableObject
    {
        /// <summary>
        /// The help description for the object. This is shown in the inspector
        /// </summary>
        protected abstract string _metaHelpDescription { get; }

        [SerializeField] private string _name;
        [Multiline] [SerializeField] private string _description;

        // [HelpBox(
        //     "If this is turned off then the effect won't be included in the list of effects an ability publicly has. Useful for removing an effect from the abilities public interface so the effect can be listed in the GUI.")]
        [SerializeField]
        private bool _includedInAbilityEffects = true;

        /// <summary>
        /// Gets the help description. Used in AbilityEffectSOEditor.
        /// </summary>
        public string HelpDescription => _metaHelpDescription;

        public string Description => _description;
        public string Name => _name;
        public bool IncludedInAbilityEffects => _includedInAbilityEffects;

        protected const string LOG_MODULE = SpamLogModules.ABILITY_EFFECTS;

        /// <summary>
        /// Applies the effect to the given target
        /// </summary>
        /// <param name="target">The target of the effect</param>
        /// <param name="abilityPos">The position the ability strikes at</param>
        /// <param name="ability">[CAN BE NULL] The ability the effect is attached to. Since effects can be attached to Conditions, this can be null.</param>
        /// <param name="invoker">The caster applying the effect</param>
        public abstract void ApplyTo(IAbilityTarget target, Vector3 abilityPos, IAbilityData ability,
            AbilityInvoker invoker);

        public override string ToString()
        {
            var effectName = this.Name.IsNullOrWhitespace()
                ? string.Empty
                : $"{this.Name}";
            var desc = _description.IsNullOrWhitespace()
                ? string.Empty
                : _description;
            string separator = effectName.IsNotNullOrWhitespace() && desc.IsNotNullOrWhitespace() ? ": " : " ";
            return $"{effectName}{separator}{desc}";
        }
    }
}