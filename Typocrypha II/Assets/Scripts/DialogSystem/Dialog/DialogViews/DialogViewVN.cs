using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogViewVN : DialogView
{
    public GameObject dialogBoxPrefab;
    public SpriteRenderer spacebar;
    public Animator spacebarAnimator;

    private DialogBox dialogBox; // Visual novel style dialogue box
    private Text nameText; // Text that contains speaker's name for VN style
    private SpriteRenderer mcSprite; // Holds mc's sprite
    private SpriteRenderer codecSprite; // Holds codec call sprites (right side)

    private void Awake()
    {
        GameObject obj = GameObject.Instantiate(dialogBoxPrefab, transform, false);
        dialogBox = obj.GetComponent<DialogBox>();
        nameText = obj.transform.Find("NameText").GetComponent<Text>();
        mcSprite = obj.transform.Find("PlayerSprite").GetComponent<SpriteRenderer>();
        codecSprite = obj.transform.Find("CodecSprite").GetComponent<SpriteRenderer>();
    }

    public override DialogBox NewDialog(DialogItem data)
    {
        #region Check Arguments
        DialogItemVN item = data as DialogItemVN;
        if(item == null)
        {
            throw new System.Exception("Incorrect Type of dialog Item for the VN view mode (requires DialogItemVN)");
        }
        #endregion

        nameText.text = item.speakerName;
        if(item.mcSprite != null)
        {
            mcSprite.sprite = item.mcSprite;
        }
        if (item.codecSprite != null)
        {
            codecSprite.sprite = item.codecSprite;
        }
        dialogBox.StartDialogBox(item);
        return dialogBox;
    }

    public override void SetEnabled(bool e)
    {
        gameObject.SetActive(e);
    }
}
