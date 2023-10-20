using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using InfiniteVoid.SpamFramework.Core.AbilityData;
using UnityEditor;
using UnityEngine;

namespace InfiniteVoid.SpamFramework.Core.Common.Pooling
{
    [Serializable]
    public class PoolingSettingsSO : ScriptableObject, ISerializationCallbackReceiver
    {
        public List<PooledVfx> PooledVfx = new List<PooledVfx>();
        public List<PooledProjectile> PooledProjectiles = new List<PooledProjectile>();

        public Dictionary<int, int> VFXWithSharedPool = new Dictionary<int, int>();
        public Dictionary<int, int> ProjectilePools = new Dictionary<int, int>();

        public void OnBeforeSerialize() { }

        public void OnAfterDeserialize()
        {
            VFXWithSharedPool = new Dictionary<int, int>();
            for (int i = 0; i < PooledVfx.Count; i++)
                VFXWithSharedPool.Add(PooledVfx[i].Prefab.GetHashCode(), PooledVfx[i].NumInstances);
            ProjectilePools = new Dictionary<int, int>();
            for (int i = 0; i < PooledProjectiles.Count; i++)
            {
                ProjectilePools.Add(PooledProjectiles[i].ProjectileAbilityId,
                    PooledProjectiles[i].NumInstances);
            }
        }

#if UNITY_EDITOR
        [Conditional("UNITY_EDITOR")]
        public void AddProjectiles(ProjectileAbilitySO[] projectileAbililies)
        {
            for (int i = PooledProjectiles.Count - 1; 0 <= i; i--)
            {
                if (projectileAbililies.All(pa => pa.Id != PooledProjectiles[i].ProjectileAbilityId))
                    PooledProjectiles.RemoveAt(i);
            }
            
            foreach (var ability in projectileAbililies)
            {
                if (!ability.Prefab) continue;
                this.AddProjectile(ability, 5);
            }
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssetIfDirty(this);
        }

        [Conditional("UNITY_EDITOR")]
        public void AddVFXs(AbilityBaseSO[] abilitiesWithPooledVfx)
        {
            var pooledVfxFromAbilities = abilitiesWithPooledVfx.Select(x => x.VFX.OnHitVfx).ToArray();
            for (int i = PooledVfx.Count - 1; 0 <= i; i--)
            {
                if (!pooledVfxFromAbilities.Contains(PooledVfx[i].Prefab))
                    PooledVfx.RemoveAt(i);
            }
            
            foreach (var ability in abilitiesWithPooledVfx)
            {
                if (!ability.VFX.OnHitVfx) continue;
                this.AddVFX(ability.VFX.OnHitVfx, 5);
            }
        }
#endif
        public int GetNumInstancesForProjectileAbility(ProjectileAbilitySO projectileAbilitySO) => ProjectilePools[projectileAbilitySO.Id];
        
        private void AddVFX(GameObject gameobject, int count)
        {
            if (PooledVfx.Select(x => x.Prefab).Contains(gameobject)) return;
            PooledVfx.Add(new PooledVfx { Prefab = gameobject, NumInstances = count });
        }

        private void AddProjectile(ProjectileAbilitySO projectileAbility, int count)
        {
            if (PooledProjectiles.Any(x => x.ProjectileAbilityId == projectileAbility.Id)) return;
            PooledProjectiles.Add(new PooledProjectile { ProjectileAbilityId = projectileAbility.Id, NumInstances = count });
        }
    }

    [Serializable]
    public struct PooledVfx
    {
        public GameObject Prefab;
        public int NumInstances;
    }

    [Serializable]
    public struct PooledProjectile
    {
        public int ProjectileAbilityId;
        public int NumInstances;
    }
}