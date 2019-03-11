using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeEditorFramework;
using NodeEditorFramework.Utilities;

namespace Gameflow
{
    /// <summary>
    /// Spawns a prefab at location. 
    /// </summary>
    [Node(false, "Event/Spawn Prefab", new System.Type[] { typeof(GameflowCanvas), typeof(DialogCanvas) })]
    public class SpawnPrefabNode : BaseNodeIO
    {
        public override string Title { get { return "Spawn Prefab"; } }
        public override Vector2 MinSize { get { return new Vector2(250, 60); } }

        private const string Id = "Spawn Prefab Node";
        public override string GetID { get { return Id; } }

        public GameObject prefab; // Prefab to spawn.
        public Vector2 pos; // Where to spawn prefab.

        protected const string tooltipPos = "Where to position prefab";

        protected override void OnCreate()
        {
            prefab = null;
            pos = Vector2.zero;
        }

        public override void NodeGUI()
        {
            GUILayout.Space(5);
            #region Prefab
            GUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent("Prefab", tooltipPos));
            prefab = RTEditorGUI.ObjectField(prefab, false, GUILayout.Width(MinSize.x - 65));
            GUILayout.EndHorizontal();
            #endregion

            #region Position
            GUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent("Position", tooltipPos));
            pos = RTEditorGUI.Vector2Field(new Rect(60, 25, MinSize.x - 65, 20), "", pos);
            GUILayout.EndHorizontal();
            #endregion
        }
    }
}

