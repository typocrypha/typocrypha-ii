using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Dialog box used for floating text.
/// </summary>
public class DialogBoxFloat : DialogBox
{
    const float stayTime = 1f; // How long does text stay once fully display?
    const float fadeTime = 1f; // How long does it take to fade from full?

    new public void StartDialogBox(DialogItem dialogItem)
    {
        base.StartDialogBox(dialogItem);
        StartCoroutine(FadeAndDestroy());
    }

    // Once text is displayed, fade and destroy object over time.
    IEnumerator FadeAndDestroy()
    {
        yield return null;
        yield return new WaitUntil(() => IsDone);
        float alpha = dialogText.color.a;
        yield return new WaitForSeconds(stayTime);
        while (dialogText.color.a > 0) // Fade over time.
        {
            yield return new WaitWhile(() => PH.Pause);
            alpha -= Time.deltaTime / fadeTime;
            dialogText.color = new Color(
                dialogText.color.r, dialogText.color.g, dialogText.color.b, alpha);
        }
        Destroy(gameObject); // Destroy self.
    }
}
