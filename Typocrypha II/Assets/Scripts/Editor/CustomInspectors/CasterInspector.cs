using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SerializableCollections.GUIUtils;

[CanEditMultipleObjects]
[CustomEditor(typeof(Caster), true)]
public class CasterInspector : Editor
{
    public override void OnInspectorGUI()
    {
        var data = target as Caster;
        float togglewidth = EditorGUIUtility.labelWidth - 4;
        float refWidth = EditorGUIUtility.currentViewWidth - (EditorGUIUtility.labelWidth + 22);

        #region Info GUI
        GUILayout.Label(data.DisplayName + " " + " (" + data.CasterState.ToString() + ")");
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
        #endregion

        #region Field Object Data GUI
        data.DisplayName = EditorGUILayout.TextField(new GUIContent("Display Name"), data.DisplayName);
        data.CasterState = EditorUtils.EnumPopup(new GUIContent("Caster State"), data.CasterState);
        data.IsMoveable = EditorGUILayout.Toggle(new GUIContent("Movable"), data.IsMoveable);
        #endregion

        #region Spawning GUI
        //EditorGUILayout.BeginHorizontal();
        //if (data.overrideSpawnAnim = EditorGUILayout.ToggleLeft(new GUIContent("Set Spawn Anim"), data.overrideSpawnAnim, GUILayout.Width(togglewidth)))
        //    data.spawnAnim = EditorGUILayout.ObjectField(GUIContent.none, data.spawnAnim, typeof(AnimationClip), false, GUILayout.Width(refWidth)) as AnimationClip;
        //EditorGUILayout.EndHorizontal();
        //EditorGUILayout.BeginHorizontal();
        //if (data.overrideSpawnSfx = EditorGUILayout.ToggleLeft(new GUIContent("Set Spawn Sfx"), data.overrideSpawnSfx, GUILayout.Width(togglewidth)))
        //    data.spawnSfx = EditorGUILayout.ObjectField(GUIContent.none, data.spawnSfx, typeof(AudioClip), false, GUILayout.Width(refWidth)) as AudioClip;
        //EditorGUILayout.EndHorizontal()
        EditorUtils.Separator();
        #endregion

        #region Stat GUI
        data.Tags.RecalculateStats();
        EditorUtils.CasterUtils.CasterStatsLabelLayout(data.Stats);
        EditorUtils.Separator();
        #endregion

        #region Playmode GUI
        if(EditorApplication.isPlaying)
        {
            EditorGUILayout.LabelField(new GUIContent("Status: " + data.BStatus));
            EditorGUILayout.LabelField(new GUIContent("Health: " + data.Health));          
            EditorGUILayout.LabelField(new GUIContent("Armor: " + data.Armor));
            EditorGUILayout.LabelField(new GUIContent("SP: " + data.SP));
            EditorGUILayout.LabelField(new GUIContent("Stagger: " + data.Stagger));            
        }
        #endregion

        // Tag GUI
        EditorUtils.CasterUtils.CasterTagDictionaryGUILayout(data.Tags);

        EditorUtils.SetSceneDirtyIfGUIChanged(target);
    }
}
