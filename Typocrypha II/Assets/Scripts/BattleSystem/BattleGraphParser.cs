using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gameflow;

public class BattleGraphParser : GraphParser
{
    [SerializeField] private BattleCanvas graph;
    public BattleCanvas Graph { set => graph = value; }
    /// <summary> Initialized the root node (for if next dialogue is called in BattleManager's awake function </summary>
    public BattleStartNode Init()
    {
        currNode = graph.GetStartNode();
        return currNode as BattleStartNode;
    }
    /// <summary> Go through the graph, porcessing nodes until a dialog node is reached
    /// When reached, translate into a dialog item and return </summary>
    public BattleWave NextWave()
    {
        currNode = Next();
        if (currNode == null) return null;
        // If shared node, go to next
        if (ProcessSharedNode(currNode))
        {
            return NextWave();
        }
        // Battle graph-specific functionality
        if (currNode is GameflowEndNode)
        {
            // Transition to next scene regardless of which end node is used
            TransitionManager.instance.TransitionToNextScene();
            return null;
        }
        else if (currNode is BattleNodeWave)
        {
            var node = currNode as BattleNodeWave;
            return new BattleWave()
            {
                waveTitle = node.waveTitle,
                waveNumberOverride = node.waveNumberOverride,
                music = node.music,
                battleField = new GOMatrix2D(node.battleField),
                battleEvents = new List<GameObject>(node.battleEvents),
                fieldOptions = node.clearFieldOptions,
                reinforcementPrefabs = new List<GameObject>(node.reinforcements),
            };
        }
        //Recursively move to next
        return NextWave();
    }
}
