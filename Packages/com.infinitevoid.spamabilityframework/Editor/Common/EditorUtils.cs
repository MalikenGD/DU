using System.IO;
using System.Linq;
using InfiniteVoid.SpamFramework.Core.AbilityData;
using InfiniteVoid.SpamFramework.Core.Common.Pooling;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace InfiniteVoid.SpamFramework.Editor.Common
{
    /// <summary>
    /// Utils to help with common editor tasks, like handling different unity versions
    /// </summary>
    public static class EditorUtils
    {
        internal const string PACKAGE_BASE_PATH = "Packages/com.infinitevoid.spamabilityframework";
        
        public static string GetCurrentAssetDirectory()
        {
            foreach (var obj in Selection.GetFiltered<Object>(SelectionMode.Assets))
            {
                var path = AssetDatabase.GetAssetPath(obj);
                if (string.IsNullOrEmpty(path))
                    continue;

                if (System.IO.Directory.Exists(path))
                    return path;
                if (File.Exists(path))
                    return Path.GetDirectoryName(path);
            }

            return "Assets";
        }
        
        /// <summary>
        /// Returns the pool for the given projectile.
        /// If no pool exists, a new one is created
        /// </summary>
        /// <returns></returns>
        public static ProjectilePool GetPool(ProjectileAbilitySO projectileAbility)
        {
            var pools = GameObject.FindObjectsOfType<ProjectilePool>();
            var pool = pools.FirstOrDefault(pool => pool.ProjectileAbilitySo.IsSameAs(projectileAbility));
            if (pool) return pool;
            
            var poolGo = new GameObject($"{projectileAbility.Name}_pool");
            pool = poolGo.AddComponent<ProjectilePool>();
            pool.SetProjectile(projectileAbility);
            return pool;
        }
        
        public static class BackgroundStyle
        {
            private static GUIStyle style = new GUIStyle();
 
            public static GUIStyle Get(Color color)
            {
                if(style is null)
                    style = new GUIStyle();

                var texture = new Texture2D(1, 1);
                texture.SetPixel(0, 0, color);
                texture.Apply();
                style.normal.background = texture;
                return style;
            }
        }

        public static string CreateAndGetAssetPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                path = "Assets";
            if (path[0] == '/')
                path = path.Substring(1);
            if (path.Substring(0, 6).ToLower() != "assets")
                path = path.Insert(0, "Assets/");
            if (!Directory.Exists(Path.GetFullPath(path)))
                Directory.CreateDirectory(path);

            return path;
        }
    }
}