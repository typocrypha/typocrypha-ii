﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace GUIUtils
{
    /// <summary> A class to help display lists with a custom reorderable GUI. 
    /// Currently based on UnityEditorInternal code. </summary>
    public class RListGUI<T>
    {
        public delegate void ElementGUI(T element, int index, Rect rect);
        public delegate T DefaultElement();
        public delegate GenericMenu Dropdown();
        public delegate float ElementHeight(T element, int index);
        public float Height => list.GetHeight();

        private UnityEditorInternal.ReorderableList list = null;
        private RListGUI(List<T> items, GUIContent label, ElementGUI elementGUI, ElementHeight height)
        {
            list = new UnityEditorInternal.ReorderableList(items, typeof(T), true, true, true, true);
            list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
            {
                if (index >= list.count || list.count <= 0)//Fixes error if .doGUI removes an element from the list
                return;
                elementGUI(items[index], index, rect);
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
        public RListGUI(List<T> items, GUIContent label, ElementGUI elementGUI, ElementHeight height, DefaultElement newItem) : this(items, label, elementGUI, height)
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
        public void DoLayoutList()
        {
            list.DoLayoutList();
        }
        public void DoList(Rect rect)
        {
            list.DoList(rect);
        }
    }
}
