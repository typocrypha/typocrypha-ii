using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gameflow;

public class DialogGraphParser : MonoBehaviour
{
    [SerializeField] private DialogCanvas graph = null;
    private BaseNode currNode = null;
    /// <summary> Initialized the root node (for if next dialogue is called in DialogManager's awake function </summary>
    public void Init()
    {
        currNode = graph.getStartNode();
    }
    private BaseNode Next()
    {
        if (currNode is BaseNodeOUT)
            return (currNode as BaseNodeOUT).Next;
        else if (currNode is GameflowBranchNode)
            return Branch(currNode as GameflowBranchNode);
        else
            throw new System.NotImplementedException("Reached end of gameflow");
    }
    private BaseNode Branch(GameflowBranchNode b)
    {
        return b.toDefaultBranch.connections[0].body as BaseNode;
    }
    /// <summary> Go through the graph, porcessing nodes until a dialog node is reached
    /// When reached, translate into a dialog item and return </summary>
    public DialogItem NextDialog()
    {
        currNode = Next();
        if (currNode is DialogNode)
        {
            if(currNode is DialogNodeVN)
            {
                var dNode = currNode as DialogNodeVN;
                return new DialogItemVN(dNode.text, dNode.characterName, dNode.mcSprite, dNode.codecSprite);
            }
            if(currNode is DialogNodeChat)
            {
                var dNode = currNode as DialogNodeChat;
                #region Determine Icon Side
                IconSide iconSide = IconSide.NONE;
                if (dNode.leftIcon != null)
                    iconSide = dNode.rightIcon != null ? IconSide.BOTH : IconSide.LEFT;
                else if (dNode.rightIcon != null)
                    iconSide = IconSide.RIGHT;
                #endregion
                return new DialogItemChat(dNode.text, dNode.characterName, iconSide, dNode.leftIcon, dNode.rightIcon);
            }
            if (currNode is DialogNodeAN)
                return new DialogItemAN((currNode as DialogNodeAN).text);
        }
        //Process other node types
        //Recursively move to next
        return NextDialog();
    }
}
