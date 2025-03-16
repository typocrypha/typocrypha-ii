using UnityEngine;
using NodeEditorFramework;

namespace Gameflow
{
    [Node(false, "Event/Trigger Battle Event", new System.Type[] { typeof(DialogCanvas) })]
    public class TriggerBattleEventNode : BaseNodeIO
    {
        #region Editor
        public override string Title => "Trigger Battle Event";
        public override Vector2 MinSize => new Vector2(150, 40);

        public const string ID = "triggerBattleEvent";
        public override string GetID { get { return ID; } }
        #endregion

        public string battleEventId;

        public override void NodeGUI()
        {
            GUILayout.Space(5);
            GUILayout.BeginVertical("box");
            GUILayout.Label(new GUIContent("Battle Event ID"), NodeEditorGUI.nodeLabelBoldCentered);
            battleEventId = GUILayout.TextField(battleEventId);
            GUILayout.EndVertical();
        }

        protected override void OnCreate()
        {
            battleEventId = string.Empty;
        }
    }
}
