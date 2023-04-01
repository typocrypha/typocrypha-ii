using NodeEditorFramework;

namespace Gameflow
{
    [Node(true, "Battle/BattleBase", new System.Type[] { typeof(BattleCanvas) })]
    public abstract class BattleNode : BaseNodeIO
    {
        public override bool AllowRecursion { get { return true; } }
    }
}