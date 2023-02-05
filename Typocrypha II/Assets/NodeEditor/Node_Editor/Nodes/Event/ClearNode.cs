using UnityEngine;
using NodeEditorFramework;

namespace Gameflow
{
    [Node(false, "Event/Clear", new System.Type[] { typeof(DialogCanvas) })]
    public class ClearNode : BaseNodeIO
    {
        #region Editor
        public override string Title { get { return "Clear"; } }
        public override Vector2 MinSize { get { return new Vector2(150, 40); } }

        public const string ID = "clear";
        public override string GetID { get { return ID; } }
        #endregion
    }
}
