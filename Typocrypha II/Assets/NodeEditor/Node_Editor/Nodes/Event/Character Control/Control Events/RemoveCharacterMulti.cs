using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeEditorFramework;

namespace Gameflow
{
    [Node(false, "Event/Character/Remove Character (Multi)", new System.Type[] { typeof(DialogCanvas) })]
    public class RemoveCharacterMulti : MultiCharacterControlNode
    {
        public const string ID = "Remove Character Node Multi";
        public override string GetID => ID;

        public override string Title => "Remove Characters";
        public override Vector2 MinSize => new Vector2(250, 60);
    }
}
