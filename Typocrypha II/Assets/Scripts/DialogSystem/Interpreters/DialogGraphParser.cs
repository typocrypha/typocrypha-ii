using Gameflow;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class DialogGraphParser : GraphParser
{
    [SerializeField] private DialogCanvas graph = null;
    public DialogCanvas Graph { set => graph = value; }
    private Stack<BaseNode> recStack = new Stack<BaseNode>();
    /// <summary> Initialized the root node (for if next dialogue is called in DialogManager's awake function </summary>
    public void Init()
    {
        currNode = graph.GetStartNode();
    }

    /// <summary> Go through the graph, porcessing nodes until a dialog node is reached
    /// When reached, translate into a dialog item and return </summary>
    /// <param name="next">Should we immediately go to next line?
    /// i.e. if false, use current value of 'currNode'.</param>
    public DialogItem NextDialog(bool next, bool loading)
    {
        if (next) currNode = Next();
        if (currNode == null || (loading && !currNode.ExecuteDuringLoading)) 
            return null;
        // Use shared functionality if currNode is a shared node
        bool isSharedNode = ProcessSharedNode(currNode);
        // If not a shared node, use dialog functionality
        if (!isSharedNode)
        {
            if (currNode is SubCanvasNode)
            {
                var node = currNode as SubCanvasNode;
                recStack.Push(Next()); // Remember exit node.
                currNode = (node.subCanvas as DialogCanvas).GetStartNode();
                return NextDialog(true, loading);
            }
            if (currNode is GameflowEndNode)
            {
                if (recStack.Count != 0)
                {
                    currNode = recStack.Pop();
                    return NextDialog(false, loading);
                }
                else if (currNode is EndAndHide)
                {
                    DialogCharacterManager.instance?.RemoveAllCharacters();
                    DialogManager.instance.Hide(DialogManager.EndType.DialogEnd, DialogManager.instance.CleanUp);
                    return null;
                }
                else if (currNode is EndAndGoto) // Immediately start new dialog graph.
                {
                    var node = currNode as EndAndGoto;
                    Graph = node.nextDialog;
                    Init();
                    return NextDialog(true, loading);
                }
                else if (currNode is EndAndTransition) // Transitions scenes.
                {
                    DialogManager.instance.Hide(DialogManager.EndType.SceneEnd, TransitionManager.instance.TransitionToNextScene);
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
                else if (currNode is DialogNodeVNPlus)
                {
                    var dNode = currNode as DialogNodeVNPlus;
                    return new DialogItemVNPlus(dNode.text, voice, cds, charNames);
                }
                else if (currNode is DialogNodeChat)
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
                else if(currNode is DialogNodeLocation locationNode)
                {
                    return new DialogItemLocation(locationNode.text, voice);
                }
            }
            else if (currNode is SetDialogViewNode)
            {
                var dialogViewNode = currNode as SetDialogViewNode;
                StartCoroutine(WaitOnRoutine(DialogManager.instance.SetView(dialogViewNode.ViewType), loading));
                return null;
            }
            else if (currNode is CharacterControlNode)
            {
                var currView = DialogManager.instance.DialogView;
                if (currNode is AddCharacter addNode)
                {
                    if (currView.AddCharacter(new DialogView.AddCharacterArgs(addNode.characterData, addNode.column, addNode.targetPos, addNode.initialPose, addNode.initialExpr)))
                    {
                        StartCoroutine(WaitOnFunc(currView.IsReadyToContinue, loading));
                        return null;
                    }
                }
                else if (currNode is RemoveCharacter removeNode)
                {
                    if (currView.RemoveCharacter(removeNode.characterData))
                    {
                        StartCoroutine(WaitOnFunc(currView.IsReadyToContinue, loading));
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
                else if (currNode is MoveCharacter moveNode)
                {
                    if (currView is DialogViewVNPlus vnPlusView)
                    {
                        var moveRoutine = vnPlusView.MoveCharacter(moveNode.characterData, moveNode.targetColumn, moveNode.top);
                        if (moveRoutine != null)
                        {
                            StartCoroutine(WaitOnRoutine(moveRoutine, loading));
                            return null;
                        }
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
            else if (currNode is ClampDialogUINode)
            {
                var node = currNode as ClampDialogUINode;
                if (node.inOut)
                    DialogManager.instance.GetComponent<Animator>().SetTrigger("ClampOut");
                else
                    DialogManager.instance.GetComponent<Animator>().SetTrigger("ClampIn");
            }
            else if (currNode is MoveCameraNode camNode)
            {
                var bounds = BackgroundManager.instance.GetBounds();
                var tween = CameraManager.instance.MoveToPivot(bounds, camNode.StartPivot, camNode.FinalPivot, camNode.Duration, camNode.EasingCurve);
                StartCoroutine(WaitOnTween(tween, loading));
                return null;
            }
            else if (currNode is SetCameraNode setCamNode)
            {
                var bounds = BackgroundManager.instance.GetBounds();
                CameraManager.instance.SetPivot(bounds, setCamNode.Pivot);
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
                DialogManager.instance.LocationText = setLocationTextNode.text;
            }
            else if (currNode is SetDateTimeTextNode setDateTimeTextNode)
            {
                DialogManager.instance.DialogView.SetDateTimeText(setDateTimeTextNode.text);
            }
            else if (currNode is ClearNode clearNode)
            {
                var clearRoutine = DialogManager.instance.DialogView.Clear();
                if (clearRoutine != null)
                {
                    StartCoroutine(WaitOnRoutine(clearRoutine, loading));
                    return null;
                }
            }
            else if (currNode is PauseNode pauseNode)
            {
                StartCoroutine(WaitOnSeconds(pauseNode.WaitTime, loading));
                return null;
            }
            else if (currNode is CastSpellNode castNode)
            {
                var spellManager = SpellManager.instance;
                var battleField = Battlefield.instance;
                if (spellManager != null && battleField != null)
                {
                    Caster caster;
                    if (!string.IsNullOrEmpty(castNode.proxyCasterName))
                    {
                        if (castNode.searchField)
                        {
                            caster = battleField.GetCaster(castNode.proxyCasterName, true);
                        }
                        else
                        {
                            caster = battleField.GetProxyCaster(castNode.proxyCasterName);
                        }
                    }
                    else
                    {
                        caster = battleField.GetCaster(new Battlefield.Position(castNode.casterPos));
                    }         
                    if (caster != null)
                    {
                        var msgOverride = string.IsNullOrEmpty(castNode.messageOverride) ? null : castNode.messageOverride;
                        StartCoroutine(WaitOnRoutine(spellManager.Cast(castNode.GetSpell(), caster, new Battlefield.Position(castNode.targetPos), msgOverride), loading));
                        return null;
                    }
                    else if (!string.IsNullOrEmpty(castNode.proxyCasterName))
                    {
                        Debug.LogError($"Proxy caster {castNode.proxyCasterName} not found.Cast will be skipped");
                    }
                    else
                    {
                        Debug.LogError($"Caster not found at (row {castNode.casterPos.y}, col {castNode.casterPos.x}). Cast will be skipped");
                    }
                }
            }
            else if (currNode is ClearReinforcementsNode clearReinforcementsNode)
            {
                BattleManager.instance.ClearReinforcements();
            }
        }
        // Check if need to wait on node to complete.
        if (currNode is ITimedNode)
        {
            StartCoroutine(WaitOnNode(currNode as ITimedNode, loading));
            return null;
        }
        else
        {
            //Recursively move to next
            return NextDialog(true, loading);
        }
    }

    /// <summary>
    /// Waits for a node to complete before allowing dialog to proceed (coroutine).
    /// </summary>
    /// <param name="node">Node to wait on.</param>
    IEnumerator WaitOnNode(ITimedNode node, bool loading)
    {
        DialogManager.instance.ReadyToContinue = false;
        yield return new WaitUntil(() => node.IsCompleted);
        DialogManager.instance.ReadyToContinue = true;
        DialogManager.instance.NextDialog(true, loading);
    }

    IEnumerator WaitOnFunc(System.Func<bool> isComplete, bool loading)
    {
        DialogManager.instance.ReadyToContinue = false;
        yield return new WaitUntil(isComplete);
        DialogManager.instance.ReadyToContinue = true;
        DialogManager.instance.NextDialog(true, loading);
    }

    IEnumerator WaitOnRoutine(Coroutine routine, bool loading)
    {
        DialogManager.instance.ReadyToContinue = false;
        yield return routine;
        DialogManager.instance.ReadyToContinue = true;
        DialogManager.instance.NextDialog(true, loading);
    }

    IEnumerator WaitOnTween(Tween tween, bool loading)
    {
        DialogManager.instance.ReadyToContinue = false;
        yield return tween.WaitForCompletion();
        DialogManager.instance.ReadyToContinue = true;
        DialogManager.instance.NextDialog(true, loading);
    }

    IEnumerator WaitOnSeconds(float seconds, bool loading)
    {
        DialogManager.instance.ReadyToContinue = false;
        yield return new WaitForSeconds(seconds);
        DialogManager.instance.ReadyToContinue = true;
        DialogManager.instance.NextDialog(true, loading);
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
