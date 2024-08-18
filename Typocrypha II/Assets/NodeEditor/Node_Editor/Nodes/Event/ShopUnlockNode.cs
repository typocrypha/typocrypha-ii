using NodeEditorFramework;
using NodeEditorFramework.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gameflow
{
    [Node(false, "Event/Shop Unlock", typeof(BattleCanvas), typeof(DialogCanvas))]
    public class ShopUnlockNode : BaseNodeIO
    {
        private const string id = "ShopUnlockNode";
        public override string GetID => id;

        public override string Title => "Shop Unlock";
        public override Vector2 MinSize => new Vector2(150, 40);

        public BadgeWord.ShopUnlockCodes unlockCodes;

        public override void NodeGUI()
        {
            GUILayout.Space(4);
            GUILayout.BeginVertical("Box");

            unlockCodes = (BadgeWord.ShopUnlockCodes)RTEditorGUI.EnumFlagsPopup(unlockCodes);

            //Don't know why this code needs to be here exactly, but it makes everything nicer? maybe add to some static stuff?
            GUILayout.BeginHorizontal();
            GUILayout.EndHorizontal();
        }
    }
}
