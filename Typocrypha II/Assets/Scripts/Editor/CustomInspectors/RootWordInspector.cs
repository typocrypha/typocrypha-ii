using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using GUIUtils;

[CustomEditor(typeof(RootWord))]
[CanEditMultipleObjects]
public class RootWordInspector : SpellWordInspector
{

    private RListGUI<RootWordEffect> rList;
    private void OnEnable()
    {
        var word = target as RootWord;
        RListGUI<RootWordEffect>.Dropdown getMenu = () =>
        {
            var menu = new GenericMenu();
            foreach (var type in ReflectionUtils.ReflectiveEnumerator.GetAllSubclassTypes<RootWordEffect>())
            {
                string[] path = type.ToString().Split('.');
                var name = path[path.Length - 1];
                menu.AddItem(new GUIContent(name), false, (obj) =>
                {
                    var newItem = ScriptableObject.CreateInstance((System.Type)obj) as RootWordEffect;
                    newItem.hideFlags = HideFlags.HideInHierarchy;
                    word.effects.Add(newItem);
                    AssetDatabase.AddObjectToAsset(newItem, target);
                }
                , type);
            }
            return menu;
        };
        RListGUI<RootWordEffect>.ElementGUI eGUI = (elem, ind, sel, rect) =>
        {
            EditorGUI.LabelField(rect, new GUIContent(elem.ToString() + " (" + ind + ")"));
            if (ind == sel)
            {
                var editor = Editor.CreateEditor(elem);
                EditorGUILayout.BeginVertical("box");
                var selectLabel = new GUIContent("Selected Effect: " + elem.ToString() + " (" + ind + ")");
                EditorGUILayout.LabelField(selectLabel, new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold});
                EditorUtils.Separator();
                editor.OnInspectorGUI();
                EditorGUILayout.EndVertical();
            }
        };
        RListGUI<RootWordEffect>.ElementHeight height = (elem, ind) =>
        {
            return EditorGUIUtility.singleLineHeight;
        };
        rList = new RListGUI<RootWordEffect>(word.effects, new GUIContent("Effects"), eGUI, height,
                                            (elem, ind) => DestroyImmediate(elem, true), getMenu);
        if (word.effects.Count > 0)
            rList.SetSelected(0);
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        var word = target as RootWord;
        EditorGUILayout.BeginHorizontal();
        GUILayout.Label(new GUIContent("Cooldown"), new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold });
        word.cooldown = EditorGUILayout.FloatField(word.cooldown);
        EditorGUILayout.EndHorizontal();
        EditorUtils.Separator();
        rList.DoLayoutList();
        if (GUI.changed)
            EditorUtility.SetDirty(this);
    }
}
