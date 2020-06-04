using System.Reflection;
using UnityEditor;
using UnityEngine;
using UnityEngine.Events;
using InternalReorderableList = UnityEditorInternal.ReorderableList;
using InternalUnityEventDrawer = UnityEditorInternal.UnityEventDrawer;

namespace Coimbra
{
    /// <summary>
    /// Custom implementation of UnityEventDrawer.
    /// </summary>
    [CustomPropertyDrawer(typeof(UnityEventAttribute))]
    public class UnityEventDrawer : InternalUnityEventDrawer
    {
        protected const float BackgroundSpacing = 6;
        protected const float ContentSpacing = 10;
        protected const float HeaderBodySpacing = 4;
        protected const float HeaderHeight = 16;

        private const string Calls = "m_Calls";
        private const string PersistentCalls = "m_PersistentCalls";

        private static readonly FieldInfo DraggableField = typeof(InternalReorderableList).GetField("m_Draggable", BindingFlags.NonPublic | BindingFlags.Instance);
        private static readonly FieldInfo LastSelectedIndexField = typeof(InternalUnityEventDrawer).GetField("m_LastSelectedIndex", BindingFlags.NonPublic | BindingFlags.Instance);
        private static readonly FieldInfo PropField = typeof(InternalUnityEventDrawer).GetField("m_Prop", BindingFlags.NonPublic | BindingFlags.Instance);
        private static readonly FieldInfo ReorderableListField = typeof(State).GetField("m_ReorderableList", BindingFlags.NonPublic | BindingFlags.Instance);
        private static readonly FieldInfo TextField = typeof(InternalUnityEventDrawer).GetField("m_Text", BindingFlags.NonPublic | BindingFlags.Instance);
        private static readonly GUIStyle BackgroundStyle = new GUIStyle("RL Background");
        private static readonly GUIStyle HeaderStyle = new GUIStyle("RL Header");
        private static readonly MethodInfo RestoreStateMethod = typeof(InternalUnityEventDrawer).GetMethod("RestoreState", BindingFlags.NonPublic | BindingFlags.Instance);
        private static readonly MethodInfo GetDummyEventMethod = typeof(InternalUnityEventDrawer).GetMethod("GetDummyEvent", BindingFlags.NonPublic | BindingFlags.Static);
        private static readonly MethodInfo GetEventParamsMethod = typeof(InternalUnityEventDrawer).GetMethod("GetEventParams", BindingFlags.NonPublic | BindingFlags.Static);
        private static readonly MethodInfo RepaintAllInspectorsMethod = typeof(Editor).Assembly.GetType("UnityEditor.InspectorWindow").GetMethod("RepaintAllInspectors", BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic);

        private bool _wasInitialized;

        protected bool CanCollapse => HasPermission(UnityEventPermissions.Collapse);
        protected bool CanDrag => HasPermission(UnityEventPermissions.Drag);
        protected bool CanRead => HasPermission(UnityEventPermissions.Read);
        protected UnityEventPermissions EditModePermissions { get; set; }
        protected UnityEventPermissions PlayModePermissions { get; set; }

        private int LastSelectedIndex => (int)LastSelectedIndexField.GetValue(this);
        private string Text { get => (string)TextField.GetValue(this); set => TextField.SetValue(this, value); }
        private SerializedProperty Property { get => (SerializedProperty)PropField.GetValue(this); set => PropField.SetValue(this, value); }

        public sealed override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (attribute != null)
            {
                ProcessAttribute();
            }

            if (CanRead == false)
            {
                return;
            }

            Property = property;
            Text = label.text;

            if (CanCollapse && property.isExpanded == false)
            {
                using (new EditorGUI.IndentLevelScope(-EditorGUI.indentLevel))
                {
                    DrawEventHeader(position);
                }
            }
            else
            {
                property.isExpanded = true;
                State state = RestoreState(property);
                InternalReorderableList reorderableList = GetReorderableList(state);
                float headerBodyHeight = GetHeaderBodyHeight(property);

                if (headerBodyHeight > 0)
                {
                    float headerHeight = HeaderHeight + headerBodyHeight + HeaderBodySpacing;

                    if (Mathf.Approximately(reorderableList.headerHeight, headerHeight) == false)
                    {
                        reorderableList.headerHeight = headerHeight;
                        RepaintAllInspectors();
                    }
                }

                SetDraggable(reorderableList, CanDrag);
                OnGUI(position);
                state.lastSelectedIndex = LastSelectedIndex;
            }
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (CanRead == false)
            {
                return 0;
            }

            if (CanCollapse && property.isExpanded == false)
            {
                return HeaderHeight + EditorGUIUtility.standardVerticalSpacing;
            }

            property.isExpanded = true;

            return base.GetPropertyHeight(property, label);
        }

        protected sealed override void DrawEventHeader(Rect headerRect)
        {
            SerializedProperty property = Property;

            if (property.isExpanded == false)
            {
                if (Event.current.type == EventType.Repaint)
                {
                    HeaderStyle.Draw(headerRect, false, false, false, false);
                }

                headerRect.xMin += BackgroundSpacing;
                headerRect.xMax -= BackgroundSpacing;
                headerRect.y += 1;
            }

            headerRect.x += ContentSpacing;
            headerRect.width -= ContentSpacing;
            headerRect.height = HeaderHeight;

            DrawHeaderOverlay(headerRect, property);

            SerializedProperty calls = property.FindPropertyRelative(PersistentCalls).FindPropertyRelative(Calls);
            string count = $" [{(calls.hasMultipleDifferentValues ? "-" : calls.arraySize.ToString())}]";
            string name = Text;
            string text = $"{(string.IsNullOrEmpty(name) == false ? name : "Event")}{GetEventParams(property)}{count}";

            if (CanCollapse)
            {
                using (var scope = new EditorGUI.ChangeCheckScope())
                {
                    bool isExpanded = EditorGUI.Foldout(headerRect, property.isExpanded, text, true);

                    if (scope.changed)
                    {
                        property.isExpanded = isExpanded;
                    }
                }
            }
            else
            {
                EditorGUI.LabelField(headerRect, text);
            }

            if (property.isExpanded && GetHeaderBodyHeight(property) > 0)
            {
                float headerBodyHeight = GetHeaderBodyHeight(property);

                if (headerBodyHeight > 0)
                {
                    headerRect.y += HeaderHeight;
                    headerRect.height = headerBodyHeight + HeaderBodySpacing;

                    if (Event.current.type == EventType.Repaint)
                    {
                        headerRect.x -= ContentSpacing;
                        headerRect.width += ContentSpacing;
                        headerRect.xMin -= BackgroundSpacing;
                        headerRect.xMax += BackgroundSpacing;

                        BackgroundStyle.Draw(headerRect, false, false, false, false);

                        headerRect.xMin += BackgroundSpacing;
                        headerRect.xMax -= BackgroundSpacing;
                    }

                    DrawHeaderBody(headerRect, property);
                }
            }
        }

        protected virtual void DrawHeaderBody(Rect position, SerializedProperty property) { }

        protected virtual void DrawHeaderOverlay(Rect position, SerializedProperty property) { }

        protected virtual void ProcessAttribute()
        {
            var unityEventAttribute = (UnityEventAttribute)attribute;
            EditModePermissions = unityEventAttribute.EditModePermissions;
            PlayModePermissions = unityEventAttribute.PlayModePermissions;
        }

        protected virtual float GetHeaderBodyHeight(SerializedProperty property)
        {
            return 0;
        }

        private void RepaintAllInspectors()
        {
            RepaintAllInspectorsMethod.Invoke(null, null);
        }

        private void SetDraggable(InternalReorderableList reorderableList, bool isDraggable)
        {
            DraggableField.SetValue(reorderableList, isDraggable);
        }

        private bool HasPermission(UnityEventPermissions value)
        {
            return ((EditorApplication.isPlaying ? PlayModePermissions : EditModePermissions) & value) == value;
        }

        private string GetEventParams(SerializedProperty property)
        {
            return (string)GetEventParamsMethod.Invoke(null, new object[] { GetDummyEvent(property) });
        }

        private InternalReorderableList GetReorderableList(State state)
        {
            return (InternalReorderableList)ReorderableListField.GetValue(state);
        }

        private State RestoreState(SerializedProperty property)
        {
            return (State)RestoreStateMethod.Invoke(this, new object[] { property });
        }

        private UnityEventBase GetDummyEvent(SerializedProperty property)
        {
            return (UnityEventBase)GetDummyEventMethod.Invoke(null, new object[] { property });
        }
    }
}
