using UnityEngine;
using NodeEditorFramework;
using NodeEditorFramework.Utilities;

namespace Gameflow
{
    [Node(false, "Dialog/VN Dialog", new System.Type[] { typeof(GameflowCanvas), typeof(DialogCanvas) })]
    public class DialogNodeVN : DialogNode
    {
        #region Editor
        public const string ID = "VN Dialog Node";
        public override string GetID { get { return ID; } }

        public override string Title { get { return "VN Dialog"; } }
        public override Vector2 MinSize { get { return new Vector2(250, 60); } }

        #region Ayin + Codec Debug Fields
        public Sprite mcSprite;
        public Sprite codecSprite;
        public GameObject mcPrefab;
        public GameObject codecPrefab;
        private bool showSpriteOptions = false;
        #endregion

        #region Tooltip Strings
        private const string tooltip_expr = "The speaking character's expression. Only needs to be set if the expression should be changed";
        #endregion

        public override void NodeGUI()
        {
            NameGUI();
            TextGUI();

            //GUILayout.Label(new GUIContent("Dialog Text", tooltip_text), NodeEditorGUI.nodeLabelBoldCentered);
            //GUILayout.BeginHorizontal();
            ////GUILayout.BeginVertical(GUILayout.Width(65f));
            ////GUILayout.Label(new GUIContent("Ayin", "TODO:Tooltip"), NodeEditorGUI.nodeLabelBoldCentered, GUILayout.Width(65f));
            ////mcSprite = RTEditorGUI.ObjectField(mcSprite, false, GUILayout.Width(65f), GUILayout.Height(65f));
            ////GUILayout.EndVertical();
            //GUIStyle dialogTextStyle = new GUIStyle(GUI.skin.textArea);
            //dialogTextStyle.wordWrap = true;
            //text = GUILayout.TextArea(text, GUI.skin.textArea, GUILayout.MinHeight(RTEditorGUI.lineHeight * 5));
            ////GUILayout.BeginVertical(GUILayout.Width(65f));
            ////GUILayout.Label(new GUIContent("Codec", "TODO:Tooltip"), NodeEditorGUI.nodeLabelBoldCentered, GUILayout.Width(65f));
            ////codecSprite = RTEditorGUI.ObjectField(codecSprite, false, GUILayout.Width(65f), GUILayout.Height(65f));
            ////GUILayout.EndVertical();
            //GUILayout.EndHorizontal();

            #region Ayin and Codec Sprites
            showSpriteOptions = GUILayout.Toggle(showSpriteOptions, new GUIContent("Show Debug Sprite Options"));
            if(showSpriteOptions)
            {
                GUILayout.BeginVertical("Box");
                GUILayout.Space(3);
                GUILayout.Label(new GUIContent(" Ayin      || Sprites ||      Codec", "TODO:Tooltip"), NodeEditorGUI.nodeLabelBoldCentered);
                GUILayout.BeginHorizontal();
                GUILayout.Space(10);
                mcSprite = RTEditorGUI.ObjectField(mcSprite, false, GUILayout.Width(65f), GUILayout.Height(65f));
                GUILayout.Space(MinSize.x - 170);
                codecSprite = RTEditorGUI.ObjectField(codecSprite, false, GUILayout.Width(65f), GUILayout.Height(65f));
                GUILayout.Space(10);
                GUILayout.EndHorizontal();
                GUILayout.Space(3);
                mcPrefab = RTEditorGUI.ObjectField(new GUIContent("Ayin Prefab"), mcPrefab, false);
                codecPrefab = RTEditorGUI.ObjectField(new GUIContent("Codec Prefab"), codecPrefab, false);
                GUILayout.EndVertical();
            }
            #endregion

            //Don't know why this code needs to be here exactly, but it makes everything nicer? maybe add to some static stuff?
            GUILayout.BeginHorizontal();
            RTEditorGUI.labelWidth = 90;
            GUILayout.EndHorizontal();
        }
        #endregion

    }
}
