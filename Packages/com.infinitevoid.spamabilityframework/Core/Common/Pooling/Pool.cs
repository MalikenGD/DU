using System;
using System.Collections.Generic;
using UnityEditor;
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
        private readonly Func<T> _createPooledItem;

        public T[] Items => _pool;

        public Pool(int poolCount, Func<T> createPooledItem, bool spawnImmediately = true)
        {
            _numInPool = poolCount;
            _pool = new T[_numInPool];
            _createPooledItem = createPooledItem;
            if(spawnImmediately)
                SpawnItems();
        }

        public Pool(T[] initialItems)
        {
            _numInPool = initialItems.Length;
            _pool = initialItems;
        }

        public void SpawnItems()
        {
            if (_pool[0]) return;
            
            for (int i = 0; i < _numInPool; i++)
            {
                var instance = _createPooledItem.Invoke(); 
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
#if UNITY_EDITOR
        /// <summary>
        /// Clear and empties the pool.
        /// Usable in editor only
        /// </summary>
        public void Clear()
        {
            for (int i = 0; i < _pool.Length; i++)
                _pool[i] = null;
            // _pool = Array.Empty<T>();
        }

        /// <summary>
        /// Destroys all items in the pool (all children).
        /// Usable in editor only
        /// </summary>
        private void DestroyPooledItems()
        {
            for (int i = 0; i < _pool.Length; i++)
            {
                if(_pool[i] != null && _pool[i].gameObject)
                    UnityEngine.Object.DestroyImmediate(_pool[i].gameObject);
            }
        }
#endif
    }
}