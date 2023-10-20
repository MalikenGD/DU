using System.Linq;
using InfiniteVoid.SpamFramework.Core.Components.Conditions;
using InfiniteVoid.SpamFramework.Core.Conditions;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;
using static InfiniteVoid.SpamFramework.Editor.Common.CommonUI;
using Debug = UnityEngine.Debug;

namespace InfiniteVoid.SpamFramework.Editor.ConditionsWindow
{
    public class ConditionVFXBox : VisualElement
    {
        private const string CREATE_POOL_BTN_NAME = "create-pool";

        public ConditionVFXBox(SerializedObject serializedCondition, AbilityConditionSO abilityConditionSO)
        {
            var vfxBox = CreateBox("VFX", this, helpUrl: "https://spam.infinitevoid.games/conditions.html#vfx-settings");
            var vfxSettings = new Box();
            var numPooledControlName = "num-pooled-vfx";
            AddPropToElement("_vfx", serializedCondition, vfxBox, evt =>
            {
                var fieldHasValue = evt.changedProperty.objectReferenceValue != null;
                vfxSettings.style.display =
                    fieldHasValue ? DisplayStyle.Flex : DisplayStyle.None;
                var createPoolButton = vfxSettings.Q<Button>(CREATE_POOL_BTN_NAME);
                createPoolButton.style.display = !fieldHasValue || ConditionHasPool(abilityConditionSO)
                    ? DisplayStyle.None
                    : DisplayStyle.Flex;

                if (!fieldHasValue) return;
                var conditionVFX = (ConditionVFX)evt.changedProperty.objectReferenceValue;
                var particleSystem = conditionVFX.GetComponent<ParticleSystem>();
                if (particleSystem.main.stopAction == ParticleSystemStopAction.Disable) return;

                var fixStopActionBox = new Box();
                fixStopActionBox.name = "fix-stop-action";
                vfxSettings.Add(fixStopActionBox);

                fixStopActionBox.Add(CreateHelpBox(
                    "The particles system for this Condition VFX is not set to disable when finished. Condition VFX should in most cases have its particle system's 'Stop Action' set to disabled to work properly when pooled.",
                    "vfx-stop-help", HelpBoxMessageType.Warning));
                CreateCTAButton(fixStopActionBox, "Fix (sets stop action to disabled)", () =>
                {
                    Selection.activeObject = particleSystem;
                    var mainModule = particleSystem.main;
                    mainModule.stopAction = ParticleSystemStopAction.Disable;
                    fixStopActionBox.style.display = DisplayStyle.None;
                });
            });
            AddPropToElement("_numPooled", serializedCondition, vfxSettings, controlName: numPooledControlName);
            AddPropToElement("_spawnAtTargetBase", serializedCondition, vfxSettings);
            AddPropToElement("_spawnOffset", serializedCondition, vfxSettings);
            AddPropToElement("_playOnEvents", serializedCondition, vfxSettings);
            var createPoolButton = CreateCTAButton(vfxSettings, "Create pool", () => CreateNewPool(abilityConditionSO),
                CREATE_POOL_BTN_NAME);
            var vfxProperty = serializedCondition.FindProperty("_vfx");
            createPoolButton.style.display = vfxProperty.objectReferenceValue == null
                                             || ConditionHasPool(abilityConditionSO)
                ? DisplayStyle.None
                : DisplayStyle.Flex;
            
            vfxBox.Add(vfxSettings);
        }

        private bool ConditionHasPool(AbilityConditionSO abilityConditionSO)
        {
            var pools = GameObject.FindObjectsOfType<ConditionVFXPool>();
            return pools.Any(pool => pool.Condition.IsSameAs(abilityConditionSO));
        }

        private void CreateNewPool(AbilityConditionSO abilityConditionSO)
        {
            this.Q<Button>(CREATE_POOL_BTN_NAME).style.display = DisplayStyle.None;
            var go = new GameObject($"{abilityConditionSO.Name} VFX Pool");
            var vfxPool = go.AddComponent<ConditionVFXPool>();
            vfxPool.SetCondition(abilityConditionSO);
            Debug.Log("Condition VFX pool created successfully", go);
        }
    }
}