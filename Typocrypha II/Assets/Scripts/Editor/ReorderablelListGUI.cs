using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace GUIUtils
{
    /// <summary> A class to help display lists with a custom reorderable GUI.
    /// Currently based on UnityEditorInternal code. </summary>
    public class RListGUI<T>
    {
        public delegate void ElementGUI(T element, int index, int selectedInd, Rect rect);
        public delegate void RemoveElement(T element, int index);
        public delegate T DefaultElement();
        public delegate GenericMenu Dropdown();
        public delegate float ElementHeight(T element, int index);
        public float Height => list.GetHeight();

        private UnityEditorInternal.ReorderableList list = null;

        #region Constructors
        private RListGUI(List<T> items, GUIContent label, ElementGUI elementGUI, ElementHeight height)
        {
            list = new UnityEditorInternal.ReorderableList(items, typeof(T), true, true, true, true);
            list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                if (index >= list.count || list.count <= 0)//Fixes error if .doGUI removes an element from the list
                    return;
                elementGUI(items[index], index, list.index, rect);
            };
            list.elementHeightCallback = (index) => { return height(items[index], index); };
            list.drawHeaderCallback = (Rect rect) =>
            {
                EditorGUI.LabelField(rect, label, new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold });
            };
            list.drawElementBackgroundCallback = (rect, index, active, focused) =>
            {
                if (list.count <= 0)
                    return;
                rect.height = height(items[index], index);
                Texture2D tex = new Texture2D(1, 1);
                tex.SetPixel(0, 0, new Color(0.33f, 0.66f, 1f, 0.66f));
                tex.Apply();
                if (active)
                    GUI.DrawTexture(rect, tex as Texture);
            };
        }
        private RListGUI(List<T> items, GUIContent label, ElementGUI elementGUI, ElementHeight height, RemoveElement remove) : this(items, label, elementGUI, height)
        {
            list.onRemoveCallback = (list) =>
            {
                var element = items[list.index];
                items.RemoveAt(list.index);
                remove(element, list.index);
            };
        }
        public RListGUI(List<T> items, GUIContent label, ElementGUI elementGUI, ElementHeight height, DefaultElement newItem) : this(items, label, elementGUI, height)
        {
            list.onAddCallback = (list) =>
            {
                items.Insert(items.Count, newItem());
            };
        }
        public RListGUI(List<T> items, GUIContent label, ElementGUI elementGUI, ElementHeight height, RemoveElement remove, DefaultElement newItem) : this(items, label, elementGUI, height, remove)
        {
            list.onAddCallback = (list) =>
            {
                items.Insert(items.Count, newItem());
            };
        }
        public RListGUI(List<T> items, GUIContent label, ElementGUI elementGUI, ElementHeight height, Dropdown menu) : this(items, label, elementGUI, height)
        {
            list.onAddDropdownCallback = (buttonRect, list) =>
            {
                menu().DropDown(buttonRect);
            };
        }
        public RListGUI(List<T> items, GUIContent label, ElementGUI elementGUI, ElementHeight height, RemoveElement remove, Dropdown menu) : this(items, label, elementGUI, height, remove)
        {
            list.onAddDropdownCallback = (buttonRect, list) =>
            {
                menu().DropDown(buttonRect);
            };
        }
        #endregion

        public void SetSelected(int index)
        {
            list.index = index;
        }
        public void DoLayoutList()
        {
            list.DoLayoutList();
        }
        public void DoList(Rect rect)
        {
            list.DoList(rect);
        }
    }

    public class RListGUIProperty
    {
        public delegate void ElementGUI(SerializedProperty element, int index, Rect rect);
        public delegate GenericMenu Dropdown();
        public float Height => list.GetHeight() + EditorGUIUtility.singleLineHeight;

        private UnityEditorInternal.ReorderableList list = null;
        public RListGUIProperty(SerializedProperty prop, GUIContent label)
        {
            list = new UnityEditorInternal.ReorderableList(prop.serializedObject, prop, true, true, true, true);
            list.drawHeaderCallback = (Rect rect) =>
            {
                EditorGUI.LabelField(rect, label, new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold });
            };
            list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                if (index >= list.count || list.count <= 0)//Fixes error if .doGUI removes an element from the list
                    return;
                EditorGUI.PropertyField(rect, prop.GetArrayElementAtIndex(index), GUIContent.none);
                prop.serializedObject.ApplyModifiedProperties();
            };
        }

        public void DoList(Rect rect)
        {
            list.DoList(rect);
        }
    }
}
