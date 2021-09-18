using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogViewVNPlus : DialogView
{
    [SerializeField] private GameObject rightDialogBoxPrefab;
    [SerializeField] private GameObject leftDialogBoxPrefab;
    [SerializeField] private Transform messageContainer;
    // Do Tween Of Some sort for the animation
    public override void CleanUp()
    {

    }

    public override DialogBox PlayDialog(DialogItem data)
    {
        if (!IsDialogItemCorrectType(data, out DialogItemVNPlus dialogItem))
            return null;
        var prefab = dialogItem.IsLeft ? leftDialogBoxPrefab : rightDialogBoxPrefab;
        var dialogBox = Instantiate(prefab, messageContainer).GetComponent<DialogBox>();
        StartCoroutine(AnimateNewMessageIn(dialogBox, dialogItem));
        return dialogBox;
    }

    private IEnumerator AnimateNewMessageIn(DialogBox box, DialogItem item)
    {
        box.StartDialogBox(item);
        // Play animation
        yield break;
    }

    public override void SetEnabled(bool e)
    {
        gameObject.SetActive(e);
    }
}
