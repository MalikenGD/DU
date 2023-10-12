using System;
using System.IO;
using InfiniteVoid.SpamFramework.Core.Conditions;
using InfiniteVoid.SpamFramework.Editor.Common;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.ScriptableObject;

namespace InfiniteVoid.SpamFramework.Editor.ConditionalEffectsWindow
{
    public class CreateNewConditionalEffectsPage : VisualElement
    {
        public CreateNewConditionalEffectsPage(VisualElement container)
        {
            var getPathFromSelectionButton = container.Q<Button>("get-path-from-selection-button");
            getPathFromSelectionButton.clicked += () =>
            {
                var pathTextField = container.Q<TextField>("new-conditional-effect-path");
                var path = EditorUtils.GetCurrentAssetDirectory();
                pathTextField.value = path;
            };

            var createButton = container.Q<Button>("create-conditional-effect-button");
            createButton.clicked += () => CreateNewConditionalEffect(container);
        }

        private void CreateNewConditionalEffect(VisualElement container)
        {
            try
            {
                var pathInput = container.Q<TextField>("new-conditional-effect-path");
                var path = Common.EditorUtils.CreateAndGetAssetPath(pathInput.text);

                var conditionType = typeof(ConditionalEffectsSO);
                var asset = CreateInstance(conditionType);
                var serializedAsset = new SerializedObject(asset);
                var proposedName = container.Q<TextField>("new-conditional-effect-name").value;
                var assetName = GetAssetName(proposedName);
                serializedAsset.FindProperty("_name").stringValue = assetName;
                serializedAsset.ApplyModifiedProperties();
                
                AssetDatabase.CreateAsset(asset, Path.Combine(path, $"{assetName}.asset"));
                Selection.activeObject = asset;
                ConditionalEffectsWindow.ReloadConditionalEffects();
                ConditionalEffectsWindow.SetSelectionAsSelected();
                
                container.style.display = DisplayStyle.None;

            }
            catch (Exception ex)
            {
                EditorUtility.DisplayDialog("Error",
                    "Error creating new ability. Check console for errors. Common errors are leading with a / (/Assets instead of just Assets) or an invalid path",
                    "Got it.");
                Debug.Log(ex);
            }
        }
        
        private string GetAssetName(string proposedName)
        {
            var assetExists = AssetDatabase.FindAssets($"t:ConditionalEffectsSO {proposedName}").Length > 0;
            if (!assetExists) return proposedName;

            int i = 1;
            while (assetExists)
            {
                assetExists = AssetDatabase.FindAssets($"t:ConditionalEffectsSO {proposedName}_{i}").Length > 0;
                if (assetExists)
                    i++;
            }
            return $"{proposedName}_{i}";
        }
    }
}