using System;
using InfiniteVoid.SpamFramework.Core.Common.Pooling;
using UnityEditor;
using UnityEngine;

namespace InfiniteVoid.SpamFramework.Editor.ObjectEditors
{
    [CustomEditor(typeof(ProjectilePool), true)]
    public class ProjectilePoolEditor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();
            var component = (ProjectilePool)target;

            if (!component.ProjectileAbilitySo) return;
            SpamPoolManager.RegisterPool(component);
            
            EditorGUILayout.Separator();
            ShowPoolHandlingButtons(component);

            EditorGUILayout.Separator();
            if (GUILayout.Button("Open pooling window"))
            {
                PoolingWindow.PoolingWindow.ShowWindow();
            }
        }

        private static void ShowPoolHandlingButtons(ProjectilePool component)
        {
            if (UnityEditor.EditorApplication.isPlaying) return;
            var numProjectilesToPool = SpamPoolManager.GetNumProjectiles(component.ProjectileAbilitySo);
            var currentPooledProjectiles = component.GetProjectileCount();
            
            if (0 < currentPooledProjectiles)
                if (GUILayout.Button("Clear pool"))
                    component.ClearPool();

            if (currentPooledProjectiles == 0)
                if (GUILayout.Button("Prefill pool"))
                    component.RefillPool();

            if (0 < currentPooledProjectiles && currentPooledProjectiles != numProjectilesToPool)
                if (GUILayout.Button("Refresh pool"))
                    component.RefillPool();
        }
    }
}