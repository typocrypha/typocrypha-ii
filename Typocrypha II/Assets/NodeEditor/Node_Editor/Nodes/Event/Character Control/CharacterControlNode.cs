using UnityEngine;
using NodeEditorFramework;
using NodeEditorFramework.Utilities;

namespace Gameflow
{
    public abstract class CharacterControlNode : BaseNodeIO
    {
        public override bool AllowRecursion { get { return true; } }

        public CharacterData characterData;

        #region Tooltip Strings
        protected const string tooltipData = "The selected character's CharacterData";
        #endregion

        public override void NodeGUI()
        {
            #region Data
            GUILayout.Space(5);
            GUILayout.BeginVertical("Box");
            GUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent("Character", tooltipData), NodeEditorGUI.nodeLabel, GUILayout.Width(65f));
            characterData = RTEditorGUI.ObjectField(characterData, false, GUILayout.Width(MinSize.x - 100f));
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            #endregion
        }
    }
}
