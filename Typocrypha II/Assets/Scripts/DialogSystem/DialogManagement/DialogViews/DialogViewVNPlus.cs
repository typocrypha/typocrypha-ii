using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
using System.Linq;

public class DialogViewVNPlus : DialogView
{
    private const float tweenTime = 0.5f;

    public enum CharacterColumn
    {
        Right,
        Left,
    }

    [SerializeField] private GameObject rightDialogBoxPrefab;
    [SerializeField] private GameObject leftDialogBoxPrefab;
    [SerializeField] private RectTransform messageContainer;
    [SerializeField] private VerticalLayoutGroup messageLayout;
    [SerializeField] private Ease messageLayoutEase;
    [SerializeField] private bool useCustomMessageLayoutEase;
    [SerializeField] private AnimationCurve customMessageLayoutEase;
    [SerializeField] private GameObject rightCharacterPrefab;
    [SerializeField] private GameObject leftCharacterPrefab;
    [SerializeField] private RectTransform rightCharacterContainer;
    [SerializeField] private RectTransform leftCharacterContainer;


    public override bool ReadyToContinue => readyToContinue;

    private bool readyToContinue = false;
    private Tween tween;

    private readonly Dictionary<string, VNPlusCharacter> characterMap = new Dictionary<string, VNPlusCharacter>(4);

    public override void CleanUp()
    {

    }

    #region Character Control

    public void AddCharacter(CharacterData data, CharacterColumn column)
    {
        if(!characterMap.ContainsKey(data.name))// Scene character
        {
            VNPlusCharacter newCharacter;
            if (column == CharacterColumn.Right)
            {
                newCharacter = Instantiate(rightCharacterPrefab, rightCharacterContainer).GetComponent<VNPlusCharacter>();
            }
            else
            {
                newCharacter = Instantiate(leftCharacterPrefab, leftCharacterContainer).GetComponent<VNPlusCharacter>();
            }
            newCharacter.Data = data;
            characterMap.Add(data.name, newCharacter);
        }
    }

    public void SetExpression(CharacterData data, string expression)
    {
        if (characterMap.ContainsKey(data.name))// Scene character
        {
            characterMap[data.name].SetExpression(expression);
        }
    }

    public void SetPose(CharacterData data, string pose)
    {
        if (characterMap.ContainsKey(data.name))// Scene character
        {
            characterMap[data.name].SetPose(pose);
        }
    }

    /// <summary>
    /// Turns highlight on or off on a character.
    /// </summary>
    /// <param name="data">Id of selected character.</param>
    /// <param name="on">Whether to turn on highlight.</param>
    /// <returns>DialogCharacter component of selected character.</returns>
    public void HighlightCharacter(IEnumerable<CharacterData> data)
    {
        foreach(var kvp in characterMap)
        {
            kvp.Value.Highlighted = data.Any(d => d.name == kvp.Key);
        }
    }

    #endregion

    public override DialogBox PlayDialog(DialogItem data)
    {
        if (!IsDialogItemCorrectType(data, out DialogItemVNPlus dialogItem))
            return null;
        HighlightCharacter(dialogItem.CharacterData);
        var prefab = dialogItem.IsLeft ? leftDialogBoxPrefab : rightDialogBoxPrefab;
        var dialogBox = Instantiate(prefab, messageContainer).GetComponent<DialogBox>();
        readyToContinue = false;
        StartCoroutine(AnimateNewMessageIn(dialogBox, dialogItem));
        return dialogBox;
    }

    private IEnumerator AnimateNewMessageIn(DialogBox box, DialogItem item)
    {
        box.SetupDialogBox(item);
        yield return null;
        box.SetBoxHeight();
        tween?.Complete();
        tween = messageContainer.DOAnchorPosY(messageContainer.anchoredPosition.y + (box.GetBoxHeight() + messageLayout.spacing), tweenTime);
        // Play animation
        if (useCustomMessageLayoutEase)
        {
            tween.SetEase(customMessageLayoutEase);
        }
        else
        {
            tween.SetEase(messageLayoutEase);
        }
        readyToContinue = true;
        box.StartDialogScroll();
        yield break;
    }

    public override void SetEnabled(bool e)
    {
        gameObject.SetActive(e);
    }
}
