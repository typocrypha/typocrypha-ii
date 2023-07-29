using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeEditorFramework;
using NodeEditorFramework.Utilities;

namespace Gameflow
{
    [Node(false, "Event/Set Background", new System.Type[] { typeof(DialogCanvas), typeof(BattleCanvas) })]
    public class SetBackgroundNode : BaseNodeIO
    {
        #region Editor
        public enum BgType
        {
            Sprite,
            Prefab,
        }
        public override string Title { get { return "Set Background"; } }
        public override Vector2 MinSize { get { return new Vector2(150, 40); } }

        public const string ID = "setBackground";
        public override string GetID { get { return ID; } }

        public BgType bgType;
        public Sprite bgSprite;
        public GameObject bgPrefab;

        protected override void OnCreate()
        {
            bgType = BgType.Sprite;
        }

        public override void NodeGUI()
        {
            GUILayout.BeginVertical("box");
            bgType = (BgType)RTEditorGUI.EnumPopup(bgType);
            if (bgType == BgType.Sprite)
                bgSprite = RTEditorGUI.ObjectField(bgSprite, false);
            else
                bgPrefab = RTEditorGUI.ObjectField(bgPrefab, false);
            GUILayout.EndVertical();
        }
        #endregion
    }
}
