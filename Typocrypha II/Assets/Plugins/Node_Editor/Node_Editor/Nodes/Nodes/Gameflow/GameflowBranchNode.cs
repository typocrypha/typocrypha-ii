using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using NodeEditorFramework;
using NodeEditorFramework.Utilities;
using Gameflow.GUIUtilities;

namespace Gameflow
{
    [Node(false, "Gameflow/Branch", new System.Type[] { typeof(GameflowCanvas), typeof(DialogCanvas) })]
    public class GameflowBranchNode : BaseNode
    {
        public enum controlExpressionType
        {
            Variable,
            Last_Input,
        }

        public override string Title { get { return "Gameflow Branch"; } }
        public override Vector2 MinSize { get { return new Vector2(300, 40); } }

        private const string Id = "gameflowBranch";
        public override string GetID { get { return Id; } }

        //Connection from previous node (INPUT)
        [ConnectionKnob("From Previous", Direction.In, "Gameflow", ConnectionCount.Multi, NodeSide.Left, 30)]
        public ConnectionKnob fromPreviousIN;
        //Connect to default branch (OUTPUT)
        [ConnectionKnob("To Default Branch", Direction.Out, "Gameflow", ConnectionCount.Single, NodeSide.Right, 30)]
        public ConnectionKnob toDefaultBranch;

        public List<BranchCase> cases;
        private RKnobListGUI<BranchCase> listGUI;

        public controlExpressionType exprType;
        public string variableName;

        const string tooltip_branch_case = "The first case to evaluate to true (in decending order) will branch to the connected node. \nNote: macro variables may be evaluated in cases by using {varName} in text cases and \\{varName\\} in regex cases";
        const string tooltip_data = "Where to get the input string from. \"Input\" takes input from the last player input and \"Variable\" takes input from the variable with the given name";
        const string tooltip_branch_default = "The branch to take if no cases match";

        protected override void OnCreate()
        {
            cases = new List<BranchCase>();
            exprType = controlExpressionType.Last_Input;
            variableName = "variableName";
        }

        public override void NodeGUI()
        {
            GUILayout.Space(5);
            GUILayout.BeginVertical("Box");
            GUILayout.Label(new GUIContent("Data to Test", tooltip_data), NodeEditorGUI.nodeLabelBoldCentered);
            GUILayout.BeginHorizontal();
            exprType = (controlExpressionType)RTEditorGUI.EnumPopup(exprType, GUILayout.Width(75));
            if(exprType == controlExpressionType.Variable)
                variableName = GUILayout.TextField(variableName, GUILayout.Width(200f));
            GUILayout.Space(5);
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            GUILayout.BeginVertical();
            if(listGUI == null)
            {
                RKnobListGUI<BranchCase>.ElementGUI eGUI = (elt, rect) =>
                {
                    #if UNITY_EDITOR
                    float xOffset = 0;
                    Rect UIrect = new Rect(rect.x, rect.y + 2, 60, RTEditorGUI.lineHeight);
                    elt.type = (BranchCase.CaseType)EditorGUI.EnumPopup(UIrect, GUIContent.none, elt.type);
                    xOffset += (UIrect.width + 2);
                    UIrect = new Rect(rect.x + xOffset, rect.y + 1, 170, RTEditorGUI.lineHeight);
                    elt.pattern = GUI.TextField(UIrect, elt.pattern);
                    xOffset += (UIrect.width + 2);
                    UIrect = new Rect(rect.x + xOffset, rect.y + 1, 20, RTEditorGUI.lineHeight);
                    xOffset += (UIrect.width + 2);
                    UIrect = new Rect(rect.x + xOffset, rect.y + 1, 20, RTEditorGUI.lineHeight);
                    #endif
                };
                listGUI = new RKnobListGUI<BranchCase>(cases, new GUIContent("Branch Cases"), eGUI, 
                                                       (elt) => RTEditorGUI.lineHeight, (item) => new ConnectionKnob[] { item.connection },
                                                       (item, rect) => item.connection.SetPosition(rect.yMin + (NodeEditorGUI.knobSize * 2) - 3), 
                                                       () => new BranchCase(this));
            }                      
            listGUI.DoLayoutList();
            GUILayout.EndVertical();
            GUILayout.BeginVertical("Box");
            GUILayout.Label(new GUIContent("Default Branch", tooltip_branch_default), NodeEditorGUI.nodeLabelBoldCentered);
            toDefaultBranch.SetPosition();
            GUILayout.EndVertical();
        }

        protected internal override void CopyScriptableObjects(Func<ScriptableObject, ScriptableObject> replaceSO)
        {
            foreach (var c in cases)
                c.connection = replaceSO(c.connection) as ConnectionKnob;
        }

        [System.Serializable]
        public class BranchCase
        {
            public enum CaseType
            {
                Text,
                Regex
            }
            public Node NextNode { get => connection.connection(0).body; }

            public ConnectionKnob connection;
            public string pattern =  string.Empty;
            public CaseType type = CaseType.Text;

            public BranchCase(Node n)
            {
                connection = n.CreateConnectionKnob(StdKnobs.KnobAttributeOUT);
            }
        }
    }
}
