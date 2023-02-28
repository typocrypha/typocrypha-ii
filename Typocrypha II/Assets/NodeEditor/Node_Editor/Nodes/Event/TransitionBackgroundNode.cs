using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeEditorFramework;
using NodeEditorFramework.Utilities;

namespace Gameflow
{
    /// <summary>
    /// Transitions background with effect.
    /// Currently unimplemented
    /// </summary>
    [Node(false, "Event/Transition Background", new System.Type[] { typeof(GameflowCanvas), typeof(DialogCanvas) })]
    public class TransitionBackgroundNode : BaseNodeIO
    {
        #region Editor
        public enum BgType
        {
            Sprite,
            Prefab,
        }
        public override string Title { get { return "Transition Background"; } }
        public override Vector2 MinSize { get { return new Vector2(150, 40); } }

        private const string Id = "transitionBackground";
        public override string GetID { get { return Id; } }

        public BgType bgType;
        public Sprite bgSprite;
        public GameObject bgPrefab;
        public GameObject transition;

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
            transition = RTEditorGUI.ObjectField(transition, false);
        }
        #endregion
    }
}

