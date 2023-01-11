using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Standard dialog view (visual novel style).
/// </summary>
public class DialogViewVN : DialogView
{
    private DialogBox dialogBox; // Visual novel style dialogue box
    private Text nameText; // Text that contains speaker's name for VN style
    private SpriteRenderer mcSprite; // Holds mc's sprite
    private SpriteRenderer codecSprite; // Holds codec call sprites (right side)

    private void Awake()
    {
        GameObject obj = Instantiate(dialogBoxPrefab, transform, false);
        dialogBox = obj.GetComponent<DialogBox>();
        nameText = obj.transform.Find("NameText").GetComponent<Text>();
        mcSprite = obj.transform.Find("PlayerSprite").GetComponent<SpriteRenderer>();
        codecSprite = obj.transform.Find("CodecSprite").GetComponent<SpriteRenderer>();
    }

    public override void SetEnabled(bool e)
    {
        gameObject.SetActive(e);
    }

    public override DialogBox PlayDialog(DialogItem data)
    {
        if (!IsDialogItemCorrectType(data, out DialogItemVN dialogItem))
            return null;
        
        // Set new dialog parameters
        nameText.text = dialogItem.speakerName;
        if(dialogItem.mcSprite != null)
        {
            mcSprite.sprite = dialogItem.mcSprite;
        }
        if (dialogItem.codecSprite != null)
        {
            codecSprite.sprite = dialogItem.codecSprite;
        }
        dialogBox.SetupAndStartDialogBox(dialogItem);
        return dialogBox;
    }

    public override void CleanUp()
    {
        // Does nothing.
    }

    public override bool AddCharacter(AddCharacterArgs args)
    {
        DialogCharacterManager.instance.AddCharacter(args.CharacterData, args.AbsolutePosition);
        return false;
    }

    public override bool RemoveCharacter(CharacterData data)
    {
        DialogCharacterManager.instance.RemoveCharacter(data);
        return false;
    }

    public override void SetExpression(CharacterData data, string expression)
    {
        DialogCharacterManager.instance.ChangeExpression(data, expression);
    }

    public override void SetPose(CharacterData data, string pose)
    {
        DialogCharacterManager.instance.ChangePose(data, pose);
    }
}
