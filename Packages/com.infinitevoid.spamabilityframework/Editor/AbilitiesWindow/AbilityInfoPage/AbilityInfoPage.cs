using System;
using System.IO;
using InfiniteVoid.SpamFramework.Core.AbilityData;
using InfiniteVoid.SpamFramework.Core.Conditions;
using InfiniteVoid.SpamFramework.Editor.Common;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEngine.ScriptableObject;
using Object = UnityEngine.Object;
using static InfiniteVoid.SpamFramework.Editor.Common.CommonUI;
using Button = UnityEngine.UIElements.Button;
using Image = UnityEngine.UIElements.Image;

namespace InfiniteVoid.SpamFramework.Editor.AbilitiesWindow.AbilityInfoPage
{
    public class AbilityInfoPage : VisualElement
    {
        private AbilityBaseSO _ability;
        public AbilityInfoPage(AbilityBaseSO ability, Box parent, Action refreshListView)
        {
            this._ability = ability;
            var serializedAbility = new SerializedObject(ability);
            var header = CommonUI.CreateHeader(GetHeaderText(ability), "ability-info-header");
            parent.Add(header);
            var assetButtonsRow = new VisualElement();
            assetButtonsRow.AddToClassList("row");
            assetButtonsRow.Add(CreateGoToAssetButton(ability));
            parent.Add(assetButtonsRow);

            var addToGoBox = CreateBox("Add to selected GameObject", parent, helpUrl: "https://spam.infinitevoid.games/add-ability-to-invoker.html");
            addToGoBox.Add(new AddAbilityToSelectedButtons(ability));

            var settingsWrapper = new Box();
            settingsWrapper.AddToClassList("c-ability-settings-wrapper");
            parent.Add(settingsWrapper);
            var abilitySettingsBox = CommonUI.CreateBox("Ability settings", settingsWrapper, true, helpUrl: "https://spam.infinitevoid.games/general-settings.html");


            var unknownIcon = AssetDatabase.LoadAssetAtPath<Texture>($"{EditorUtils.PACKAGE_BASE_PATH}/Editor/Textures/unknown.png");
            var image = new Image();
            image.AddToClassList("ability-image");
            abilitySettingsBox.Add(image);
            if (ability.Icon)
                image.image = ability.Icon.texture;
            else image.image = unknownIcon;
            
            var propertiesWrapper = new VisualElement();
            propertiesWrapper.AddToClassList("c-abilities-props");
            var idField = new IntegerField("Internal Id");
            idField.value = ability.Id;
            idField.SetEnabled(false);
            propertiesWrapper.Add(idField);
            AddPropToElement("_name", serializedAbility, propertiesWrapper, _ =>
            {
                refreshListView();
                header.text = GetHeaderText(ability);
            });
            var descriptionField = new TextField { multiline = true, bindingPath = "_description", label = "Description" };
            descriptionField.AddToClassList("c-multiline-input");
            descriptionField.Bind(serializedAbility);
            propertiesWrapper.Add(descriptionField);
            AddPropToElement("_icon", serializedAbility, propertiesWrapper, evt =>
            {
                var sprite = evt.changedProperty.objectReferenceValue as Sprite;
                image.image = sprite == null ? unknownIcon : sprite.texture;
                refreshListView();
            });
            AddPropToElement("_abilityCooldown", serializedAbility, propertiesWrapper);
            AddPropToElement("_cooldownType", serializedAbility, propertiesWrapper,
                tooltip: "Automatic uses Time.deltaTime, Manual ticks when Ability.TickCooldown() is called");
            if (!(ability is RaycastAbilitySO))
            {
                AddPropToElement("_castRange", serializedAbility, propertiesWrapper);
            }
            else
            {
                AddPropToElement("_raycastLength", serializedAbility, propertiesWrapper);
                AddPropToElement("_raycastLayers", serializedAbility, propertiesWrapper);
            }

            AddPropToElement("_requiresAbilityTarget", serializedAbility, propertiesWrapper, evt =>
            {
                var distanceCheckWrapper = settingsWrapper.Q<VisualElement>("cast-on-ground-wrapper");
                if (distanceCheckWrapper == null) return;
                distanceCheckWrapper.style.display =
                    evt.changedProperty.boolValue ? DisplayStyle.None : DisplayStyle.Flex;
            });
            
            AddPropToElement("_telegraph", serializedAbility, propertiesWrapper);
            if (ability is TargetedAbilitySO)
            {
                AddPropToElement("_castOnSelf", serializedAbility, propertiesWrapper);
            }
            else if (ability is ProjectileAbilitySO projectileAbilitySo)
            {
                settingsWrapper.Add(new ProjectileSettingsBox(settingsWrapper, serializedAbility, projectileAbilitySo));
            }
            AddPropToElement("_customSettings", serializedAbility, propertiesWrapper);
            abilitySettingsBox.Add(propertiesWrapper);
            
            CreateEffectsBox(ability, serializedAbility, parent);
            AddReferencesRow(serializedAbility, parent);
        }

        private Button CreateGoToAssetButton(AbilityBaseSO ability)
        {
            var goToAssetButton = new Button(() => { Selection.activeObject = ability; });
            goToAssetButton.text = "Select asset in project";
            goToAssetButton.AddToClassList("u-margin-bottom");
            return goToAssetButton;
        }

        private Button CreateRenameAssetButton(AbilityBaseSO ability)
        {
            var renameAssetButton = new Button(() =>
            {
                // ability.name = ability.Name;
            });
            renameAssetButton.text = "Rename asset(s) to ability name";
            renameAssetButton.AddToClassList("u-margin-bottom");
            return renameAssetButton;
        }

        private void CreateEffectsBox(AbilityBaseSO ability, SerializedObject serializedAbility, Box abilityInfoBox)
        {
            var effectsBox = CommonUI.CreateBox("Effects", abilityInfoBox, helpUrl:"https://spam.infinitevoid.games/ability-effects.html");
            effectsBox.AddToClassList("c-effects-box");
            effectsBox.Add(CreateHeader2("Main effects"));
            effectsBox.Add(new EffectAndTimeList.EffectAndTimeList(ability, serializedAbility));
            effectsBox.Add(CreateHeader2("Pre-conditions", "u-margin-top"));
            effectsBox.Add(CreateHelpBox("Conditions that needs to be satisfied for this ability's main- and conditional effects to be applied.", "preconditions-help"));
            var preconditionsLabel = new Label() { name = "preconditions-info" };
            preconditionsLabel.AddToClassList("u-italic");
            preconditionsLabel.AddToClassList("u-margin-top--half");
            effectsBox.Add(preconditionsLabel);
            UpdatePreconditionsLabel(ability, preconditionsLabel);
            var preconditionList = new PreconditionsList.PreconditionsList(ability, serializedAbility);
            preconditionList.PreconditionChanged += () => UpdatePreconditionsLabel(ability, preconditionsLabel);
            preconditionList.PreconditionRemoved += () => UpdatePreconditionsLabel(ability, preconditionsLabel);
            effectsBox.Add(preconditionList);
            effectsBox.Add(new ConditionalEffectsBox.ConditionalEffectsBox(ability, serializedAbility));
            effectsBox.Add(new AoeSettingsBox(ability, serializedAbility, effectsBox));
        }

        private void UpdatePreconditionsLabel(AbilityBaseSO ability, Label preconditionsLabel)
        {
            preconditionsLabel.style.display = ability.PreConditions.Count <= 0 ? DisplayStyle.None : DisplayStyle.Flex;
            preconditionsLabel.text = $"Effects will only apply if {ConditionConstraint.Print(ability.PreConditions)}";
        }

        /// <summary>
        /// Adds Animations and VFX boxen to a separate row
        /// </summary>
        /// <param name="serializedAbility"></param>
        /// <param name="abilityInfoBox"></param>
        private void AddReferencesRow(SerializedObject serializedAbility, Box abilityInfoBox)
        {
            var referencesRow = new Box();
            referencesRow.AddToClassList("c-references-wrapper");
            referencesRow.AddToClassList("row");

            var prop = serializedAbility.FindProperty("_animationTimings");
            referencesRow.Add(new AnimationsBox(serializedAbility, prop));

            prop = serializedAbility.FindProperty("_abilityVfx");
            referencesRow.Add(new VfxBox(_ability, serializedAbility, prop));

            prop = serializedAbility.FindProperty("_abilitySfx");
            var sfxBox = CommonUI.CreateBox("SFX", referencesRow, helpUrl: "https://spam.infinitevoid.games/sfx-settings.html");
            sfxBox.Add(
                CreateObjectBox(serializedAbility,
                    prop,
                    "c-vfx-box",
                    "Create SFX", () =>
                    {
                        var abilitySfxAsset = CreateInstance<AbilitySFXSO>();
                        var path = AssetDatabase.GetAssetPath(serializedAbility.targetObject);
                        var directory = Path.GetDirectoryName(path);
                        AssetDatabase.CreateAsset(abilitySfxAsset,
                            Path.Combine(directory,
                                serializedAbility.FindProperty("_name").stringValue + "_sfx.asset"));
                        var animProp = serializedAbility.FindProperty("_abilitySfx");
                        animProp.objectReferenceValue = abilitySfxAsset;
                        serializedAbility.ApplyModifiedProperties();
                        Selection.activeObject = abilitySfxAsset;
                    }));
            abilityInfoBox.Add(referencesRow);
        }

        private Box CreateObjectBox(SerializedObject serializedAbility,
            SerializedProperty serializedProperty,
            string className,
            string emptyReferenceButtonText,
            Action onNewReferenceClick)
        {
            Box parentBox = new Box();
            parentBox.AddToClassList(className);
            if (serializedProperty.objectReferenceValue == null)
            {
                CreateEmptyReferenceBox(parentBox, emptyReferenceButtonText, onNewReferenceClick);
                var prop = new PropertyField(serializedProperty);
                prop.Bind(serializedAbility);
                prop.RegisterValueChangeCallback(evt =>
                {
                    parentBox.Clear();
                    if (evt.changedProperty.objectReferenceValue == null)
                    {
                        CreateEmptyReferenceBox(parentBox, emptyReferenceButtonText, onNewReferenceClick);
                        parentBox.Add(prop);
                        return;
                    }

                    CreateReferenceBox(evt.changedProperty.objectReferenceValue, parentBox,
                        prop);
                });

                parentBox.Add(prop);
            }
            else
            {
                var prop = new PropertyField(serializedProperty);
                prop.Bind(serializedAbility);
                prop.RegisterValueChangeCallback(evt =>
                {
                    CreateReferenceBox(evt.changedProperty.objectReferenceValue, parentBox,
                        prop);
                });
                CreateReferenceBox(serializedProperty.objectReferenceValue, parentBox, prop);
            }

            return parentBox;
        }


        private void CreateEmptyReferenceBox(Box parentBox, string buttonText,
            Action onNewReferenceClick)
        {
            var button = new Button();
            button.text = buttonText;
            button.clicked += onNewReferenceClick;
            parentBox.Add(button);
        }

        private void CreateReferenceBox(Object objectReference, Box parentBox, PropertyField prop)
        {
            parentBox.Clear();
            parentBox.Add(prop);

            var serializedObject = new SerializedObject(objectReference);
            SerializedProperty objectProps = serializedObject.GetIterator();
            objectProps.Next(true);
            while (objectProps.NextVisible(false))
            {
                if (objectProps.name == "m_Script")
                    continue;

                var propField = new PropertyField(objectProps);

                if (objectProps.name == "_onHitSfx")
                {
                    // Special case since we style PropertyField with a greater height and this breaks
                    // lists. On Hit SFX is as array.
                    propField.AddToClassList("u-list-property");
                }

                propField.Bind(serializedObject);
                parentBox.Add(propField);
            }
        }

        private string GetNameFromAbilityType(string abilityType)
        {
            switch (abilityType)
            {
                case "ProjectileAbilitySO":
                    return "Projectile";
                case "TargetedAbilitySO":
                    return "Targeted ability";
                case "DirectionalAbilitySO":
                    return "Directional ability";
                case "RaycastAbilitySO":
                    return "Raycast ability";
                default:
                    return "Unknown";
            }
        }

        private string GetHeaderText(AbilityBaseSO ability)
        {
            return $"{ability.Name} | {GetNameFromAbilityType(ability.GetType().Name)}";
        }
    }
}