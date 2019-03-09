using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gameflow;

public class DialogGraphParser : MonoBehaviour
{
    [SerializeField] private DialogCanvas graph = null;
    public DialogCanvas Graph { set => graph = value; }
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
        string value = string.Empty;
        if (b.exprType == GameflowBranchNode.controlExpressionType.Last_Input)
            value = PlayerDataManager.instance[PlayerDataManager.lastInputKey].ToString();
        else
            value = PlayerDataManager.instance[b.variableName].ToString();
        foreach (var brCase in b.cases)
        {
            if (brCase.type == GameflowBranchNode.BranchCase.CaseType.Regex)
            {
                if (CheckRegexCase(brCase.pattern, value))
                    return brCase.connection.connections[0].body as BaseNode;
            }
            else if (CheckTextCase(brCase.pattern, value))//brCase.type == BranchCaseData.CaseType.Text
                return brCase.connection.connections[0].body as BaseNode;
        }
        return b.toDefaultBranch.connection(0).body as BaseNode;
    }

    private bool CheckTextCase(string pattern, string value)
    {
        //Probably should compress this to regex
        return value.Trim().ToLower() == DialogParser.instance.SubstituteMacros(pattern).Trim().ToLower().Replace(".", string.Empty).Replace("?", string.Empty).Replace("!", string.Empty);
    }
    private bool CheckRegexCase(string pattern, string value)
    {
        throw new System.NotImplementedException();
    }

    /// <summary> Go through the graph, porcessing nodes until a dialog node is reached
    /// When reached, translate into a dialog item and return </summary>
    public DialogItem NextDialog()
    {
        currNode = Next();
        if (currNode is GameflowEndNode)
        {
            if (currNode is EndAndHide)
            {
                DialogManager.instance.Display(false);
                return null;
            }
            else if (currNode is EndAndGoto) // Immediately start new dialog graph.
            {
                var node = currNode as EndAndGoto;
                Graph = node.nextDialog;
                Init();
                return NextDialog();
            }
            else if (currNode is EndAndTransition) // Transitions scenes.
            {
                var node = currNode as EndAndTransition;
                TransitionManager.instance.TransitionScene(node.nextScene, node.loadingScreen);
                return null;
            }
        }
        if (currNode is DialogNodeInput) // If input, set up input manager
        {
            var iNode = currNode as DialogNodeInput;
            DialogInputManager.instance.EnableInput(new DialogInputItem(iNode.variableName));
        }
        if (currNode is DialogNode)
        {
            var cNode = currNode as DialogNode;
            // Get speaking SFX if valid name.
            var cd = DialogCharacterManager.instance.CharacterDataByName(cNode.characterName);
            AudioClip voice = null;
            if (cd != null) voice = cd.talk_sfx;
            // Set TIPS search.
            TIPSManager.instance.CurrSearchable = cNode.tipsData;
            if (currNode is DialogNodeVN)
            {
                var dNode = currNode as DialogNodeVN;
                // Highlight speaking character.
                if (cd != null) DialogCharacterManager.instance.SoloHighlightCharacter(cd);
                return new DialogItemVN(dNode.text, voice, dNode.characterName, dNode.mcSprite, dNode.codecSprite);
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
                return new DialogItemChat(dNode.text, voice, dNode.characterName, iconSide, dNode.leftIcon, dNode.rightIcon);
            }
            if (currNode is DialogNodeAN)
                return new DialogItemAN((currNode as DialogNodeAN).text, voice);
        }
        else if (currNode is SetVariableNode)
        {
            var node = currNode as SetVariableNode;
            PlayerDataManager.instance[node.variableName] = node.value;
        }
        else if (currNode is CharacterControlNode)
        {
            if (currNode is AddCharacter)
            {
                var cNode = currNode as AddCharacter;
                DialogCharacterManager.instance.AddCharacter(cNode.characterData, cNode.targetPos);
            }
            else if (currNode is RemoveCharacter)
            {
                var cNode = currNode as RemoveCharacter;
                DialogCharacterManager.instance.RemoveCharacter(cNode.characterData);
            }
            else if (currNode is MoveCharacter)
            {
                var cNode = currNode as MoveCharacter;
                if (cNode.movementType == CharacterMovementType.Teleport)
                {
                    DialogCharacterManager.instance.TeleportCharacter(cNode.characterData, cNode.targetPos);
                }
                else if (cNode.movementType == CharacterMovementType.Lerp)
                {
                    DialogCharacterManager.instance.LerpCharacter(cNode.characterData, cNode.targetPos, cNode.time);
                }
                else if (cNode.movementType == CharacterMovementType.SmoothDamp)
                {
                    DialogCharacterManager.instance.SmoothDampCharacter(cNode.characterData, cNode.targetPos, cNode.time);
                }
            }
            else if (currNode is SetPose)
            {
                var cNode = currNode as SetPose;
                DialogCharacterManager.instance.ChangePose(cNode.characterData, cNode.pose);
            }
            else if (currNode is SetExpression)
            {
                var cNode = currNode as SetExpression;
                DialogCharacterManager.instance.ChangeExpression(cNode.characterData, cNode.expr);
            }
            else if (currNode is AnimateCharacter)
            {
                var cNode = currNode as AnimateCharacter;
                DialogCharacterManager.instance.AnimateCharacter(cNode.characterData, cNode.clip);
            }
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
        return NextDialog();
    }
}
