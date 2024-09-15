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
        if(node is BadgeUnlockNode badgeUnlockNode)
        {
            foreach(var word in badgeUnlockNode.Words)
            {
                if(word != null)
                {
                    PlayerDataManager.Equipment.UnlockBadge(word);
                }
            }
        }
        if (node is ShopUnlockNode shopUnlockNode)
        {
            PlayerDataManager.instance.ShopData.UnlockShopCodes(shopUnlockNode.unlockCodes);
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
            else if (node is CrossfadeBgm crossfadeNode)
            {
                AudioManager.instance.CrossfadeBGM(crossfadeNode.bgm, crossfadeNode.fadeCurveIn, crossfadeNode.fadeCurveOut);
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
                if (cooldowns != null && PlayerDataManager.Equipment != null)
                {
                    PlayerDataManager.Equipment.ClearEquipment();
                    cooldowns.ClearWords();
                    cooldowns.InitializeEquippedWords();
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
                if (cooldowns != null && PlayerDataManager.Equipment != null)
                {
                    void AddWord(SpellWord word)
                    {
                        if (word != null)
                        {
                            cooldowns.AddWord(word);
                            PlayerDataManager.Equipment.EquipWord(word);
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
        if (node is SetAllyNode setAllyNode)
        {
            var allyManager = AllyBattleBoxManager.instance;
            if (allyManager != null)
            {
                allyManager.SetBattleAllyData(setAllyNode.allyData, setAllyNode.expr, setAllyNode.pose);
                if (setAllyNode.show)
                {
                    // show ally
                    allyManager.ShowBattleAlly();
                }
            }
            var battlefield = Battlefield.instance;
            if (battlefield != null && setAllyNode.prefab != null)
            {
                var newAlly = Instantiate(setAllyNode.prefab).GetComponent<Caster>();
                battlefield.Add(newAlly, new Battlefield.Position(1, 2));
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
