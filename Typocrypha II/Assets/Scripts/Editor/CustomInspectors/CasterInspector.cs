using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SerializableCollections.GUIUtils;

[CustomEditor(typeof(Caster), true)]
public class CasterInspector : Editor
{
    public override void OnInspectorGUI()
    {
        var data = target as Caster;
        float togglewidth = EditorGUIUtility.labelWidth - 4;
        float refWidth = EditorGUIUtility.currentViewWidth - (EditorGUIUtility.labelWidth + 22);
        // Info GUI
        GUILayout.Label(data.DisplayName + " " + " (" + data.CasterState.ToString() + ")");
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        // Field Object Data
        data.DisplayName = EditorGUILayout.TextField(new GUIContent("Display Name"), data.DisplayName);
        data.CasterState = EditorUtils.EnumPopup(new GUIContent("Caster State"), data.CasterState);
        data.IsMoveable = EditorGUILayout.Toggle(new GUIContent("Movable"), data.IsMoveable);
        //EditorGUILayout.BeginHorizontal();
        //if (data.overrideSpawnAnim = EditorGUILayout.ToggleLeft(new GUIContent("Set Spawn Anim"), data.overrideSpawnAnim, GUILayout.Width(togglewidth)))
        //    data.spawnAnim = EditorGUILayout.ObjectField(GUIContent.none, data.spawnAnim, typeof(AnimationClip), false, GUILayout.Width(refWidth)) as AnimationClip;
        //EditorGUILayout.EndHorizontal();
        //EditorGUILayout.BeginHorizontal();
        //if (data.overrideSpawnSfx = EditorGUILayout.ToggleLeft(new GUIContent("Set Spawn Sfx"), data.overrideSpawnSfx, GUILayout.Width(togglewidth)))
        //    data.spawnSfx = EditorGUILayout.ObjectField(GUIContent.none, data.spawnSfx, typeof(AudioClip), false, GUILayout.Width(refWidth)) as AudioClip;
        //EditorGUILayout.EndHorizontal()
        EditorUtils.Separator();
        // Stat GUI
        data.Tags.RecalculateStats();
        EditorUtils.CasterUtils.CasterStatsLabelLayout(data.Stats);
        EditorUtils.Separator();
        // Tag GUI
        EditorUtils.CasterUtils.CasterTagDictionaryGUILayout(data.Tags);
        EditorUtils.SetSceneDirtyIfGUIChanged(target);
    }
}
