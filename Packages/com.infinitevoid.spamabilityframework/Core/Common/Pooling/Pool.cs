using System;
using UnityEngine;

namespace InfiniteVoid.SpamFramework.Core.Common.Pooling
{
    /// <summary>
    /// A generic pool for pooling everything deriving from <see cref="Component"/>
    /// </summary>
    /// <typeparam name="T"></typeparam>
    [Serializable]
    public class Pool<T> where T : Component
    {
        protected T this[int index] => _pool[index]; 
        private T[] _pool;
        private int _numInPool;
        protected int CurIndex = 0;

        public T[] Items => _pool;

        public Pool(int poolCount, Func<T> createPooledItem)
        {
            _numInPool = poolCount;
            _pool = new T[_numInPool];
            for (int i = 0; i < _numInPool; i++)
            {
                var instance = createPooledItem.Invoke(); 
                instance.gameObject.SetActive(false);
                _pool[i] = instance;
            } 
        }
        public T GetNext(Vector3 position, bool active, Vector3 forward)
        {
            var pooledItem = _pool[CurIndex];
            // This flips the active state so its an on-hit effect it can replay if necessary
            if (!pooledItem && CurIndex + 1 <= _numInPool)
                return GetNext(position, active, forward);
            pooledItem.gameObject.SetActive(false);
            pooledItem.transform.position = position;
            pooledItem.transform.forward = forward;
            pooledItem.gameObject.SetActive(active);
            CurIndex++;
            if (_numInPool <= CurIndex)
                CurIndex = 0;
            return pooledItem;
        }
    }
}