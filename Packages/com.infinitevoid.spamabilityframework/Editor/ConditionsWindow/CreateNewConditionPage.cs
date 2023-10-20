using System;
using System.IO;
using InfiniteVoid.SpamFramework.Core.Conditions;
using InfiniteVoid.SpamFramework.Editor.Common;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.ScriptableObject;

namespace InfiniteVoid.SpamFramework.Editor.ConditionsWindow
{
    public class CreateNewConditionPage : VisualElement
    {
        public CreateNewConditionPage(VisualElement parent)
        {
            var getPathFromSelectionButton = parent.parent.Q<Button>("get-path-from-selection-button");
            getPathFromSelectionButton.clicked += () =>
            {
                var pathTextField = parent.parent.Q<TextField>("new-condition-path");
                var path = EditorUtils.GetCurrentAssetDirectory();
                pathTextField.value = path;
            };
            
            var createButton = new Button { text = "Create"};
            createButton.AddToClassList("c-button--blue");
            createButton.AddToClassList("c-new-condition-grid__button");
            createButton.clicked += () => CreateNewCondition();
            parent.Add(createButton);
        }

        private void CreateNewCondition()
        {
            try
            {
                var pathInput = parent.parent.Q<TextField>("new-condition-path");
                var path = Common.EditorUtils.CreateAndGetAssetPath(pathInput.value);

                var conditionType = typeof(AbilityConditionSO);
                var asset = CreateInstance(conditionType);
                var serializedAsset = new SerializedObject(asset);
                var proposedName = parent.parent.Q<TextField>("new-condition-name").value;
                var assetName = GetAssetName(proposedName);
                serializedAsset.FindProperty("_name").stringValue = assetName;
                serializedAsset.ApplyModifiedProperties();
                
                AssetDatabase.CreateAsset(asset, Path.Combine(path, $"{assetName}.asset"));
                Selection.activeObject = asset;
                ConditionsWindow.ReloadConditions();
                ConditionsWindow.SetSelectionAsSelected();
                
                var newEffectPage = parent.parent.Q<Box>("new-condition-page");
                newEffectPage.style.display = DisplayStyle.None;

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
            var assetExists = AssetDatabase.FindAssets($"t:AbilityConditionSO {proposedName}").Length > 0;
            if (!assetExists) return proposedName;

            int i = 1;
            while (assetExists)
            {
                assetExists = AssetDatabase.FindAssets($"t:AbilityConditionSO {proposedName}_{i}").Length > 0;
                if (assetExists)
                    i++;
            }
            return $"{proposedName}_{i}";
        }
    }
}