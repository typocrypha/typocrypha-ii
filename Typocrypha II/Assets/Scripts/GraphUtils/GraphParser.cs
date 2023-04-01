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

    protected bool ProcessSharedNode(Node node)
    {
        if (node is SetVariableNode setVariableNode)
        {
            PlayerDataManager.instance.Set(setVariableNode.variableName, setVariableNode.value);
            return true;
        }
        if (node is SetBackgroundNode setBgNode)
        {
            if (setBgNode.bgType == SetBackgroundNode.BgType.Sprite)
            {
                BackgroundManager.instance.SetBackground(setBgNode.bgSprite);
            }
            else
            {
                BackgroundManager.instance.SetBackground(setBgNode.bgPrefab);
            }
            return true;
        }
        if (node is SpawnPrefabNode spawnPrefabNode)
        {
            var go = Instantiate(spawnPrefabNode.prefab);
            go.transform.position = spawnPrefabNode.pos;
            return true;
        }
        // Audio Control
        if (node is AudioControlNode)
        {
            if (node is PlayBgm playBgmNode)
            {
                AudioManager.instance.PlayBGM(playBgmNode.bgm, playBgmNode.fadeCurve);
            }
            else if (node is StopBgm stopBgmNode)
            {
                AudioManager.instance.StopBGM(stopBgmNode.fadeCurve);
            }
            else if (node is PauseBgm pauseBgmNode)
            {
                AudioManager.instance.PauseBGM(pauseBgmNode.pause);
            }
            else if (node is PlaySfx playSfxNode)
            {
                AudioManager.instance.PlaySFX(playSfxNode.sfx);
            }
            return true;
        }
        // Battle Event Nodes (That also function in battle interrupts)
        if (node is ClearEquippedSpellsNode)
        {
            var playerData = PlayerDataManager.instance;
            if (playerData != null)
            {
                var cooldowns = SpellCooldownManager.instance;
                var equipment = playerData.equipment;
                if (cooldowns != null && equipment != null)
                {
                    cooldowns.ClearWords();
                    equipment.ClearEquipment();
                }
            }
            return true;
        }
        if (node is AddEquippedSpellsNode addSpellsNode)
        {
            var playerData = PlayerDataManager.instance;
            if (playerData != null)
            {
                var cooldowns = SpellCooldownManager.instance;
                var equipment = playerData.equipment;
                if (cooldowns != null && equipment != null)
                {
                    void AddWord(SpellWord word)
                    {
                        if (word != null)
                        {
                            cooldowns.AddWord(word);
                            equipment.EquipWord(word);
                        }
                    }
                    AddWord(addSpellsNode.word1);
                    AddWord(addSpellsNode.word2);
                    AddWord(addSpellsNode.word3);
                    cooldowns.SortCooldowns();
                }
            }
            return true;
        }
        return false;
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
