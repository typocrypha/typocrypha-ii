using UnityEngine;
using NodeEditorFramework;
using NodeEditorFramework.Utilities;

namespace Gameflow
{
    public abstract class MultiCharacterControlNode : BaseNodeIO
    {
        public const int MaxArgs = 3;
        public override bool AllowRecursion => true;

        public CharacterData characterData1;
        public CharacterData characterData2;
        public CharacterData characterData3;

        #region Tooltip Strings
        protected const string tooltipData = "The selected characters's CharacterData";
        #endregion

        public override void NodeGUI()
        {
            #region Data
            GUILayout.Space(5);
            GUILayout.BeginVertical("Box");
            GUILayout.Label(new GUIContent("Characters", tooltipData), NodeEditorGUI.nodeLabel, GUILayout.Width(65f));
            characterData1 = RTEditorGUI.ObjectField(characterData1, false, GUILayout.Width(MinSize.x - 100f));
            characterData2 = RTEditorGUI.ObjectField(characterData2, false, GUILayout.Width(MinSize.x - 100f));
            characterData3 = RTEditorGUI.ObjectField(characterData3, false, GUILayout.Width(MinSize.x - 100f));
            GUILayout.EndVertical();
            #endregion
        }
    }
}
