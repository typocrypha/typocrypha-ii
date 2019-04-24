using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SerializableCollections.GUIUtils;

[CustomEditor(typeof(CasterTag))]
public class CasterTagInspector : Editor
{
    private bool viewCumulative = true;
    public override void OnInspectorGUI()
    {
        CasterTag tag = target as CasterTag;
        EditorGUILayout.LabelField("Caster Tag: " + tag.name + " (" + tag.displayName + ")");
        EditorUtils.Separator();
        tag.displayName = EditorGUILayout.TextField(new GUIContent("Display Name"), tag.displayName);
        EditorUtils.Separator();
        if (tag.subTags.Count > 0)
        {
            viewCumulative = EditorGUILayout.ToggleLeft("Cumulative Stats", viewCumulative);
            if(viewCumulative)
            {
                var statMod = new CasterStats();
                statMod.AddInPlace(tag.statMods);
                foreach (var mod in tag.subTags)
                    statMod.AddInPlace(mod.statMods);
                EditorUtils.CasterUtils.CasterStatsLabelLayout(statMod);

            }
        }
        

        EditorUtils.Separator();
        EditorGUILayout.LabelField("Stat Modifiers", new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter });
        if (tag.statMods != null)
            EditorUtils.CasterUtils.CasterStatsGUILayout(tag.statMods);
        EditorUtils.Separator();
        if (tag.reactions != null)
        {
            SDictionaryGUI.ValueGUI<Reaction> valGUI = (r) => EditorUtils.EnumPopup(GUIContent.none, r, GUILayout.MaxWidth(120));
            tag.reactions.DoGUILayout(valGUI, () => tag.reactions.ObjPickerAddGUI(), "Reactions", true);
        }
        EditorGUILayout.Space();
        EditorUtils.Separator();
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
