using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace InfiniteVoid.SpamFramework.Core.Utils
{
    [Serializable]
    public class FloatValue
    {
        public FloatValueType Type;

        [SerializeField] private float _minValue;
        [SerializeField] private float _maxValue;

        public float GetValue() => Type == FloatValueType.RandomBetweenTwoConstants
            ? Random.Range(_minValue, _maxValue)
            : _minValue;
    }

    public enum FloatValueType
    {
        Constant,
        RandomBetweenTwoConstants
    }
}