using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeEditorFramework;
using NodeEditorFramework.Utilities;

namespace Gameflow
{
    public enum CharacterMovementType
    {
        Teleport, // Immediately move character.
        Lerp, // Linearly move character.
        SmoothDamp // Smoothly move character.
    }
    [Node(false, "Event/Character/Move Character", new System.Type[] { typeof(GameflowCanvas), typeof(DialogCanvas) })]
    public class MoveCharacter : CharacterControlNode
    {
        public const string ID = "Move Character Node";
        public override string GetID { get { return ID; } }

        public override string Title { get { return "Move Character"; } }
        public override Vector2 MinSize { get { return new Vector2(250, 60); } }

        public CharacterMovementType movementType = CharacterMovementType.Teleport; // Type of movement.
        public Vector2 targetPos = new Vector2(0, 0); // Target position.
        public float time = 1f; // Time to get to target (used by Lerp mode).

        #region Tooltip Strings
        protected const string tooltipType = "Type of movement";
        protected const string tooltipPos = "Where to position character (center pivot)";
        protected const string tooltipTime = "Amount of time to complete movement";
        #endregion

        protected override void OnCreate()
        {
            characterData = null;
            targetPos = Vector2.zero;
        }

        public override void NodeGUI()
        {
            base.NodeGUI();

            #region Type
            GUILayout.BeginHorizontal();
            GUILayout.Space(5);
            GUILayout.Label(new GUIContent("Movement Type", tooltipType));
            movementType = (CharacterMovementType)RTEditorGUI.EnumPopup(movementType, GUILayout.Width(100f));
            GUILayout.Space(5);
            GUILayout.EndHorizontal();
            #endregion

            #region Position
            GUILayout.BeginHorizontal();
            GUILayout.Label(new GUIContent("Position", tooltipPos));
            targetPos = RTEditorGUI.Vector2Field(new Rect(60, 50, MinSize.x - 65, 20), "", targetPos);
            GUILayout.EndHorizontal();
            #endregion

            #region Lerp
            if (movementType == CharacterMovementType.Lerp ||
                movementType == CharacterMovementType.SmoothDamp)
            {
                GUILayout.Space(5);
                GUILayout.BeginHorizontal();
                GUILayout.Label(new GUIContent("Time", tooltipTime));
                time = RTEditorGUI.FloatField(time, GUILayout.Width(MinSize.x - 50));
                GUILayout.Space(5);
                GUILayout.EndHorizontal();
            }
            #endregion
        }

    }

}
