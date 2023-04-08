using UnityEngine;
using NodeEditorFramework;

namespace Gameflow
{
    [Node(false, "Event/Clear Reinforcments", new System.Type[] { typeof(DialogCanvas) })]
    public class ClearReinforcementsNode : BaseNodeIO
    {
        #region Editor
        public override string Title { get { return "Clear Reinforcements"; } }
        public override Vector2 MinSize { get { return new Vector2(150, 40); } }

        public const string ID = "clearReinforcements";
        public override string GetID { get { return ID; } }
        #endregion
    }
}
