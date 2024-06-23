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
        else if (currNode is VictoryScreenNode victoryNode)
        {
            BattleManager.instance.Victory(victoryNode);
            return null;
        }
        else if(currNode is VictoryScreenUnlockNode unlockMessageNode)
        {
            RewardsManager.Instance.AddBonusEntry(unlockMessageNode.name, unlockMessageNode.reasonText, unlockMessageNode.descriptionText, unlockMessageNode.priority, unlockMessageNode.clarkeMessage);
            return NextWave();
        }
        else if (currNode is BattleNodeWave waveNode)
        {
            return new BattleWave()
            {
                waveTitle = waveNode.waveTitle,
                waveNumberOverride = waveNode.waveNumberOverride,
                music = waveNode.music,
                battleField = new GOMatrix2D(waveNode.battleField),
                battleEvents = new List<GameObject>(waveNode.battleEvents),
                fieldOptions = waveNode.clearFieldOptions,
                reinforcementPrefabs = new List<GameObject>(waveNode.reinforcements),
                useStdEvents = waveNode.useStandardEvents,
                openingScene = waveNode.openingScene,
                allowEquipment = waveNode.allowEquipement,
            };
        }
        //Recursively move to next
        return NextWave();
    }
}
