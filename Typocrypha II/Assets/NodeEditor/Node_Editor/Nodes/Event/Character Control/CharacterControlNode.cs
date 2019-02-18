using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NodeEditorFramework;

using System;
using Gameflow.GUIUtilities;

namespace Gameflow
{
    [Node(true, "Event/Character/Character Control Base", new System.Type[] { typeof(GameflowCanvas), typeof(DialogCanvas) })]
    public abstract class CharacterControlNode : BaseNodeIO
    {
        public override bool AllowRecursion { get { return true; } }

        public CharacterData characterData;
    }
}
