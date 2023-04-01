using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gameflow;
using NodeEditorFramework;

public abstract class GraphParser : MonoBehaviour
{
    protected BaseNode currNode = null;

    protected virtual BaseNode Next()
    {
        if (currNode is BaseNodeOUT outNode)
        {
            return outNode.Next;
        }
        else if (currNode is GameflowBranchNode branchNode)
        {
            return Branch(branchNode);
        }
        return null;
    }
    protected virtual BaseNode Branch(GameflowBranchNode b)
    {
        string value = string.Empty;
        if (b.exprType == GameflowBranchNode.controlExpressionType.Last_Input)
        {
            value = PlayerDataManager.instance.GetObj(PlayerDataManager.lastInputKey).ToString();
        }
        else
        {
            value = PlayerDataManager.instance.GetObj(b.variableName).ToString();
        }
        foreach (var brCase in b.cases)
        {
            if (brCase.type == GameflowBranchNode.BranchCase.CaseType.Regex)
            {
                if (CheckRegexCase(brCase.pattern, value))
                    return brCase.connection.connections[0].body as BaseNode;
            }
            else if (CheckTextCase(brCase.pattern, value))//brCase.type == BranchCaseData.CaseType.Text
            {
                return brCase.connection.connections[0].body as BaseNode;
            }
        }
        return b.toDefaultBranch.connection(0).body as BaseNode;
    }

    private bool CheckTextCase(string pattern, string value)
    {
        //Probably should compress this to regex
        return value.Trim().ToLower() == TextMacros.SubstituteMacros(pattern).Trim().ToLower().Replace(".", string.Empty).Replace("?", string.Empty).Replace("!", string.Empty);
    }
    private bool CheckRegexCase(string pattern, string value)
    {
        throw new System.NotImplementedException();
    }
}
