using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace Coimbra
{
    [CustomPropertyDrawer(typeof(AnimatorParameterAttribute))]
    public sealed class AnimatorParameterDrawer : PropertyAttributeDrawer<AnimatorParameterAttribute>
    {
        private const BindingFlags SerializedFieldFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
        private const string NoParameterFoundError = "No {0} parameter found in Animator Controller.";

        private static readonly string[] OverrideControllerError = { "Using an Animator Override Controller is not supported." };
        private static readonly string[] MultiEditErrorError = { "Cannot multi-edit different controllers." };
        private static readonly List<int> Options = new List<int>();
        private static readonly List<object> Targets = new List<object>();
        private static readonly List<GUIContent> Contents = new List<GUIContent>();
        private static readonly Dictionary<AnimatorControllerParameterType, string[]> NoParameterFoundErrorDictionary = new Dictionary<AnimatorControllerParameterType, string[]>
        {
            [AnimatorControllerParameterType.Float] = new[] { string.Format(NoParameterFoundError, "float") },
            [AnimatorControllerParameterType.Int] = new[] { string.Format(NoParameterFoundError, "int") },
            [AnimatorControllerParameterType.Bool] = new[] { string.Format(NoParameterFoundError, "bool") },
            [AnimatorControllerParameterType.Trigger] = new[] { string.Format(NoParameterFoundError, "trigger") }
        };

        protected override void DrawGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (property.propertyType != SerializedPropertyType.Integer && property.propertyType != SerializedPropertyType.String)
            {
                EditorGUI.LabelField(position, label.text, "Use AnimatorParameter with int or string.");

                return;
            }

            Targets.Clear();
            property.GetParentObjects(Targets);

            FieldInfo animatorFieldInfo = null;

            if (string.IsNullOrEmpty(Attribute.AnimatorField))
            {
                foreach (FieldInfo field in Targets[0].GetType().GetFields(SerializedFieldFlags))
                {
                    if (field.FieldType == typeof(Animator))
                    {
                        animatorFieldInfo = field;

                        break;
                    }
                }

                if (animatorFieldInfo == null)
                {
                    EditorGUI.LabelField(position, label.text, "No field of type Animator was found.");

                    return;
                }
            }
            else
            {
                animatorFieldInfo = Targets[0].GetType().GetField(Attribute.AnimatorField, SerializedFieldFlags);

                if (animatorFieldInfo == null)
                {
                    EditorGUI.LabelField(position, label.text, "Animator field is invalid.");

                    return;
                }

                if (animatorFieldInfo.FieldType != typeof(Animator))
                {
                    EditorGUI.LabelField(position, label.text, "Animator field is not of type Animator.");

                    return;
                }
            }

            var animator = (Animator)animatorFieldInfo.GetValue(Targets[0]);

            if (animator == null)
            {
                if (HasDifferentControllers(animatorFieldInfo, Targets, a => a != null))
                {
                    ShowDifferentControllersPopup(position, label.text);

                    return;
                }

                EditorGUI.LabelField(position, label.text, "Animator is null.");

                return;
            }

            bool IsNullOrDifferent(Animator otherAnimator)
            {
                return otherAnimator == null || otherAnimator.runtimeAnimatorController != animator.runtimeAnimatorController;
            }

            if (HasDifferentControllers(animatorFieldInfo, Targets, IsNullOrDifferent))
            {
                ShowDifferentControllersPopup(position, label.text);

                return;
            }

            if (animator.runtimeAnimatorController == null)
            {
                EditorGUI.LabelField(position, label.text, "Animator Controller is null.");

                return;
            }

            var animatorController = animator.runtimeAnimatorController as AnimatorController;

            if (animatorController == null)
            {
                using (new EditorGUI.DisabledScope(true))
                {
                    EditorGUI.Popup(position, label.text, 0, OverrideControllerError);
                }

                return;
            }

            AnimatorControllerParameter[] parameters = animatorController.parameters;

            if (parameters.Length == 0)
            {
                ShowInvalidParameterTypePopup(position, label.text);

                return;
            }

            if (property.propertyType == SerializedPropertyType.String)
            {
                DrawStringPopup(position, property, label, parameters);
            }
            else
            {
                DrawIntPopup(position, property, label, parameters);
            }
        }

        protected override float GetHeight(SerializedProperty property, GUIContent label)
        {
            return EditorGUIUtility.singleLineHeight;
        }

        private void DrawIntPopup(Rect position, SerializedProperty property, GUIContent label, AnimatorControllerParameter[] parameters)
        {
            int index = 0;
            Contents.Clear();
            Options.Clear();

            for (int i = 0; i < parameters.Length; i++)
            {
                AnimatorControllerParameter parameter = parameters[i];

                if (parameter.type != Attribute.ParameterType)
                {
                    continue;
                }

                int hash = Animator.StringToHash(parameter.name);

                if (hash == property.intValue)
                {
                    index = Contents.Count;
                }

                Contents.Add(new GUIContent(parameter.name));
                Options.Add(hash);
            }

            if (Contents.Count == 0)
            {
                ShowInvalidParameterTypePopup(position, label.text);

                return;
            }

            if (property.hasMultipleDifferentValues)
            {
                index = -1;
            }

            int SetValue(int value)
            {
                for (int i = 0; i < Options.Count; i++)
                {
                    if (Options[i] == value)
                    {
                        return value;
                    }
                }

                return Options[0];
            }

            property.SetValues<int>(SetValue);

            using (var propertyScope = new EditorGUI.PropertyScope(position, label, property))
            {
                using (var changeCheckScope = new EditorGUI.ChangeCheckScope())
                {
                    int value = EditorGUI.Popup(position, propertyScope.content, index, Contents.ToArray());

                    if (changeCheckScope.changed)
                    {
                        property.intValue = Options[value];
                    }
                }
            }
        }

        private void DrawStringPopup(Rect position, SerializedProperty property, GUIContent label, AnimatorControllerParameter[] parameters)
        {
            int index = 0;
            Contents.Clear();

            for (int i = 0; i < parameters.Length; i++)
            {
                AnimatorControllerParameter parameter = parameters[i];

                if (parameter.type != Attribute.ParameterType)
                {
                    continue;
                }

                if (parameter.name == property.stringValue)
                {
                    index = Contents.Count;
                }

                Contents.Add(new GUIContent(parameter.name));
            }

            if (Contents.Count == 0)
            {
                ShowInvalidParameterTypePopup(position, label.text);

                return;
            }

            if (property.hasMultipleDifferentValues)
            {
                index = -1;
            }

            string SetValue(string value)
            {
                if (string.IsNullOrEmpty(value))
                {
                    return Contents[0].text;
                }

                for (int i = 0; i < Contents.Count; i++)
                {
                    if (Contents[i].text == value)
                    {
                        return value;
                    }
                }

                return Contents[0].text;
            }

            property.SetValues<string>(SetValue);

            using (var propertyScope = new EditorGUI.PropertyScope(position, label, property))
            {
                using (var changeCheckScope = new EditorGUI.ChangeCheckScope())
                {
                    int value = EditorGUI.Popup(position, propertyScope.content, index, Contents.ToArray());

                    if (changeCheckScope.changed)
                    {
                        property.stringValue = Contents[value].text;
                    }
                }
            }
        }

        private void ShowDifferentControllersPopup(Rect position, string text)
        {
            using (new EditorGUI.DisabledScope(true))
            {
                EditorGUI.Popup(position, text, 0, MultiEditErrorError);
            }
        }

        private void ShowInvalidParameterTypePopup(Rect position, string text)
        {
            using (new EditorGUI.DisabledScope(true))
            {
                EditorGUI.Popup(position, text, 0, NoParameterFoundErrorDictionary[Attribute.ParameterType]);
            }
        }

        private static bool HasDifferentControllers(FieldInfo fieldInfo, List<object> targets, System.Predicate<Animator> condition)
        {
            if (targets.Count > 1)
            {
                for (int i = 1; i < targets.Count; i++)
                {
                    object o = fieldInfo.GetValue(targets[i]);

                    if (condition.Invoke(o as Animator))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }
}
