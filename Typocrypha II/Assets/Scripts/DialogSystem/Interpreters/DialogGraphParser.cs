using Gameflow;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogGraphParser : MonoBehaviour
{
    [SerializeField] private DialogCanvas graph = null;
    public DialogCanvas Graph { set => graph = value; }
    private BaseNode currNode = null;
    private Stack<BaseNode> recStack = new Stack<BaseNode>();
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
            //throw new System.NotImplementedException("Error");
            return null;
    }
    private BaseNode Branch(GameflowBranchNode b)
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
        return value.Trim().ToLower() == DialogParser.instance.SubstituteMacros(pattern).Trim().ToLower().Replace(".", string.Empty).Replace("?", string.Empty).Replace("!", string.Empty);
    }
    private bool CheckRegexCase(string pattern, string value)
    {
        throw new System.NotImplementedException();
    }

    /// <summary> Go through the graph, porcessing nodes until a dialog node is reached
    /// When reached, translate into a dialog item and return </summary>
    /// <param name="next">Should we immediately go to next line?
    /// i.e. if false, use current value of 'currNode'.</param>
    public DialogItem NextDialog(bool next = true)
    {
        if (next) currNode = Next();
        if (currNode == null) return null;
        if (currNode is SubCanvasNode)
        {
            var node = currNode as SubCanvasNode;
            recStack.Push(Next()); // Remember exit node.
            currNode = (node.subCanvas as DialogCanvas).getStartNode();
            return NextDialog(true);
        }
        if (currNode is GameflowEndNode)
        {
            if (recStack.Count != 0)
            {
                currNode = recStack.Pop();
                return NextDialog(false);
            }
            else if (currNode is EndAndHide)
            {         
                DialogCharacterManager.instance?.RemoveAllCharacters();
                DialogManager.instance.Hide(true, DialogManager.instance.CleanUp);
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
                DialogManager.instance.Hide(true, TransitionManager.instance.TransitionToNextScene);
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
            List<CharacterData> cds = new List<CharacterData>();
            var charNames = cNode.characterName.Split(new char[] { '|' }, new char[] { '\\' });
            List<AudioClip> voice = new List<AudioClip>();
            foreach (var charName in charNames)
            {
                // Get speaking SFX if valid name.
                var cd = DialogCharacterManager.instance.CharacterDataByName(charName.Trim());
                cds.Add(cd);
                if (cd != null)
                {
                    voice.Add(cd.talk_sfx);
                }
            } 
            // Set TIPS search.
            TIPSManager.instance.CurrSearchable = cNode.tipsData;
            // Get proper display name.
            string displayName = (cNode.displayName.Trim().Length == 0)
                               ? cNode.characterName 
                               : cNode.displayName;
            // Add to history.
            DialogHistory.instance.AddHistory(displayName, cNode.text);
            if (currNode is DialogNodeVN)
            {
                var dNode = currNode as DialogNodeVN;
                // Highlight speaking characters.
                DialogCharacterManager.instance.HighlightAllCharacters(false);
                for (int i = 0; i < cds.Count; i++)
                    if (cds[i] != null) DialogCharacterManager.instance.HighlightCharacter(cds[i], true);
                return new DialogItemVN(dNode.text, voice, displayName, dNode.mcSprite, dNode.codecSprite);
            }
            else if(currNode is DialogNodeVNPlus)
            {
                var dNode = currNode as DialogNodeVNPlus;
                return new DialogItemVNPlus(dNode.text, voice, cds, charNames);
            }
            else if(currNode is DialogNodeChat)
            {
                var dNode = currNode as DialogNodeChat;
                #region Determine Icon Side
                IconSide iconSide = IconSide.NONE;
                if (dNode.leftIcon != null)
                    iconSide = dNode.rightIcon != null ? IconSide.BOTH : IconSide.LEFT;
                else if (dNode.rightIcon != null)
                    iconSide = IconSide.RIGHT;
                #endregion
                return new DialogItemChat(dNode.text, voice, displayName, iconSide, dNode.leftIcon, dNode.rightIcon);
            }
            else if (currNode is DialogNodeAN)
            {
                var dNode = currNode as DialogNodeAN;
                return new DialogItemAN(dNode.text, voice, dNode.alignmentOptions, dNode.layoutSetting);
            }
            else if (currNode is DialogNodeBubble)
            {
                var dNode = currNode as DialogNodeBubble;
                var ditem = new DialogItemBubble(dNode.text, voice, cds, dNode.gridPosition, dNode.absolutePosition);
                return ditem;
            }    
        }
        else if (currNode is SetDialogViewNode)
        {
            var dialogViewNode = currNode as SetDialogViewNode;
            StartCoroutine(WaitOnRoutine(DialogManager.instance.SetView(dialogViewNode.ViewType)));
            return null;
        }
        else if (currNode is SetVariableNode)
        {
            var node = currNode as SetVariableNode;
            PlayerDataManager.instance.Set(node.variableName, node.value);
        }
        else if (currNode is CharacterControlNode)
        {
            var currView = DialogManager.instance.DialogView;
            if (currNode is AddCharacter addNode)
            {
                if(currView.AddCharacter(new DialogView.AddCharacterArgs(addNode.characterData, addNode.column, addNode.targetPos, addNode.initialPose, addNode.initialExpr)))
                {
                    StartCoroutine(WaitOnFunc(currView.IsReadyToContinue));
                    return null;
                }
            }
            else if(currNode is RemoveCharacter removeNode)
            {
                if (currView.RemoveCharacter(removeNode.characterData))
                {
                    StartCoroutine(WaitOnFunc(currView.IsReadyToContinue));
                    return null;
                }
            }
            else if (currNode is SetPose setPoseNode)
            {
                currView.SetPose(setPoseNode.characterData, setPoseNode.pose);
            }
            else if (currNode is SetExpression setExpressionNode)
            {
                currView.SetExpression(setExpressionNode.characterData, setExpressionNode.expr);
            }
            else if (currNode is MoveCharacter) // Deprecated
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
            else if (currNode is SetBCH) // Deprecated
            {
                var cNode = currNode as SetBCH;
                DialogCharacterManager.instance.ChangeBCH(cNode.characterData, cNode.body, cNode.clothes, cNode.hair);

            }
            else if (currNode is AnimateCharacter) // Deprecated
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
                AudioManager.instance?.PlayBGM(node.bgm, node.fadeCurve);
            }
            else if (currNode is StopBgm)
            {
                var node = currNode as StopBgm;
                AudioManager.instance?.StopBGM(node.fadeCurve);
            }
            else if (currNode is PauseBgm)
            {
                var node = currNode as PauseBgm;
                AudioManager.instance?.PauseBGM(node.pause);
            }
            else if (currNode is PlaySfx)
            {
                var node = currNode as PlaySfx;
                AudioManager.instance?.PlaySFX(node.sfx);
            }
        }
        else if (currNode is ClampDialogUINode)
        {
            var node = currNode as ClampDialogUINode;
            if (node.inOut)
                DialogManager.instance.GetComponent<Animator>().SetTrigger("ClampOut");
            else
                DialogManager.instance.GetComponent<Animator>().SetTrigger("ClampIn");
        }
        else if (currNode is FadeNode)
        {
            var node = currNode as FadeNode;
            float fadeStart = node.fadeType == FadeNode.FadeType.Fade_In ? 1f : 0f;
            float fadeEnd = 1f - fadeStart;
            StartCoroutine(FadeNode.FadeScreenOverTime(node.fadeTime, fadeStart, fadeEnd, node.fadeColor));
        }
        else if (currNode is SetLocationTextNode setLocationTextNode)
        {
            DialogManager.instance.DialogView.SetLocationText(setLocationTextNode.text);
        }
        else if (currNode is SetDateTimeTextNode setDateTimeTextNode)
        {
            DialogManager.instance.DialogView.SetDateTimeText(setDateTimeTextNode.text);
        }
        else if (currNode is ClearNode clearNode)
        {
            var clearRoutine = DialogManager.instance.DialogView.Clear();
            if(clearRoutine != null)
            {
                StartCoroutine(WaitOnRoutine(clearRoutine));
                return null;
            }
        }
        else if (currNode is CastSpellNode castNode)
        {
            var spellManager = SpellManager.instance;
            var battleField = Battlefield.instance;
            if(spellManager != null && battleField != null)
            {
                var caster = battleField.GetCaster(new Battlefield.Position(castNode.casterPos));
                if (caster != null)
                {
                    var msgOverride = string.IsNullOrEmpty(castNode.messageOverride) ? null : castNode.messageOverride;
                    StartCoroutine(WaitOnRoutine(spellManager.Cast(castNode.GetSpell(), caster, new Battlefield.Position(castNode.targetPos), msgOverride)));
                    return null;
                }
                else
                {
                    Debug.LogError($"Caster found at (row {castNode.casterPos.y}, col {castNode.casterPos.x}). Cast will be skipped");
                }
            }
        }
        else if (currNode is ClearEquippedSpellsNode)
        {
            var playerData = PlayerDataManager.instance;
            if(playerData != null)
            {
                var cooldowns = SpellCooldownManager.instance;
                var equipment = playerData.equipment;
                if (cooldowns != null && equipment != null)
                {
                    cooldowns.ClearWords();
                    equipment.EquippedWords.Clear();
                }
            }
        }
        else if(currNode is AddEquippedSpellsNode addSpellsNode)
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
                        if(word != null)
                        {
                            cooldowns.AddWord(word);
                            equipment.EquippedWords.Add(word);
                        }
                    }
                    AddWord(addSpellsNode.word1);
                    AddWord(addSpellsNode.word2);
                    AddWord(addSpellsNode.word3);
                    cooldowns.SortCooldowns();
                }
            }
        }
        // Check if need to wait on node to complete.
        if (currNode is ITimedNode)
        {
            DialogManager.instance.PH.Pause = true;
            StartCoroutine(WaitOnNode(currNode as ITimedNode));
            return null;
        }
        else
        {
            //Recursively move to next
            return NextDialog();
        }
    }

    /// <summary>
    /// Waits for a node to complete before allowing dialog to proceed (coroutine).
    /// </summary>
    /// <param name="node">Node to wait on.</param>
    IEnumerator WaitOnNode(ITimedNode node)
    {
        yield return new WaitUntil(() => node.IsCompleted);
        DialogManager.instance.PH.Pause = false;
        DialogManager.instance.NextDialog(true);
    }

    IEnumerator WaitOnFunc(System.Func<bool> isComplete)
    {
        yield return new WaitUntil(isComplete);
        DialogManager.instance.PH.Pause = false;
        DialogManager.instance.NextDialog(true);
    }

    IEnumerator WaitOnRoutine(Coroutine routine)
    {
        yield return routine;
        DialogManager.instance.PH.Pause = false;
        DialogManager.instance.NextDialog(true);
    }

    /// <summary>
    /// Skips to saved position.
    /// Only works when no branching.
    /// </summary>
    /// <param name="pos">Node (dialog node count) position to fast forward to.</param>
    public void SkipTo(int pos)
    {
        for (int i = 0; i <= pos;)
        {
            if (currNode is BaseNodeOUT)
            {
                currNode = (currNode as BaseNodeOUT).Next;
                if (currNode is DialogNode) i++;
            }
            else if (currNode is GameflowBranchNode)
            {
                currNode = Branch(currNode as GameflowBranchNode);
            }
            else throw new System.NotImplementedException("Reached end of gameflow");
        }

    }
}
