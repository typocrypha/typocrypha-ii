using System.Collections.Generic;
using UnityEngine;
using NodeEditorFramework;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditorInternal;
#endif

namespace Gameflow
{
    namespace GUIUtilities
    {
        /// <summary> A static class contaning standard ConnectionKnobAttributes for easy reference </summary>
        public static class StdKnobs
        {
            public static readonly ConnectionKnobAttribute KnobAttributeOUT = new ConnectionKnobAttribute("OUT", Direction.Out, "Gameflow", ConnectionCount.Single, NodeSide.Right);
            public static readonly ConnectionKnobAttribute KnobAttributeIN = new ConnectionKnobAttribute("IN", Direction.In, "Gameflow", ConnectionCount.Multi, NodeSide.Left);
        }
        /// <summary> A class to help display lists with a custom reorderable GUI. 
        /// Currently based on UnityEditorInternal code. Safe for non-editor solutions </summary>
        public class RListGUI<T>
        {          
            public delegate void ElementGUI(T element, Rect rect);
            public delegate T DefaultElement();
            public delegate NodeEditorFramework.Utilities.GenericMenu Dropdown();
            public delegate float ElementHeight(T element);
            public float Height
            {
                get
                {
                    #if UNITY_EDITOR
                    return list.GetHeight();
                    #else
                    return 0;
                    #endif
                }
            }
            #if UNITY_EDITOR
            private UnityEditorInternal.ReorderableList list = null;
#endif
            private RListGUI(List<T> items, GUIContent label, ElementGUI elementGUI, ElementHeight height)
            {
#if UNITY_EDITOR
                list = new UnityEditorInternal.ReorderableList(items, typeof(T), true, true, true, true);
                list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    if (index >= list.count || list.count <= 0)//Fixes error if .doGUI removes an element from the list
                        return;
                    elementGUI(items[index], rect);
                };
                list.elementHeightCallback = (index) => { return height(items[index]); };
                list.drawHeaderCallback = (Rect rect) =>
                {
                    EditorGUI.LabelField(rect, label, new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold });
                };
                list.drawElementBackgroundCallback = (rect, index, active, focused) =>
                {
                    if (list.count <= 0)
                        return;
                    rect.height = height(items[index]);
                    Texture2D tex = new Texture2D(1, 1);
                    tex.SetPixel(0, 0, new Color(0.33f, 0.66f, 1f, 0.66f));
                    tex.Apply();
                    if (active)
                        GUI.DrawTexture(rect, tex as Texture);
                };
#endif
            }
            public RListGUI(List<T> items, GUIContent label, ElementGUI elementGUI, ElementHeight height, DefaultElement newItem) : this (items, label, elementGUI, height)
            {
                #if UNITY_EDITOR
                list.onAddCallback = (list) =>
                {
                    items.Insert(items.Count, newItem());
                };
                #endif
            }
            public RListGUI(List<T> items, GUIContent label, ElementGUI elementGUI, ElementHeight height, Dropdown menu) : this(items, label, elementGUI, height)
            {
#if UNITY_EDITOR
                list.onAddDropdownCallback = (buttonRect, list) =>
                {
                    menu().Show(Event.current.mousePosition + Event.current.delta);
                };
#endif
            }
            public void DoLayoutList()
            {
                #if UNITY_EDITOR
                list.DoLayoutList();
                #endif
            }
            public void DoList(Rect rect)
            {
                #if UNITY_EDITOR
                list.DoList(rect);
                #endif
            }
        }
        /// <summary> A class to help display lists with a custom reorderable and NodeEditor ConnectionKnobs. 
        /// Currently based on UnityEditorInternal code. Safe for non-editor solutions </summary>
        public class RKnobListGUI<T>
        {
            public delegate void ElementGUI(T element, Rect rect);
            public delegate T DefaultElement();
            public delegate NodeEditorFramework.Utilities.GenericMenu Dropdown();
            public delegate float ElementHeight(T element);
            public delegate ConnectionKnob[] ConnectionKnobs(T element);
            public delegate void RepositionKnobs(T element, Rect rect);
#if UNITY_EDITOR
            private UnityEditorInternal.ReorderableList list = null;
#endif
            private RKnobListGUI(List<T> items, GUIContent label, ElementGUI elementGUI, ElementHeight height, ConnectionKnobs knobs, RepositionKnobs reKnobs)
            {
#if UNITY_EDITOR
                list = new UnityEditorInternal.ReorderableList(items, typeof(T), true, true, true, true);
                list.drawElementCallback = (Rect rect, int index, bool isActive, bool isFocused) =>
                {
                    if (index >= list.count || list.count <= 0)//Fixes error if .doGUI removes an element from the list
                        return;
                    elementGUI(items[index], rect);
                    if (Event.current.type == EventType.Repaint)
                        reKnobs(items[index], rect);
                };
                list.elementHeightCallback = (index) => { return height(items[index]); };
                list.drawHeaderCallback = (Rect rect) =>
                {
                    EditorGUI.LabelField(rect, label, new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold });
                };
                list.drawElementBackgroundCallback = (rect, index, active, focused) =>
                {
                    if (list.count <= 0)
                        return;
                    rect.height = height(items[index]);
                    Texture2D tex = new Texture2D(1, 1);
                    tex.SetPixel(0, 0, new Color(0.33f, 0.66f, 1f, 0.66f));
                    tex.Apply();
                    if (active)
                        GUI.DrawTexture(rect, tex as Texture);
                };
                list.onRemoveCallback = (list) =>
                {
                    foreach (var knob in knobs(items[list.index]))
                        knob.body.DeleteConnectionPort(knob);
                    items.RemoveAt(list.index);
                };
#endif
            }
            public RKnobListGUI(List<T> items, GUIContent label, ElementGUI elementGUI, ElementHeight height, ConnectionKnobs knobs, RepositionKnobs reKnobs, DefaultElement newItem) : this(items, label, elementGUI, height, knobs, reKnobs)
            {
#if UNITY_EDITOR
                list.onAddCallback = (list) =>
                {
                    items.Insert(items.Count, newItem());
                };
#endif
            }
            public RKnobListGUI(List<T> items, GUIContent label, ElementGUI elementGUI, ElementHeight height, ConnectionKnobs knobs, RepositionKnobs reKnobs, Dropdown menu) : this(items, label, elementGUI, height, knobs, reKnobs)
            {
#if UNITY_EDITOR
                list.onAddDropdownCallback = (buttonRect, list) =>
                {
                    menu().Show(Event.current.mousePosition + Event.current.delta);
                };
#endif
            }
            public void DoLayoutList()
            {
#if UNITY_EDITOR
                list.DoLayoutList();
#endif
            }
            public void DoList(Rect rect)
            {
#if UNITY_EDITOR
                list.DoList(rect);
#endif
            }
        }
    }
}

