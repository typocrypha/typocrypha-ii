using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Experimental.SceneManagement;

public static partial class EditorUtils
{
    public static void Slider()
    {
        EditorGUILayout.LabelField("", GUI.skin.horizontalSlider);
    }
    public static T EnumPopup<T>(GUIContent label, T enumData, params GUILayoutOption[] options) where T : System.Enum
    {
        return (T)EditorGUILayout.EnumPopup(label, enumData, options);
    }
    public static void SetSceneDirtyIfGUIChanged(Object target)
    {
        if (GUI.changed)
        {
            if (!EditorApplication.isPlayingOrWillChangePlaymode)
                EditorSceneManager.MarkSceneDirty(EditorSceneManager.GetActiveScene());
            var prefabStage = PrefabStageUtility.GetCurrentPrefabStage();
            if (prefabStage != null)
            {
                EditorSceneManager.MarkSceneDirty(prefabStage.scene);
            }
        }
    }
}
