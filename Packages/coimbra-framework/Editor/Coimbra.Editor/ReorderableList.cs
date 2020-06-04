using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Coimbra
{
    public sealed class ReorderableList
    {
        private struct DragElement
        {
            public SerializedProperty Property;
            public int StartIndex;
            public float DragOffset;
            public bool Selected;
            public Rect Rect;
            public Rect DesiredRect;

            private bool _isExpanded;
            private Dictionary<int, bool> _states;

            internal void RecordState()
            {
                _states = new Dictionary<int, bool>();
                _isExpanded = Property.isExpanded;
                Iterate(this, Property, (e, p, index) => { e._states[index] = p.isExpanded; });
            }

            internal void RestoreState(SerializedProperty property)
            {
                property.isExpanded = _isExpanded;
                Iterate(this, property, (e, p, index) => { p.isExpanded = e._states[index]; });
            }

            internal bool Overlaps(Rect value, int index, int direction)
            {
                if (direction < 0 && index < StartIndex)
                {

                    return DesiredRect.yMin < value.center.y;
                }

                if (direction > 0 && index > StartIndex)
                {

                    return DesiredRect.yMax > value.center.y;
                }

                return false;
            }

            private static void Iterate(DragElement element, SerializedProperty property, System.Action<DragElement, SerializedProperty, int> action)
            {
                SerializedProperty copy = property.Copy();
                SerializedProperty end = copy.GetEndProperty();
                int index = 0;

                while (copy.NextVisible(true) && !SerializedProperty.EqualContents(copy, end))
                {
                    if (copy.hasVisibleChildren)
                    {
                        action(element, copy, index);
                        index++;
                    }
                }
            }
        }

        private static class Internals
        {
            private static readonly MethodInfo DragAndDropValidation = System.Type.GetType("UnityEditor.EditorGUI, UnityEditor").GetMethod("ValidateObjectFieldAssignment", BindingFlags.NonPublic | BindingFlags.Static);

            private static MethodInfo _appendDragDrop;
            private static object[] _dragDropValidationParams;
            private static object[] _appendDragDropParams;

            static Internals()
            {
                _dragDropValidationParams = new object[4];
                _appendDragDrop = typeof(SerializedProperty).GetMethod("AppendFoldoutPPtrValue", BindingFlags.NonPublic | BindingFlags.Instance);
                _appendDragDropParams = new object[1];
            }

            internal static Object ValidateObjectDragAndDrop(Object[] references, SerializedProperty property)
            {
                _dragDropValidationParams[0] = references;
                _dragDropValidationParams[1] = null;
                _dragDropValidationParams[2] = property;
                _dragDropValidationParams[3] = 0;

                return DragAndDropValidation.Invoke(null, _dragDropValidationParams) as Object;
            }

            internal static void AppendDragAndDropValue(Object obj, SerializedProperty list)
            {
                _appendDragDropParams[0] = obj;
                _appendDragDrop.Invoke(list, _appendDragDropParams);
            }
        }

        private static class Style
        {
            public static readonly GUIContent CollapseButton = EditorGUIUtility.IconContent("winbtn_win_min");
            public static readonly GUIContent ExpandButton = EditorGUIUtility.IconContent("winbtn_win_max");
            public static readonly GUIContent IconToolbarMinus = EditorGUIUtility.IconContent("Toolbar Minus", "Remove selection from list");
            public static readonly GUIContent IconToolbarPlus = EditorGUIUtility.IconContent("Toolbar Plus", "Add to list");
            public static readonly GUIContent IconToolbarPlusMore = EditorGUIUtility.IconContent("Toolbar Plus More", "Choose to add to list");
            public static readonly GUIStyle BoxBackground = new GUIStyle("RL Background") { border = new RectOffset(6, 3, 3, 6) };
            public static readonly GUIStyle DraggingHandle = new GUIStyle("RL DragHandle");
            public static readonly GUIStyle ElementBackground = new GUIStyle("RL Element") { border = new RectOffset(2, 3, 2, 3) };
            public static readonly GUIStyle FooterBackground = new GUIStyle("RL Footer");
            public static readonly GUIStyle HeaderBackground = new GUIStyle("RL Header");
            public static readonly GUIStyle PreButton = new GUIStyle("RL FooterButton");
            public static readonly GUIStyle VerticalLabel = new GUIStyle(EditorStyles.label) { alignment = TextAnchor.MiddleLeft, contentOffset = new Vector2(10, -3) };
        }

        private sealed class InvalidListException : System.InvalidOperationException
        {
            public InvalidListException() : base("ReorderableList serializedProperty must be an array!") { }
        }

        private sealed class ListSelection : IEnumerable<int>
        {
            private int? _firstSelected;
            private List<int> _indexes;

            public int this[int index]
            {
                get { return _indexes[index]; }
                set
                {
                    int oldIndex = _indexes[index];

                    _indexes[index] = value;

                    if (oldIndex == _firstSelected)
                    {

                        _firstSelected = value;
                    }
                }
            }

            public int First => _indexes.Count > 0 ? _indexes[0] : -1;
            public int Last => _indexes.Count > 0 ? _indexes[_indexes.Count - 1] : -1;
            public int Length => _indexes.Count;

            public ListSelection()
            {
                _indexes = new List<int>();
            }

            public ListSelection(IEnumerable<int> indexes)
            {
                _indexes = new List<int>(indexes);
            }

            public void AppendWithAction(int index, Event e)
            {
                if (EditorGUI.actionKey)
                {
                    if (Contains(index))
                    {
                        Remove(index);
                    }
                    else
                    {
                        Append(index);
                        _firstSelected = index;
                    }
                }
                else if (e.shift && _indexes.Count > 0 && _firstSelected.HasValue)
                {
                    _indexes.Clear();

                    AppendRange(_firstSelected.Value, index);
                }
                else if (!Contains(index))
                {
                    Select(index);
                }
            }

            public bool CanRevert(SerializedProperty list)
            {
                if (list.serializedObject.targetObjects.Length == 1)
                {
                    for (int i = 0; i < Length; i++)
                    {
                        if (list.GetArrayElementAtIndex(this[i]).isInstantiatedPrefab)
                        {
                            return true;
                        }
                    }
                }

                return false;
            }

            public bool Contains(int index)
            {
                return _indexes.Contains(index);
            }

            public void Clear()
            {
                _indexes.Clear();
                _firstSelected = null;
            }

            public void Delete(SerializedProperty list)
            {
                Sort();

                int i = Length;

                while (--i > -1)
                {
                    list.GetArrayElementAtIndex(this[i]).DeleteCommand();
                }

                Clear();

                list.serializedObject.ApplyModifiedProperties();
                list.serializedObject.Update();

                HandleUtility.Repaint();
            }

            public void Duplicate(SerializedProperty list)
            {
                int offset = 0;

                for (int i = 0; i < Length; i++)
                {
                    this[i] += offset;

                    list.GetArrayElementAtIndex(this[i]).DuplicateCommand();
                    list.serializedObject.ApplyModifiedProperties();
                    list.serializedObject.Update();

                    offset++;
                }

                HandleUtility.Repaint();
            }

            public void SelectWhenNoAction(int index, Event e)
            {
                if (!EditorGUI.actionKey && !e.shift)
                {
                    Select(index);
                }
            }

            public void Select(int index)
            {
                _indexes.Clear();
                _indexes.Add(index);
                _firstSelected = index;
            }

            public void Remove(int index)
            {
                if (_indexes.Contains(index))
                {
                    _indexes.Remove(index);
                }
            }

            public void RevertValues(object userData)
            {
                var list = (SerializedProperty)userData;

                for (int i = 0; i < Length; i++)
                {
                    SerializedProperty property = list.GetArrayElementAtIndex(this[i]);

                    if (property.isInstantiatedPrefab)
                    {
                        property.prefabOverride = false;
                    }
                }

                list.serializedObject.ApplyModifiedProperties();
                list.serializedObject.Update();

                HandleUtility.Repaint();
            }

            public void Sort()
            {
                if (_indexes.Count > 0)
                {
                    _indexes.Sort();
                }
            }

            public void Sort(System.Comparison<int> comparison)
            {
                if (_indexes.Count > 0)
                {
                    _indexes.Sort(comparison);
                }
            }

            public int[] ToArray()
            {
                return _indexes.ToArray();
            }

            public ListSelection Clone()
            {
                return new ListSelection(ToArray()) { _firstSelected = _firstSelected };
            }

            private void Append(int index)
            {
                if (index >= 0 && !_indexes.Contains(index))
                {
                    _indexes.Add(index);
                }
            }

            private void AppendRange(int from, int to)
            {
                int sign = (int)Mathf.Sign(to - from);

                if (sign != 0)
                {
                    for (int i = from; i != to; i += sign)
                    {
                        Append(i);
                    }
                }

                Append(to);
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return ((IEnumerable<int>)_indexes).GetEnumerator();
            }

            IEnumerator<int> IEnumerable<int>.GetEnumerator()
            {
                return ((IEnumerable<int>)_indexes).GetEnumerator();
            }
        }

        private sealed class MissingListException : System.ArgumentNullException
        {
            public MissingListException(string paramName) : base(paramName) { }
        }

        private sealed class SlideGroup
        {
            private Dictionary<int, Rect> _ids;

            public SlideGroup()
            {
                _ids = new Dictionary<int, Rect>();
            }

            public Rect GetRect(int id, Rect rect, float easing)
            {
                if (Event.current.type != EventType.Repaint)
                {
                    return rect;
                }

                if (!_ids.ContainsKey(id))
                {
                    _ids.Add(id, rect);

                    return rect;
                }

                Rect newRect = _ids[id];

                if (Mathf.Abs(newRect.y - rect.y) > Mathf.Epsilon)
                {
                    float delta = rect.y - newRect.y;
                    float absDelta = Mathf.Abs(delta);

                    if (absDelta > (newRect.height * 2))
                    {
                        rect.y = delta > 0 ? rect.y - newRect.height : rect.y + newRect.height;
                    }
                    else if (absDelta > 0.5)
                    {
                        rect.y = Mathf.Lerp(newRect.y, rect.y, easing);
                    }

                    _ids[id] = rect;
                    HandleUtility.Repaint();
                }

                return rect;
            }

            public Rect SetRect(int id, Rect rect)
            {
                if (_ids.ContainsKey(id))
                {
                    _ids[id] = rect;
                }
                else
                {
                    _ids.Add(id, rect);
                }

                return rect;
            }
        }

        public delegate void ActionHandler(ReorderableList list);

        public delegate void AddDropdownHandler(ReorderableList list, Rect position);

        public delegate void DragAndDropAppendHandler(ReorderableList list, Object reference);

        public delegate void DrawElementHandler(ReorderableList list, Rect position, SerializedProperty element, GUIContent label, bool isSelected, bool isFocused);

        public delegate void DrawFooterHandler(ReorderableList list, Rect position);

        public delegate void DrawHeaderHandler(ReorderableList list, Rect position, GUIContent label);

        public delegate void EditElementHandler(ReorderableList list, SerializedProperty element);

        public delegate bool ValidateHandler(ReorderableList list);

        public delegate float GetElementHeightHandler(ReorderableList list, SerializedProperty element, int index);

        public delegate float GetElementsHeightHandler(ReorderableList list);

        public delegate string GetHeaderLabelHandler(ReorderableList list);

        public delegate string GetElementNameHandler(ReorderableList list, SerializedProperty element);

        public delegate Object DragAndDropReferenceHandler(ReorderableList list, Object reference);

        public event ActionHandler OnAdd;
        public event ActionHandler OnChanged;
        public event ActionHandler OnMouseUp;
        public event ActionHandler OnRemove;
        public event ActionHandler OnReorder;
        public event ActionHandler OnSelect;
        public event AddDropdownHandler OnAddDropdown;
        public event DragAndDropAppendHandler OnAppendDragAndDrop;
        public event DragAndDropReferenceHandler OnValidateDragAndDrop;
        public event DrawElementHandler OnDrawElementBackground;
        public event DrawElementHandler OnDrawElement;
        public event DrawFooterHandler OnDrawFooter;
        public event DrawHeaderHandler OnDrawHeader;
        public event EditElementHandler OnEditElement;
        public event GetElementHeightHandler OnGetElementHeight;
        public event GetElementNameHandler OnGetElementName;
        public event GetElementsHeightHandler OnGetElementsHeight;
        public event GetHeaderLabelHandler OnGetHeaderLabel;
        public event ValidateHandler OnGetCanRemove;

        public readonly int Id;

        private static readonly int SelectionHash = "ReorderableListSelection".GetHashCode();
        private static readonly int DragAndDropHash = "ReorderableListDragAndDrop".GetHashCode();
        private static readonly ReorderableAttribute DefaultAttribute = new ReorderableAttribute();
        private static readonly char[] PathSeparator = { '.' };
        private static readonly Dictionary<int, ReorderableList> ReorderableListDictionary = new Dictionary<int, ReorderableList>();

        private int _dragDirection;
        private int _dragAndDropControlId = -1;
        private int _keyboardControlId = -1;
        private int _pressIndex;
        private float _dragPosition;
        private float _pressPosition;
        private ListSelection _beforeDragSelection;
        private ListSelection _selection;
        private GUIContent _label;
        private GUIContent _elementLabel;
        private SlideGroup _slideGroup;
        private Rect[] _elementsPositions;
        private DragElement[] _dragElements;

        public bool CanAdd => HasPermission(ReorderablePermissions.Add);
        public bool CanCollapse => HasPermission(ReorderablePermissions.Collapse);
        public bool CanDrag => HasPermission(ReorderablePermissions.Drag);
        public bool CanDuplicate => HasPermission(ReorderablePermissions.Duplicate);
        public bool CanEditElements => HasPermission(ReorderablePermissions.EditElements);
        public bool CanMultiSelect => HasPermission(ReorderablePermissions.MultiSelect);
        public bool CanRead => HasPermission(ReorderablePermissions.Read);
        public bool CanRemoveByButton => HasPermission(ReorderablePermissions.RemoveByButton);
        public bool CanRemoveByContext => HasPermission(ReorderablePermissions.RemoveByContext);
        public bool HasKeyboardControl => GUIUtility.keyboardControl == Id;
        public bool IsPropertyValid => Property != null && Property.isArray;
        public bool IsReadOnly => CurrentModePermissions == ReorderablePermissions.Read || CurrentModePermissions == ReorderablePermissions.Collapse;
        public int Count => IsPropertyValid ? Property.arraySize : 0;
        public ReorderablePermissions CurrentModePermissions => EditorApplication.isPlaying ? PlayModePermissions : EditModePermissions;

        public float ElementsHeight
        {
            get
            {
                if (OnGetElementsHeight != null)
                {
                    return OnGetElementsHeight(this);
                }

                int i;
                int size = Property.arraySize;

                if (size == 0)
                {
                    return 28;
                }

                float height = 0;

                for (i = 0; i < size; i++)
                {
                    height += GetElementHeight(Property.GetArrayElementAtIndex(i), i);
                }

                return height + 7;
            }
        }
        public float TotalHeight
        {
            get
            {
                if (CanRead == false)
                {
                    return 0;
                }

                if (IsPropertyValid)
                {
                    if (CanAdd || CanRemoveByButton)
                    {
                        return Property.isExpanded ? HeaderHeight + ElementsHeight + FooterHeight : HeaderHeight;
                    }

                    return Property.isExpanded ? HeaderHeight + ElementsHeight : HeaderHeight;
                }

                return EditorGUIUtility.singleLineHeight;
            }
        }

        [SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global")]
        public bool ShowDefaultBackground { get; set; }
        [SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global")]
        public float FooterHeight { get; set; }
        [SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global")]
        public float HeaderHeight { get; set; }
        [SuppressMessage("ReSharper", "AutoPropertyCanBeMadeGetOnly.Global")]
        public float SlideEasing { get; set; }
        public string ElementName { get; set; }
        public ReorderableElementDisplay ElementDisplay { get; set; }
        public ReorderablePermissions EditModePermissions { get; set; }
        public ReorderablePermissions PlayModePermissions { get; set; }

        public int SelectedIndex { get => _selection.First; set => _selection.Select(value); }
        public int[] SelectedIndexes { get => _selection.ToArray(); set => _selection = new ListSelection(value); }

        public bool IsDragging { get; private set; }
        public SerializedProperty Property { get; private set; }
        public SerializedProperty RawProperty { get; private set; }

        public ReorderableList(SerializedProperty property, ReorderableElementDisplay elementDisplay = ReorderableElementDisplay.Auto)
            : this(property, null, ReorderablePermissions.Everything, ReorderablePermissions.Everything, elementDisplay) { }

        public ReorderableList(SerializedProperty property, ReorderablePermissions permissions, ReorderableElementDisplay elementDisplay = ReorderableElementDisplay.Auto)
            : this(property, null, permissions, permissions, elementDisplay) { }

        public ReorderableList(SerializedProperty property, ReorderablePermissions editModePermissions, ReorderablePermissions playModePermissions, ReorderableElementDisplay elementDisplay = ReorderableElementDisplay.Auto)
            : this(property, null, editModePermissions, playModePermissions, elementDisplay) { }

        /// <param name="elementName">
        /// Change the element display name. If the element is an <see cref="UnityEngine.Object"/> you can use the value "name" to display the object's name.
        /// <para>
        /// To change it based on a method of the inspected object, use "MethodName" where the method signature is "string MethodName(IList, int)".
        /// </para>
        /// <para>
        /// To change it based on a SerializedField on the element, use the "$" as the first character. E.g. "$elementFieldName", "$elementFieldName.nested"
        /// </para>
        /// <para>
        /// To change it to a constant string followed by the element index, use "@" as the first character. E.g. "@MyElementName"
        /// </para>
        /// </param>
        public ReorderableList(SerializedProperty property, string elementName, ReorderableElementDisplay elementDisplay = ReorderableElementDisplay.Auto)
            : this(property, elementName, ReorderablePermissions.Everything, ReorderablePermissions.Everything, elementDisplay) { }

        /// <param name="elementName">
        /// Change the element display name. If the element is an <see cref="UnityEngine.Object"/> you can use the value "name" to display the object's name.
        /// <para>
        /// To change it based on a method of the inspected object, use "MethodName" where the method signature is "string MethodName(IList, int)".
        /// </para>
        /// <para>
        /// To change it based on a SerializedField on the element, use the "$" as the first character. E.g. "$elementFieldName", "$elementFieldName.nested"
        /// </para>
        /// <para>
        /// To change it to a constant string followed by the element index, use "@" as the first character. E.g. "@MyElementName"
        /// </para>
        /// </param>
        public ReorderableList(SerializedProperty property, string elementName, ReorderablePermissions permissions, ReorderableElementDisplay elementDisplay = ReorderableElementDisplay.Auto)
            : this(property, elementName, permissions, permissions, elementDisplay) { }

        /// <param name="elementName">
        /// Change the element display name. If the element is an <see cref="UnityEngine.Object"/> you can use the value "name" to display the object's name.
        /// <para>
        /// To change it based on a method of the inspected object, use "MethodName" where the method signature is "string MethodName(IList, int)".
        /// </para>
        /// <para>
        /// To change it based on a SerializedField on the element, use the "$" as the first character. E.g. "$elementFieldName", "$elementFieldName.nested"
        /// </para>
        /// <para>
        /// To change it to a constant string followed by the element index, use "@" as the first character. E.g. "@MyElementName"
        /// </para>
        /// </param>
        public ReorderableList(SerializedProperty property, string elementName, ReorderablePermissions editModePermissions, ReorderablePermissions playModePermissions, ReorderableElementDisplay elementDisplay = ReorderableElementDisplay.Auto)
        {
            SetProperty(property);

            ElementName = elementName;
            EditModePermissions = editModePermissions;
            PlayModePermissions = playModePermissions;
            ElementDisplay = elementDisplay;

            FooterHeight = 13f;
            HeaderHeight = 18f;
            ShowDefaultBackground = true;
            SlideEasing = 0.15f;

            _elementLabel = new GUIContent();
            _selection = new ListSelection();
            _slideGroup = new SlideGroup();
            _elementsPositions = new Rect[0];

            Id = GetHashCode();
        }

        public static int GetReorderableListId(SerializedProperty property)
        {
            if (property != null)
            {
                int hash1 = property.serializedObject.targetObject.GetHashCode();
                int hash2 = property.propertyPath.GetHashCode();

                return ((hash1 << 5) + hash1) ^ hash2;
            }

            return 0;
        }

        public static ReorderableList GetReorderableList(SerializedProperty property)
        {
            return GetReorderableList(property, null, GetReorderableListId(property));
        }

        public static ReorderableList GetReorderableList(SerializedProperty property, ReorderableAttribute attribute)
        {
            return GetReorderableList(property, attribute, GetReorderableListId(property));
        }

        public static ReorderableList GetReorderableList(SerializedProperty property, int id)
        {
            return GetReorderableList(property, null, id);
        }

        public static ReorderableList GetReorderableList(SerializedProperty property, ReorderableAttribute attribute, int id)
        {
            if (property == null)
            {
                return null;
            }

            if (ReorderableListDictionary.TryGetValue(id, out ReorderableList reorderableList))
            {
                if (attribute == null)
                {
                    attribute = DefaultAttribute;
                }

                reorderableList.SetProperty(property);
                reorderableList.ElementName = attribute.ElementName;
                reorderableList.EditModePermissions = attribute.EditModePermissions;
                reorderableList.PlayModePermissions = attribute.PlayModePermissions;
                reorderableList.ElementDisplay = attribute.ElementDisplay;
            }
            else
            {
                reorderableList = attribute == null
                                      ? new ReorderableList(property)
                                      : new ReorderableList(property,
                                                            attribute.ElementName,
                                                            attribute.EditModePermissions,
                                                            attribute.PlayModePermissions,
                                                            attribute.ElementDisplay);

                ReorderableListDictionary.Add(id, reorderableList);
            }

            return reorderableList;
        }

        public void DrawGUI(Rect position)
        {
            if (CanRead == false)
            {
                return;
            }

            DrawGUI(position, _label);
        }

        public void DrawGUI(Rect position, GUIContent label)
        {
            if (CanRead == false)
            {
                return;
            }

            using (new LabelWidthScope(-(EditorGUI.IndentedRect(position).x - position.x)))
            {
                using (new EditorGUI.IndentLevelScope(-EditorGUI.indentLevel))
                {
                    Rect headerPosition = position;
                    headerPosition.height = HeaderHeight;

                    if (IsPropertyValid == false)
                    {
                        DrawEmpty(headerPosition, label.text + " must be a SerializableList", GUIStyle.none, EditorStyles.helpBox);
                    }
                    else
                    {
                        using (new EditorGUI.DisabledScope(IsReadOnly))
                        {
                            _keyboardControlId = GUIUtility.GetControlID(SelectionHash, FocusType.Keyboard, position);
                            _dragAndDropControlId = GUIUtility.GetControlID(DragAndDropHash, FocusType.Passive, position);

                            DrawHeader(headerPosition, label);

                            if (Property.isExpanded)
                            {
                                Rect elementBackgroundPosition = position;
                                elementBackgroundPosition.yMin = headerPosition.yMax;
                                elementBackgroundPosition.yMax = CanAdd || CanRemoveByButton ? position.yMax - FooterHeight : position.yMax;

                                Event e = Event.current;

                                if (_selection.Length > 1)
                                {
                                    if (e.type == EventType.ContextClick && CanSelect(e.mousePosition))
                                    {
                                        HandleMultipleContextClick(e);
                                    }
                                }

                                if (Property.arraySize > 0)
                                {
                                    if (!IsDragging)
                                    {
                                        UpdateElementsPositions(elementBackgroundPosition, e);
                                    }

                                    if (_elementsPositions.Length > 0)
                                    {
                                        Rect selectablePosition = elementBackgroundPosition;
                                        selectablePosition.yMin = _elementsPositions[0].yMin;
                                        selectablePosition.yMax = _elementsPositions[_elementsPositions.Length - 1].yMax;

                                        DrawElements(elementBackgroundPosition, e);
                                        HandlePreSelection(selectablePosition, e);
                                        HandlePostSelection(selectablePosition, e);
                                    }
                                }
                                else
                                {
                                    DrawEmpty(elementBackgroundPosition, "List is Empty", Style.BoxBackground, Style.VerticalLabel);
                                }

                                if (CanAdd || CanRemoveByButton)
                                {
                                    Rect footerRect = position;
                                    footerRect.yMin = elementBackgroundPosition.yMax;
                                    footerRect.xMin = position.xMax - 58;

                                    DrawFooter(footerRect);
                                }
                            }
                        }
                    }
                }
            }
        }

        public void DrawGUILayout()
        {
            if (CanRead == false)
            {
                return;
            }

            DrawGUI(EditorGUI.IndentedRect(EditorGUILayout.GetControlRect(false, TotalHeight, EditorStyles.largeLabel)), _label);
        }

        public void DrawGUILayout(GUIContent label)
        {
            if (CanRead == false)
            {
                return;
            }

            DrawGUI(EditorGUI.IndentedRect(EditorGUILayout.GetControlRect(false, TotalHeight, EditorStyles.largeLabel)), label);
        }

        public void GrabKeyboardFocus()
        {
            GUIUtility.keyboardControl = Id;
        }

        public void ReleaseKeyboardFocus()
        {
            if (GUIUtility.keyboardControl == Id)
            {
                GUIUtility.keyboardControl = 0;
            }
        }

        public void Remove(int[] selection)
        {
            System.Array.Sort(selection);

            int i = selection.Length;

            while (--i > -1)
            {
                RemoveItem(selection[i]);
            }
        }

        public void RemoveItem(int index)
        {
            if (IsPropertyValid && index >= 0 && index < Property.arraySize)
            {
                SerializedProperty element = Property.GetArrayElementAtIndex(index);

                if (element.propertyType == SerializedPropertyType.ObjectReference && element.objectReferenceValue)
                {
                    element.objectReferenceValue = null;
                }

                Property.DeleteArrayElementAtIndex(index);
                _selection.Remove(index);

                if (Property.arraySize > 0)
                {
                    _selection.Select(Mathf.Max(0, index - 1));
                }

                DispatchChanges();
            }
        }

        public bool IsSelected(int index)
        {
            return _selection.Contains(index);
        }

        public int IndexOf(SerializedProperty elementProperty)
        {
            if (elementProperty != null)
            {
                int i = Property.arraySize;

                while (--i > -1)
                {
                    if (SerializedProperty.EqualContents(elementProperty, Property.GetArrayElementAtIndex(i)))
                    {
                        return i;
                    }
                }
            }

            return -1;
        }

        public SerializedProperty AddItem<T>(T item) where T : Object
        {
            SerializedProperty property = AddItem();

            if (property != null)
            {
                property.objectReferenceValue = item;
            }

            return property;
        }

        public SerializedProperty AddItem()
        {
            if (IsPropertyValid)
            {
                Property.arraySize++;
                _selection.Select(Property.arraySize - 1);
                DispatchChanges();

                return Property.GetArrayElementAtIndex(_selection.Last);
            }

            Debug.LogException(new InvalidListException());

            return null;
        }

        public SerializedProperty GetItem(int index)
        {
            if (IsPropertyValid && index >= 0 && index < Property.arraySize)
            {
                return Property.GetArrayElementAtIndex(index);
            }

            return null;
        }

        private void AppendDragAndDropValue(Object reference)
        {
            Internals.AppendDragAndDropValue(reference, Property);
            DispatchChanges();
        }

        private void DispatchChanges()
        {
            OnChanged?.Invoke(this);
        }

        private void DoSelection(int index, bool setKeyboardControl, Event e)
        {
            _pressIndex = index;

            if (CanMultiSelect)
            {
                _selection.AppendWithAction(_pressIndex, e);
            }
            else
            {
                _selection.Select(_pressIndex);
            }

            OnSelect?.Invoke(this);

            if (CanDrag)
            {
                IsDragging = false;
                _dragPosition = _pressPosition = e.mousePosition.y;
                _dragElements = GetDragList(_dragPosition);
                _beforeDragSelection = _selection.Clone();

                GUIUtility.hotControl = _keyboardControlId;
            }

            if (setKeyboardControl)
            {
                GUIUtility.keyboardControl = _keyboardControlId;
            }

            e.Use();
        }

        private void DrawElement(SerializedProperty elementProperty, Rect position, bool selected, bool focused)
        {
            using (new EditorGUI.DisabledScope(CanEditElements == false))
            {
                Event e = Event.current;

                if (OnDrawElementBackground != null)
                {
                    OnDrawElementBackground.Invoke(this, position, elementProperty, null, selected, focused);
                }
                else if (e.type == EventType.Repaint)
                {
                    Style.ElementBackground.Draw(position, false, selected, selected, focused);
                }

                if (e.type == EventType.Repaint && CanDrag)
                {
                    Style.DraggingHandle.Draw(new Rect(position.x + 5, position.y + 6, 10, position.height - (position.height - 6)), false, false, false, false);
                }

                GUIContent label = GetElementLabel(elementProperty);
                Rect renderRect = GetElementRenderPosition(elementProperty, position);

                if (ElementDisplay == ReorderableElementDisplay.Custom)
                {
                    renderRect.xMin -= 10;
                }

                float offset = renderRect.x - position.x;

                using (new LabelWidthScope(-offset))
                {
                    using (var changeCheckScope = new EditorGUI.ChangeCheckScope())
                    {
                        if (OnDrawElement != null)
                        {
                            OnDrawElement.Invoke(this, renderRect, elementProperty, label, selected, focused);
                        }
                        else
                        {
                            EditorGUI.PropertyField(renderRect, elementProperty, label, true);
                        }

                        if (changeCheckScope.changed)
                        {
                            OnEditElement?.Invoke(this, elementProperty);
                        }
                    }
                }

                int controlId = GUIUtility.GetControlID(label, FocusType.Passive, position);

                switch (e.GetTypeForControl(controlId))
                {
                    case EventType.ContextClick:

                        if (position.Contains(e.mousePosition))
                        {
                            HandleContextClick(e, elementProperty);
                        }

                        break;
                }
            }
        }

        private void DrawElements(Rect position, Event e)
        {
            if (ShowDefaultBackground && e.type == EventType.Repaint)
            {
                Style.BoxBackground.Draw(position, false, false, false, false);
            }

            if (IsDragging == false)
            {
                int i;
                int length = Property.arraySize;

                for (i = 0; i < length; i++)
                {
                    bool isSelected = _selection.Contains(i);
                    DrawElement(Property.GetArrayElementAtIndex(i), GetElementDrawPosition(i, _elementsPositions[i]), isSelected, isSelected && GUIUtility.keyboardControl == _keyboardControlId);
                }
            }
            else if (e.type == EventType.Repaint)
            {
                int dragIndex;
                int dragLength = _dragElements.Length;
                int selectionLength = _selection.Length;

                for (dragIndex = 0; dragIndex < selectionLength; dragIndex++)
                {
                    DragElement element = _dragElements[dragIndex];
                    element.DesiredRect.y = _dragPosition - element.DragOffset;
                    _dragElements[dragIndex] = element;
                }

                dragIndex = dragLength;

                while (--dragIndex > -1)
                {
                    DragElement element = _dragElements[dragIndex];

                    if (element.Selected)
                    {
                        DrawElement(element.Property, element.DesiredRect, true, true);

                        continue;
                    }

                    Rect elementRect = element.Rect;
                    int elementIndex = element.StartIndex;
                    int startIndex = _dragDirection > 0 ? selectionLength - 1 : 0;
                    int endIndex = _dragDirection > 0 ? -1 : selectionLength;

                    for (int i = startIndex; i != endIndex; i -= _dragDirection)
                    {
                        DragElement selected = _dragElements[i];

                        if (selected.Overlaps(elementRect, elementIndex, _dragDirection))
                        {
                            elementRect.y -= selected.Rect.height * _dragDirection;
                            elementIndex += _dragDirection;
                        }
                    }

                    DrawElement(element.Property, GetElementDrawPosition(dragIndex, elementRect), false, false);

                    element.DesiredRect = elementRect;
                    _dragElements[dragIndex] = element;
                }
            }
        }

        private void DrawEmpty(Rect position, string label, GUIStyle backgroundStyle, GUIStyle labelStyle)
        {
            if (ShowDefaultBackground && Event.current.type == EventType.Repaint)
            {
                backgroundStyle.Draw(position, false, false, false, false);
            }

            EditorGUI.LabelField(position, label, labelStyle);
        }

        private void DrawFooter(Rect position)
        {
            if (OnDrawFooter != null)
            {
                OnDrawFooter.Invoke(this, position);

                return;
            }

            if (Event.current.type == EventType.Repaint)
            {
                Style.FooterBackground.Draw(position, false, false, false, false);
            }

            var addRect = new Rect(position.xMin + 4f, position.y - 3f, 25f, 13f);
            var subRect = new Rect(position.xMax - 29f, position.y - 3f, 25f, 13f);

            using (new EditorGUI.DisabledScope(!CanAdd))
            {
                if (CanAdd && GUI.Button(addRect, OnAddDropdown != null ? Style.IconToolbarPlusMore : Style.IconToolbarPlus, Style.PreButton))
                {
                    if (OnAddDropdown != null)
                    {
                        OnAddDropdown.Invoke(this, addRect);
                    }
                    else if (OnAdd != null)
                    {
                        OnAdd(this);
                    }
                    else
                    {
                        AddItem();
                    }
                }
            }

            using (new EditorGUI.DisabledScope(!CanSelect(_selection) || !CanRemoveByButton || OnGetCanRemove != null && !OnGetCanRemove(this)))
            {
                if (CanRemoveByButton && GUI.Button(subRect, Style.IconToolbarMinus, Style.PreButton))
                {
                    if (OnRemove != null)
                    {
                        OnRemove(this);
                    }
                    else
                    {
                        Remove(_selection.ToArray());
                    }
                }
            }
        }

        private void DrawHeader(Rect position, GUIContent label)
        {
            if (ShowDefaultBackground && Event.current.type == EventType.Repaint)
            {
                Style.HeaderBackground.Draw(position, false, false, false, false);
            }

            HandleDragAndDrop(position, Event.current);

            Rect titlePosition = position;
            titlePosition.xMin += 6f;
            titlePosition.xMax -= 55f;
            titlePosition.height -= 2f;
            titlePosition.y++;

            var titleLabel = new GUIContent(label);
            titleLabel.text = OnGetHeaderLabel != null ? OnGetHeaderLabel.Invoke(this) : $"{titleLabel.text} [{Property.arraySize}]";

            using (var propertyScope = new EditorGUI.PropertyScope(titlePosition, titleLabel, Property))
            {
                if (OnDrawHeader != null)
                {
                    OnDrawHeader.Invoke(this, titlePosition, propertyScope.content);
                }
                else if (CanCollapse)
                {
                    titlePosition.xMin += 10;

                    EditorGUI.BeginChangeCheck();

                    bool isExpanded = EditorGUI.Foldout(titlePosition, Property.isExpanded, propertyScope.content, true);

                    if (EditorGUI.EndChangeCheck())
                    {
                        Property.isExpanded = isExpanded;

                        foreach (Object target in Property.serializedObject.targetObjects)
                        {
                            EditorUtility.SetDirty(target);
                        }
                    }
                }
                else
                {
                    Property.isExpanded = true;
                    GUI.Label(titlePosition, propertyScope.content, EditorStyles.label);
                }
            }

            if (ElementDisplay != ReorderableElementDisplay.NonExpandable)
            {
                Rect expandButtonPosition = position;
                expandButtonPosition.xMin = position.xMax - 25;
                expandButtonPosition.xMax = position.xMax - 5;

                if (GUI.Button(expandButtonPosition, Style.ExpandButton, Style.PreButton))
                {
                    ExpandElements(true);
                }

                Rect collapseButtonPosition = position;
                collapseButtonPosition.xMin = expandButtonPosition.xMin - 20;
                collapseButtonPosition.xMax = expandButtonPosition.xMin;

                if (GUI.Button(collapseButtonPosition, Style.CollapseButton, Style.PreButton))
                {
                    ExpandElements(false);
                }
            }
        }

        private void ExpandElements(bool expand)
        {
            if (Property.isExpanded == false && expand)
            {
                Property.isExpanded = true;
            }

            for (int i = 0; i < Property.arraySize; i++)
            {
                Property.GetArrayElementAtIndex(i).isExpanded = expand;
            }
        }

        private void HandleContextClick(Event e, SerializedProperty elementProperty)
        {
            _selection.Select(IndexOf(elementProperty));

            var menu = new GenericMenu();

            if (elementProperty.isInstantiatedPrefab)
            {
                menu.AddItem(new GUIContent($"Revert {GetElementLabel(elementProperty).text} to Prefab"), false, _selection.RevertValues, Property);

                if (CanDuplicate || CanRemoveByContext)
                {
                    menu.AddSeparator(string.Empty);
                }
            }

            if (CanDuplicate)
            {
                menu.AddItem(new GUIContent("Duplicate Element"), false, HandleDuplicate, Property);
            }

            if (CanRemoveByContext)
            {
                menu.AddItem(new GUIContent("Delete Element"), false, HandleDelete, Property);
            }

            if (menu.GetItemCount() > 0)
            {
                menu.ShowAsContext();
            }

            e.Use();
        }

        private void HandleDelete(object userData)
        {
            _selection.Delete(userData as SerializedProperty);
            DispatchChanges();
        }

        private void HandleDragAndDrop(Rect position, Event e)
        {
            switch (e.GetTypeForControl(_dragAndDropControlId))
            {
                case EventType.DragUpdated:
                case EventType.DragPerform:
                {
                    if (CanAdd && GUI.enabled && position.Contains(e.mousePosition))
                    {
                        Object[] objectReferences = DragAndDrop.objectReferences;
                        var references = new Object[1];
                        bool acceptDrag = false;

                        foreach (Object reference in objectReferences)
                        {
                            references[0] = reference;
                            Object validatedReference = ValidateDragAndDropObject(references);

                            if (validatedReference != null)
                            {
                                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                                if (e.type == EventType.DragPerform)
                                {
                                    if (OnAppendDragAndDrop != null)
                                    {
                                        OnAppendDragAndDrop.Invoke(this, validatedReference);
                                    }
                                    else
                                    {
                                        AppendDragAndDropValue(validatedReference);
                                    }

                                    acceptDrag = true;
                                    DragAndDrop.activeControlID = 0;
                                }
                                else
                                {
                                    DragAndDrop.activeControlID = _dragAndDropControlId;
                                }
                            }
                        }

                        if (acceptDrag)
                        {
                            GUI.changed = true;
                            DragAndDrop.AcceptDrag();
                        }
                    }

                    break;
                }

                case EventType.DragExited:
                {
                    if (GUI.enabled)
                    {
                        HandleUtility.Repaint();
                    }

                    break;
                }
            }
        }

        private void HandleDuplicate(object userData)
        {
            _selection.Duplicate(userData as SerializedProperty);
            DispatchChanges();
        }

        private void HandleMultipleContextClick(Event currentEvent)
        {
            var menu = new GenericMenu();

            if (_selection.CanRevert(Property))
            {
                menu.AddItem(new GUIContent("Revert Values to Prefab"), false, _selection.RevertValues, Property);

                if (CanDuplicate || CanRemoveByContext)
                {
                    menu.AddSeparator(string.Empty);
                }
            }

            if (CanDuplicate)
            {
                menu.AddItem(new GUIContent("Duplicate Elements"), false, HandleDuplicate, Property);
            }

            if (CanRemoveByContext)
            {
                menu.AddItem(new GUIContent("Delete Elements"), false, HandleDelete, Property);
            }

            if (menu.GetItemCount() > 0)
            {
                menu.ShowAsContext();
            }

            currentEvent.Use();
        }

        private void HandlePostSelection(Rect position, Event e)
        {
            switch (e.GetTypeForControl(_keyboardControlId))
            {
                case EventType.MouseDown:
                {
                    if (position.Contains(e.mousePosition) && IsSelectionButton(e))
                    {
                        int index = GetSelectionIndex(e.mousePosition);

                        if (CanSelect(index))
                        {
                            DoSelection(index, GUIUtility.keyboardControl == 0 || GUIUtility.keyboardControl == _keyboardControlId || e.button == 2, e);
                        }
                        else
                        {
                            _selection.Clear();
                        }

                        HandleUtility.Repaint();
                    }

                    break;
                }

                case EventType.MouseUp:
                {
                    if (!CanDrag)
                    {
                        _selection.SelectWhenNoAction(_pressIndex, e);

                        if (OnMouseUp != null && IsPositionWithinElement(e.mousePosition, _selection.Last))
                        {
                            OnMouseUp.Invoke(this);
                        }
                    }
                    else if (GUIUtility.hotControl == _keyboardControlId)
                    {
                        e.Use();

                        if (IsDragging)
                        {
                            IsDragging = false;
                            ReorderDraggedElements(_dragElements);
                            Property.serializedObject.ApplyModifiedProperties();
                            Property.serializedObject.Update();

                            OnReorder?.Invoke(this);

                            DispatchChanges();
                        }
                        else
                        {
                            _selection.SelectWhenNoAction(_pressIndex, e);

                            OnMouseUp?.Invoke(this);
                        }

                        GUIUtility.hotControl = 0;
                    }

                    HandleUtility.Repaint();

                    break;
                }

                case EventType.KeyDown:
                {
                    if (GUIUtility.keyboardControl == _keyboardControlId)
                    {
                        if (e.keyCode == KeyCode.DownArrow && !IsDragging)
                        {
                            _selection.Select(Mathf.Min(_selection.Last + 1, Property.arraySize - 1));
                            e.Use();
                        }
                        else if (e.keyCode == KeyCode.UpArrow && !IsDragging)
                        {
                            _selection.Select(Mathf.Max(_selection.Last - 1, 0));
                            e.Use();
                        }
                        else if (e.keyCode == KeyCode.Escape && GUIUtility.hotControl == _keyboardControlId)
                        {
                            GUIUtility.hotControl = 0;

                            if (IsDragging)
                            {
                                IsDragging = false;
                                _selection = _beforeDragSelection;
                            }

                            e.Use();
                        }
                    }

                    break;
                }
            }
        }

        private void HandlePreSelection(Rect position, Event e)
        {
            if (e.type == EventType.MouseDown)
            {
                if (position.Contains(e.mousePosition) && IsSelectionButton(e))
                {
                    int index = GetSelectionIndex(e.mousePosition);

                    if (CanSelect(index))
                    {
                        SerializedProperty element = Property.GetArrayElementAtIndex(index);

                        if (IsElementExpandable(element))
                        {
                            Rect elementHeaderRect = GetElementHeaderPosition(_elementsPositions[index]);
                            Rect elementRenderRect = GetElementRenderPosition(element, _elementsPositions[index]);
                            Rect elementExpandRect = elementHeaderRect;
                            elementExpandRect.xMin = elementRenderRect.xMin - 10;
                            elementExpandRect.xMax = elementRenderRect.xMin;

                            if (elementHeaderRect.Contains(e.mousePosition) && !elementExpandRect.Contains(e.mousePosition))
                            {
                                DoSelection(index, true, e);
                                HandleUtility.Repaint();
                            }
                        }
                    }
                }
            }
            else if (e.type == EventType.MouseDrag && CanDrag && GUIUtility.hotControl == _keyboardControlId)
            {
                if (_selection.Length > 0 && UpdateDragPosition(e.mousePosition, position, _dragElements))
                {
                    GUIUtility.keyboardControl = _keyboardControlId;
                    IsDragging = true;
                }

                e.Use();
            }
        }

        private void ReorderDraggedElements(DragElement[] dragList)
        {
            for (int i = 0; i < dragList.Length; i++)
            {
                dragList[i].RecordState();
            }

            System.Array.Sort(dragList, (a, b) => a.DesiredRect.center.y.CompareTo(b.DesiredRect.center.y));

            _selection.Sort(delegate(int a, int b)
            {
                int d1 = GetDragIndexFromSelection(a);
                int d2 = GetDragIndexFromSelection(b);

                return _dragDirection > 0 ? d1.CompareTo(d2) : d2.CompareTo(d1);
            });

            int selectionLength = _selection.Length;

            while (--selectionLength > -1)
            {
                int newIndex = GetDragIndexFromSelection(_selection[selectionLength]);
                _selection[selectionLength] = newIndex;
                Property.MoveArrayElement(dragList[newIndex].StartIndex, newIndex);
            }

            for (int i = 0; i < dragList.Length; i++)
            {
                dragList[i].RestoreState(Property.GetArrayElementAtIndex(i));
            }
        }

        private void SetProperty(SerializedProperty value)
        {
            if (value == null)
            {
                Debug.LogException(new MissingListException("value"));

                return;
            }

            RawProperty = value;

            if (value.isArray == false)
            {
                SerializedProperty property = value.FindPropertyRelative("_items");

                if (property == null || property.isArray == false)
                {
                    Debug.LogException(new InvalidListException());

                    return;
                }

                Property = property;
            }
            else
            {
                Property = value;
            }

            _label = new GUIContent(value.displayName);
        }

        private void UpdateElementsPositions(Rect position, Event e)
        {
            int length = Property.arraySize;

            if (length != _elementsPositions.Length)
            {
                System.Array.Resize(ref _elementsPositions, length);
            }

            if (e.type == EventType.Repaint)
            {
                Rect elementPosition = position;
                elementPosition.yMin = elementPosition.yMax = position.yMin + EditorGUIUtility.standardVerticalSpacing;

                for (int i = 0; i < length; i++)
                {
                    SerializedProperty elementProperty = Property.GetArrayElementAtIndex(i);

                    elementPosition.y = elementPosition.yMax;
                    elementPosition.height = GetElementHeight(elementProperty, i);
                    _elementsPositions[i] = elementPosition;
                }
            }
        }

        private bool CanSelect(int index)
        {
            return index >= 0 && index < Property.arraySize;
        }

        private bool CanSelect(Vector2 position)
        {
            return _selection.Length > 0 && _selection.Any(s => IsPositionWithinElement(position, s));
        }

        private bool CanSelect(ListSelection selection)
        {
            return selection.Length > 0 && selection.All(CanSelect);
        }

        private bool HasPermission(ReorderablePermissions value)
        {
            return ((EditorApplication.isPlaying ? PlayModePermissions : EditModePermissions) & value) == value;
        }

        private bool IsElementExpandable(SerializedProperty elementProperty)
        {
            switch (ElementDisplay)
            {
                case ReorderableElementDisplay.Auto:

                    return elementProperty.hasVisibleChildren && elementProperty.propertyType != SerializedPropertyType.ObjectReference;

                case ReorderableElementDisplay.Custom:
                case ReorderableElementDisplay.Expandable:

                    return true;

                case ReorderableElementDisplay.NonExpandable:

                    return false;
            }

            return false;
        }

        private bool IsPositionWithinElement(Vector2 position, int index)
        {
            return CanSelect(index) && _elementsPositions[index].Contains(position);
        }

        private bool IsSelectionButton(Event e)
        {
            return e.button == 0 || e.button == 2;
        }

        private bool UpdateDragPosition(Vector2 position, Rect bounds, IList<DragElement> dragElements)
        {
            int endIndex = _selection.Length - 1;
            float minOffset = dragElements[0].DragOffset;
            float maxOffset = dragElements[endIndex].Rect.height - dragElements[endIndex].DragOffset;

            _dragPosition = Mathf.Clamp(position.y, bounds.yMin + minOffset, bounds.yMax - maxOffset);

            if (Mathf.Abs(_dragPosition - _pressPosition) > 1)
            {
                _dragDirection = (int)Mathf.Sign(_dragPosition - _pressPosition);

                return true;
            }

            return false;
        }

        private int GetDragIndexFromSelection(int index)
        {
            return System.Array.FindIndex(_dragElements, t => t.StartIndex == index);
        }

        private int GetSelectionIndex(Vector2 position)
        {
            for (int i = 0, length = _elementsPositions.Length; i < length; i++)
            {
                Rect rect = _elementsPositions[i];

                if (rect.Contains(position) || i == 0 && position.y <= rect.yMin || i == (length - 1) && position.y >= rect.yMax)
                {

                    return i;
                }
            }

            return -1;
        }

        private float GetElementHeight(SerializedProperty elementProperty, int index)
        {
            if (OnGetElementHeight != null)
            {
                return OnGetElementHeight.Invoke(this, elementProperty, index) + 5;
            }

            return EditorGUI.GetPropertyHeight(elementProperty, GUIContent.none, true) + 5;
        }

        private string GetIndexedElementName(string name, string propertyPath)
        {
            if (propertyPath.EndsWith("]"))
            {
                int startIndex = propertyPath.LastIndexOf('[') + 1;

                return $"{name} {propertyPath.Substring(startIndex, propertyPath.Length - startIndex - 1)}";
            }

            return name;
        }

        private string GetDynamicElementNameWithMethod(string methodName, int elementIndex)
        {
            const BindingFlags flags = BindingFlags.Public
                                     | BindingFlags.NonPublic
                                     | BindingFlags.Static
                                     | BindingFlags.Instance
                                     | BindingFlags.FlattenHierarchy;

            object target = RawProperty.GetParentObject();
            MethodInfo method = target.GetType().GetMethod(methodName, flags);

            if (method == null)
            {
                Debug.LogError($"Missing member {target}.{methodName}");

                return null;
            }

            ParameterInfo returnParameter = method.ReturnParameter;

            if (returnParameter == null || returnParameter.ParameterType != typeof(string))
            {
                Debug.LogError($"{target}.{methodName} return should be (string)");

                return null;
            }

            ParameterInfo[] methodParameters = method.GetParameters();

            if (methodParameters.Length != 2 || !typeof(IList).IsAssignableFrom(methodParameters[0].ParameterType) || methodParameters[1].ParameterType != typeof(int))
            {
                Debug.LogError($"{target}.{methodName} parameters should be (IList, int)");

                return null;
            }

            return (string)method.Invoke(target, new[] { RawProperty.GetValue(), elementIndex });
        }

        private string GetDynamicElementNameWithSerializedPropertyPath(string propertyPath, SerializedProperty elementProperty)
        {
            if (elementProperty.propertyType == SerializedPropertyType.ObjectReference
             && elementProperty.objectReferenceValue == null)
            {
                return null;
            }

            string[] splitPath = propertyPath.Split(PathSeparator, System.StringSplitOptions.RemoveEmptyEntries);
            SerializedProperty property = elementProperty;

            for (int i = 0; i < splitPath.Length; i++)
            {
                if (property.GetValue() == null)
                {
                    return null;
                }

                if (property.propertyType == SerializedPropertyType.ObjectReference)
                {
                    property = new SerializedObject(property.objectReferenceValue).FindProperty(splitPath[i]);
                }
                else
                {
                    property = property.FindPropertyRelative(splitPath[i]);
                }

                if (property == null)
                {
                    Debug.LogError($"SerializedProperty not found: {propertyPath}");

                    return null;
                }
            }

            switch (property.propertyType)
            {
                case SerializedPropertyType.ObjectReference:

                    return property.objectReferenceValue ? property.objectReferenceValue.name : null;

                case SerializedPropertyType.Enum:

                    return property.enumDisplayNames[property.enumValueIndex];

                case SerializedPropertyType.Character:

                    return ((char)property.intValue).ToString();

                case SerializedPropertyType.LayerMask:

                    return GetLayerMaskName(property.intValue);

                default:

                    return property.GetValue()?.ToString();
            }
        }

        private string GetElementName(SerializedProperty elementProperty, string name)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                return name;
            }

            name = name.Trim();

            if (elementProperty.propertyType == SerializedPropertyType.ObjectReference && name == "name")
            {
                return elementProperty.objectReferenceValue ? elementProperty.objectReferenceValue.name : null;
            }

            switch (name[0])
            {
                case '@':

                    return GetIndexedElementName(name.Remove(0, 1), elementProperty.propertyPath);

                case '$':

                    return GetDynamicElementNameWithSerializedPropertyPath(name.Remove(0, 1), elementProperty);

                default:

                    return GetDynamicElementNameWithMethod(name, IndexOf(elementProperty));
            }
        }

        private string GetLayerMaskName(int mask)
        {
            if (mask == 0)
            {
                return "Nothing";
            }

            if (mask < 0)
            {
                return "Everything";
            }

            string name = string.Empty;
            int n = 0;

            for (int i = 0; i < 32; i++)
            {
                if (((1 << i) & mask) != 0)
                {
                    if (n == 4)
                    {
                        return "Mixed ...";
                    }

                    name += $"{(n > 0 ? ", " : string.Empty)}{LayerMask.LayerToName(i)}";
                    n++;
                }
            }

            return name;
        }

        private Rect GetElementDrawPosition(int index, Rect position)
        {
            if (SlideEasing <= 0)
            {
                return position;
            }

            return IsDragging ? _slideGroup.GetRect(_dragElements[index].StartIndex, position, SlideEasing) : _slideGroup.SetRect(index, position);
        }

        private Rect GetElementHeaderPosition(Rect elementPosition)
        {
            elementPosition.height = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;

            return elementPosition;
        }

        private Rect GetElementRenderPosition(SerializedProperty elementProperty, Rect elementPosition)
        {
            float offset = CanDrag ? 20 : 5;
            Rect position = elementPosition;
            position.xMin += IsElementExpandable(elementProperty) ? offset + 10 : offset;
            position.xMax -= 5;
            position.yMin += 1;
            position.yMax -= 1;

            return position;
        }

        private GUIContent GetElementLabel(SerializedProperty elementProperty)
        {
            string name = OnGetElementName != null ? OnGetElementName.Invoke(this, elementProperty) : GetElementName(elementProperty, ElementName);

            _elementLabel.text = name ?? elementProperty.displayName;
            _elementLabel.tooltip = elementProperty.tooltip;

            return _elementLabel;
        }

        private Object ValidateDragAndDropObject(Object[] references)
        {
            return OnValidateDragAndDrop != null
                       ? OnValidateDragAndDrop.Invoke(this, references[0])
                       : Internals.ValidateObjectDragAndDrop(references, Property);

        }

        private DragElement[] GetDragList(float dragPosition)
        {
            int i;
            int length = Property.arraySize;

            if (_dragElements == null)
            {
                _dragElements = new DragElement[length];
            }
            else if (_dragElements.Length != length)
            {
                System.Array.Resize(ref _dragElements, length);
            }

            for (i = 0; i < length; i++)
            {
                SerializedProperty elementProperty = Property.GetArrayElementAtIndex(i);
                Rect elementRect = _elementsPositions[i];

                var dragElement = new DragElement()
                {
                    Property = elementProperty,
                    DragOffset = dragPosition - elementRect.y,
                    Rect = elementRect,
                    DesiredRect = elementRect,
                    Selected = _selection.Contains(i),
                    StartIndex = i
                };

                _dragElements[i] = dragElement;
            }

            System.Array.Sort(_dragElements, delegate(DragElement a, DragElement b)
            {
                if (b.Selected)
                {
                    return a.Selected ? a.StartIndex.CompareTo(b.StartIndex) : 1;
                }

                if (a.Selected)
                {
                    return -1;
                }

                return a.StartIndex.CompareTo(b.StartIndex);
            });

            return _dragElements;
        }
    }
}
