using UnityEngine;
using NodeEditorFramework;

namespace Gameflow
{
    [Node(false, "Event/Clear Spells", new System.Type[] { typeof(DialogCanvas) })]
    public class ClearEquippedSpellsNode : BaseNodeIO
    {
        #region Editor
        public override string Title { get { return "Clear Spells"; } }
        public override Vector2 MinSize { get { return new Vector2(150, 40); } }

        public const string ID = "clearSpells";
        public override string GetID { get { return ID; } }
        #endregion
    }
}
