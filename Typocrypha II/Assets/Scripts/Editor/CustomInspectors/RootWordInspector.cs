using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using GUIUtils;

[CustomEditor(typeof(RootWord))]
public class RootWordInspector : Editor
{

    private GUIUtils.RListGUI<RootWordEffect> rList;
    private void OnEnable()
    {
        var word = target as RootWord;
        var arrProp = serializedObject.FindProperty("effects");
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
                    //newItem.hideFlags = HideFlags.HideInHierarchy;
                    word.effects.Add(newItem);
                    serializedObject.Update();
                    AssetDatabase.AddObjectToAsset(newItem, target);
                }
                , type);
            }
            return menu;
        };
        rList = new RListGUI<RootWordEffect>(word.effects, new GUIContent("Effects"),
                                            (elem, ind, rect) => EditorGUI.PropertyField(rect, arrProp.GetArrayElementAtIndex(ind), GUIContent.none),
                                            (elem, ind) => EditorGUI.GetPropertyHeight(arrProp.GetArrayElementAtIndex(ind)),
                                            getMenu);
    }

    public override void OnInspectorGUI()
    {
        var word = target as RootWord;
        EditorGUILayout.LabelField("Root Word: " + word.name);
        EditorGUILayout.TextField(new GUIContent("Display Name"), word.displayName);
        EditorUtils.Separator();
        GUILayout.Label(new GUIContent("Description"), new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter, fontStyle = FontStyle.Bold });
        word.description = EditorGUILayout.TextArea(word.description, new GUIStyle(GUI.skin.textArea) { wordWrap = true }, GUILayout.MinHeight(EditorGUIUtility.singleLineHeight * 2));
        EditorUtils.Separator();
        rList.DoLayoutList();
    }
}
