using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NodeEditorFramework;

namespace Gameflow
{
    public abstract class BaseNode : Node
    {
        public override bool AutoLayout => true;
        public virtual bool ExecuteDuringLoading => false;
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
