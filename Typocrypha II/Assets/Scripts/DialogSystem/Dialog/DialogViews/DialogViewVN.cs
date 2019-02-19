using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

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
        #region Check Arguments
        DialogItemVN dialogItem = data as DialogItemVN;
        if(dialogItem == null)
        {
            throw new System.Exception("Incorrect Type of dialog Item for the VN " +
                                       "view mode (requires DialogItemVN)");
        }
        #endregion
        
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
        dialogBox.StartDialogBox(dialogItem);
        return dialogBox;
    }
}
