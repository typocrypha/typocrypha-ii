using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeEditorFramework;

namespace Gameflow
{
    [Node(true, "Gameflow/GameflowBase", new System.Type[] { typeof(GameflowCanvas) })]
    public abstract class BaseNode : Node
    {
        public override bool AutoLayout { get { return true; } }
        public enum ProcessFlag
        {
            Continue,
            Wait,
        }
    }
    public class GameflowType : ConnectionKnobStyle //: IConnectionTypeDeclaration
    {
        public override string Identifier { get { return "Gameflow"; } }
        public override Color Color { get { return Color.white; } }
    }
}
