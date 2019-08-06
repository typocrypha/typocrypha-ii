using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SerializableCollections.GUIUtils;

[CustomEditor(typeof(CasterTag))]
[CanEditMultipleObjects]
public class CasterTagInspector : Editor
{
    private bool viewCumulative = true;
    private bool viewInfo = false;
    public override void OnInspectorGUI()
    {
        CasterTag tag = target as CasterTag;
        viewInfo = EditorGUILayout.ToggleLeft("View Name and Description Info", viewInfo, EditorUtils.Bold);
        if(viewInfo)
        {
            tag.internalName = EditorGUILayout.TextField(new GUIContent("Internal Name"), tag.internalName);
            if (GUILayout.Button("Set internal name to asset name"))
            {
                foreach (var t in targets)
                {
                    var tag2 = t as CasterTag;
                    if (tag2.internalName != tag2.name)
                    {
                        tag2.internalName = tag2.name;
                        EditorUtility.SetDirty(tag2);
                    }
                }
            }
            tag.displayName = EditorGUILayout.TextField(new GUIContent("Display Name"), tag.displayName);
            var areaStyle = new GUIStyle(GUI.skin.textArea) { wordWrap = true };
            EditorGUILayout.LabelField(new GUIContent("Description"), EditorUtils.BoldCentered);
            tag.description = EditorGUILayout.TextArea(tag.description, areaStyle, GUILayout.MinHeight(EditorGUIUtility.singleLineHeight * 2)); ; ;
            EditorGUILayout.LabelField(new GUIContent("Documentation"), EditorUtils.BoldCentered);
            tag.documentation = EditorGUILayout.TextArea(tag.documentation, areaStyle, GUILayout.MinHeight(EditorGUIUtility.singleLineHeight * 2));
        }
        EditorUtils.Separator();

        // Cumulative Stats
        if (tag.subTags != null && tag.subTags.Count > 0)
        {
            viewCumulative = EditorGUILayout.ToggleLeft("View Cumulative Stats",viewCumulative, EditorUtils.Bold);
            if(viewCumulative)
            {
                var statMod = new CasterStats();
                statMod.AddInPlace(tag.statMods);
                foreach (var mod in tag.subTags)
                    statMod.AddInPlace(mod.statMods);
                EditorUtils.CasterUtils.CasterStatsLabelLayout(statMod);
            }
            EditorUtils.Separator();
        }
        // Stat Modifiers
        EditorGUILayout.LabelField("Stat Modifiers", new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter });
        if (tag.statMods != null)
            EditorUtils.CasterUtils.CasterStatsGUILayout(tag.statMods);
        EditorUtils.Separator();

        // Abilities
        EditorGUILayout.PropertyField(serializedObject.FindProperty("ability1"));
        EditorGUILayout.PropertyField(serializedObject.FindProperty("ability2"));
        EditorUtils.Separator();

        // Reactions
        if (tag.reactions != null)
        {
            Reaction valGUI(Reaction r) => EditorUtils.EnumPopup(GUIContent.none, r, GUILayout.MaxWidth(100));
            void kGUI(SpellTag s) => EditorGUILayout.LabelField(s.internalName, GUILayout.MaxWidth(100));
            tag.reactions.DoGUILayout(kGUI, valGUI, () => tag.reactions.ObjPickerAddGUI(), "Reactions", true);
            EditorUtility.SetDirty(tag);
        }
        EditorGUILayout.Space();
        EditorUtils.Separator();

        // SubTags
        if (tag.subTags != null)
        {
            tag.subTags.DoGUILayout(EditorUtils.CasterUtils.CasterTagGUI, () => tag.subTags.ObjPickerAddGUI(), "SubTags");
            if (tag.subTags.Contains(tag))
                tag.subTags.Remove(tag);
        }
        if (GUI.changed)
            EditorUtility.SetDirty(target);
    }
}
