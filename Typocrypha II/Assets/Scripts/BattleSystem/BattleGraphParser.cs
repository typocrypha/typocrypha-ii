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
        currNode = graph.getStartNode();
        return currNode as BattleStartNode;
    }
    /// <summary> Go through the graph, porcessing nodes until a dialog node is reached
    /// When reached, translate into a dialog item and return </summary>
    public BattleWave NextWave()
    {
        currNode = Next();
        if (currNode == null) return null;
        if (currNode is GameflowEndNode)
        {
            if (currNode is EndAndHide)
            {
                DialogManager.instance.Display(false, null);
                return null;
            }
            // NON-FUNCTIONAL WITH BATTLES (CHECK WITH JAMES)
            //else if (currNode is EndAndGoto) // Immediately start new dialog graph.
            //{
            //    var node = currNode as EndAndGoto;
            //    Graph = node.nextDialog;
            //    Init();
            //    return NextDialog();
            //}
            else if (currNode is EndAndTransition) // Transitions scenes.
            {
                var node = currNode as EndAndTransition;
                TransitionManager.instance.TransitionScene(node.nextScene, node.loadingScreen);
                return null;
            }
        }
        else if (currNode is BattleNodeWave)
        {
            var node = currNode as BattleNodeWave;
            return new BattleWave()
            {
                waveTitle = node.waveTitle,
                music = node.music,
                battleField = new GOMatrix2D(node.battleField),
                battleEvents = new List<GameObject>(node.battleEvents),
                fieldOptions = node.clearFieldOptions,
                reinforcementPrefabs = new List<GameObject>(node.reinforcements),
            };
        }
        else if (currNode is SetVariableNode)
        {
            var node = currNode as SetVariableNode;
            PlayerDataManager.instance.Set(node.variableName, node.value);
        }
        else if (currNode is SetBackgroundNode)
        {
            var node = currNode as SetBackgroundNode;
            if (node.bgType == SetBackgroundNode.BgType.Sprite)
            {
                BackgroundManager.instance.SetBackground(node.bgSprite);
            }
            else
            {
                BackgroundManager.instance.SetBackground(node.bgPrefab);
            }
        }
        else if (currNode is SpawnPrefabNode)
        {
            var node = currNode as SpawnPrefabNode;
            var go = Instantiate(node.prefab);
            go.transform.position = node.pos;
        }
        else if (currNode is AudioControlNode)
        {
            if (currNode is PlayBgm)
            {
                var node = currNode as PlayBgm;
                AudioManager.instance.PlayBGM(node.bgm, node.fadeCurve);
            }
            else if (currNode is StopBgm)
            {
                var node = currNode as StopBgm;
                AudioManager.instance.StopBGM(node.fadeCurve);
            }
            else if (currNode is PauseBgm)
            {
                var node = currNode as PauseBgm;
                AudioManager.instance.PauseBGM(node.pause);
            }
            else if (currNode is PlaySfx)
            {
                var node = currNode as PlaySfx;
                AudioManager.instance.PlaySFX(node.sfx);
            }
        }
        //Process other node types
        //Recursively move to next
        return NextWave();
    }
}
