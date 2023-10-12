using System;
using InfiniteVoid.SpamFramework.Core.Components;
using InfiniteVoid.SpamFramework.Core.ExternalSystems;

namespace InfiniteVoid.SpamFramework.Core.Conditions
{
    /// <summary>
    /// A condition that is currently active on a target. Only used within a <see cref="ConditionsSet"/>
    /// </summary>
    public class ActiveCondition
    {
        public AbilityConditionSO Condition { get; private set; }
        public float Lifetime { get; private set; }
        private AbilityInvoker _appliedBy;
        private float _timeSinceLastTick = 0;
        private event Action<AbilityConditionSO> _conditionTicked;
        
        public ActiveCondition(AbilityConditionSO condition, float lifetime, AbilityInvoker appliedBy,
            Action<AbilityConditionSO> onConditionTicked)
        {
            this.Condition = condition;
            this.Lifetime = lifetime;
            this._appliedBy = appliedBy;
            this._conditionTicked += onConditionTicked;
        }


        /// <summary>
        /// Sets the lifetime to the condition, overwriting any lifetime that's left.
        /// </summary>
        /// <param name="lifetime"></param>
        public void SetLifetime(float lifetime) => this.Lifetime = lifetime;

        /// <summary>
        /// Extends the lifetime of the condition with the given amount.
        /// </summary>
        /// <param name="additionalLifetime"></param>
        public void ExtendLifetime(float additionalLifetime) => this.Lifetime += additionalLifetime;

        public void TickLifetime(float deltaTime)
        {
            this.Lifetime -= deltaTime;
            _timeSinceLastTick += deltaTime;
        }

        // ReSharper disable once ArrangeObjectCreationWhenTypeEvident
        private static ActiveCondition _noneInstance = new ActiveCondition(null, -1, null, null);
        public static ActiveCondition None => _noneInstance;
        public bool IsPermanent => this.Lifetime == 0;
        public bool IsExpired => this.Lifetime <= 0;

        public bool IsSameConditionAs(AbilityConditionSO condition) =>
            this.Condition && this.Condition.IsSameAs(condition);

        /// <summary>
        /// Applies the condition's secondary effect immediately without tick-checks
        /// </summary>
        /// <param name="target"></param>
        /// <param name="conditionEffectEvent"></param>
        public void ApplySecondaryEffects(IAbilityTarget target, ConditionEffectEvent conditionEffectEvent)
        {
            if (!Condition.HasSecondaryEffects) return;
            Condition.ApplySecondaryEffects(target, target.Transform.position, _appliedBy, conditionEffectEvent);
        }

        /// <summary>
        /// Apples the condition's secondary effect if it's been enough time since it's last tick
        /// </summary>
        /// <param name="abilityTarget"></param>
        public void ApplyTickedSecondaryEffects(IAbilityTarget abilityTarget)
        {
            if (!Condition.HasTickedEffects) return;
            if (_timeSinceLastTick < Condition.TimeBetweenTicks) return;

            _conditionTicked?.Invoke(this.Condition);
            Condition.ApplySecondaryEffects(abilityTarget, abilityTarget.Transform.position, _appliedBy, ConditionEffectEvent.Ticked);
            _timeSinceLastTick = 0;
        }
    }
}