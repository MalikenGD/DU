using System;
using InfiniteVoid.SpamFramework.Core.Utils;
using UnityEngine;

namespace InfiniteVoid.SpamFramework.Core.Projectiles
{
    [Serializable]
    public struct CurvedMovementSettings
    {
        [SerializeField] private AnimationCurve _curve;
        [SerializeField] FloatValue _strength;

        private float _cachedStrength;
        public float Evaluate(float percentageAlongCurve) => 0 < _cachedStrength && _curve != null ? _curve.Evaluate(percentageAlongCurve) * _cachedStrength : 1;

        public void CacheStrength() => _cachedStrength = _strength.GetValue();
    }
}