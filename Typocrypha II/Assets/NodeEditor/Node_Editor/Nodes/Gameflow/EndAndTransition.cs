using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using NodeEditorFramework;
using NodeEditorFramework.Utilities;
using Gameflow.GUIUtilities;

namespace Gameflow
{
    /// <summary>
    /// Transition to a new scene when dialog ends.
    /// </summary>
    [Node(false, "Gameflow/EndAndTransition", new System.Type[] { typeof(GameflowCanvas), typeof(DialogCanvas), typeof(BattleCanvas)})]
    public class EndAndTransition : GameflowEndNode
    {
        new public const string ID = "End And Transition Node";
        public override string GetID { get { return ID; } }

        public override string Title { get { return "End And Transition"; } }
        public override Vector2 MinSize { get { return new Vector2(220, 70); } }
        public override bool AutoLayout { get { return true; } }

        #region Tooltip Strings
        const string tooltipScene = "Name of next scene to load";
        const string tooltipLoadingScreen = "Loading screen prefab to use";
        #endregion

        public string nextScene; // Next scene to transition to.
        public GameObject loadingScreen; // Loading screen to use.

        public override void NodeGUI()
        {
            GUILayout.Space(3);
            #region Scene
            GUILayout.Space(5);
            GUILayout.BeginVertical("Box");
            GUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent("Scene name", tooltipScene), NodeEditorGUI.nodeLabel, GUILayout.Width(80f));
            nextScene = RTEditorGUI.TextField(nextScene, GUILayout.Width(MinSize.x - 95f));
            GUILayout.EndHorizontal();
            GUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent("Loading Screen", tooltipLoadingScreen), NodeEditorGUI.nodeLabel, GUILayout.Width(90f));
            loadingScreen = RTEditorGUI.ObjectField(loadingScreen, false, GUILayout.Width(MinSize.x - 100f));
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            #endregion
        }
    }
}
