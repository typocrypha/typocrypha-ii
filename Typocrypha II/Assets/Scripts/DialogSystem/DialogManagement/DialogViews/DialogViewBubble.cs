using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Dialog view for positionable text bubbles.
/// </summary>
public class DialogViewBubble : DialogView
{
    List<GameObject> old = new List<GameObject>(); // Previous dialog boxes.
    DialogBox_Multi multiBox; // Interface for multiple boxes.

    private void Awake()
    {
        multiBox = gameObject.AddComponent<DialogBox_Multi>();
    }

    public override void SetEnabled(bool e)
    {
        gameObject.SetActive(e);
    }

    /// <summary>
    /// Create and start single or multiple speech bubbles.
    /// </summary>
    public override DialogBox PlayDialog(DialogItem data)
    {
        RemoveOld();
        if (data is DialogItemBubble_Multi)
        {
            List<DialogBox> boxes = new List<DialogBox>();
            foreach(var bdata in (data as DialogItemBubble_Multi).bubbleList)
            {
                boxes.Add(PlayDialog(bdata as DialogItemBubble));
            }
            multiBox.boxes = boxes;
            return multiBox;
        }
        else return PlayDialog(data as DialogItemBubble);
    }

    DialogBox PlayDialog(DialogItemBubble dialogItem)
    {
        var obj = Instantiate(dialogBoxPrefab, transform);
        old.Add(obj);
        // Absolute Pixel Positioning (Top Left Origin)
        //obj.GetComponent<RectTransform>().anchoredPosition = dialogItem.rect.position;
        //obj.GetComponent<RectTransform>().sizeDelta = new Vector2(dialogItem.rect.width, dialogItem.rect.height);
        // Normalized World Positioning (Top Left)
        Vector2 res = new Vector2(Screen.width, Screen.height);
        obj.GetComponent<RectTransform>().anchoredPosition = dialogItem.rect.position * res;
        obj.GetComponent<RectTransform>().sizeDelta = 
            new Vector2(dialogItem.rect.width, dialogItem.rect.height) * res;
        var dialogBox = obj.GetComponent<DialogBox>();
        dialogBox.SetupAndStartDialogBox(dialogItem);
        return dialogBox;
    }

    /// <summary>
    /// Remove old speech bubbles.
    /// </summary>
    public void RemoveOld()
    {
        while (old.Count > 0)
        {
            var tmp = old[0];
            old.RemoveAt(0);
            Destroy(tmp);
        }
    }

    public override void CleanUp()
    {
        RemoveOld();
    }
}
