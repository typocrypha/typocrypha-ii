using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeEditorFramework;
using NodeEditorFramework.Utilities;

namespace Gameflow
{
    /// <summary>
    /// Applies a single animation clip to a character.
    /// </summary>
    [Node(false, "Event/Character/AnimateCharacter", new System.Type[] { typeof(GameflowCanvas), typeof(DialogCanvas) })]
    public class AnimateCharacter : CharacterControlNode
    {
        public const string ID = "Animate Character Node";
        public override string GetID { get { return ID; } }

        public override string Title { get { return "Animate Character"; } }
        public override Vector2 MinSize { get { return new Vector2(250, 60); } }

        public AnimationClip clip; // Animation clip to play.

        #region Tooltip Strings
        protected const string tooltipClip = "Animation clip to use.";
        #endregion

        protected override void OnCreate()
        {
            characterData = null;
            clip = null;
        }

        public override void NodeGUI()
        {
            base.NodeGUI();

            #region Expressions
            GUILayout.Label(new GUIContent("Animation Clip", tooltipClip), NodeEditorGUI.nodeLabelBoldCentered);
            GUILayout.BeginHorizontal();
            GUILayout.Space(5);
            clip = RTEditorGUI.ObjectField(clip, false, GUILayout.Width(MinSize.x - 10));
            GUILayout.Space(5);
            GUILayout.EndHorizontal();
            #endregion
        }
    }
}

