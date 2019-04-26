using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupDefault : PopupBase
{
    public GameObject imgHolderPrefab;
    public GameObject textHolderPrefab;
    public Canvas uiCanvas;

    public override Coroutine PopText(string text, Vector2 pos, float time)
    {      
        var textObj = Instantiate(textHolderPrefab, Camera.main.WorldToScreenPoint(pos), Quaternion.identity, uiCanvas.transform);
        var textComponent = textObj.GetComponent<Text>();
        var rect = textObj.GetComponent<RectTransform>();
        if (textComponent == null || rect == null)
            return null;
        textComponent.text = text;
        return StartCoroutine(PopTextCr(rect, textComponent, time));
    }

    private IEnumerator PopTextCr(RectTransform rect, Text text, float time)
    {
        for (int i = 1; i < 7; i++)
        {
            text.color = new Color(text.color.r, text.color.g, text.color.b, (i + 1) / 7);
            Vector3 scale = new Vector3((i / 6f), rect.localScale.y, rect.localScale.z);
            rect.localScale = scale;
            yield return new WaitForEndOfFrame();
        }
        yield return new WaitForSeconds(time);
        for (int i = 6; i > 0; i--)
        {
            Vector3 scale = new Vector3(rect.localScale.x * (16 - i) / 10, ((rect.localScale.y) * i) / 10, rect.localScale.z);
            rect.localScale = scale;
            yield return new WaitForEndOfFrame();
        }
        Destroy(gameObject);
    }

    public override Coroutine PopImage(Sprite image, Vector2 pos, float time)
    {
        var imgObj = Instantiate(imgHolderPrefab, Camera.main.WorldToScreenPoint(pos), Quaternion.identity, uiCanvas.transform);
        var imgComponent = imgObj.GetComponent<Image>();
        var rect = imgComponent?.rectTransform;
        if (imgComponent == null || rect == null)
            return null;
        imgComponent.sprite = image;
        rect.sizeDelta = Vector2.zero;
        return StartCoroutine(PopImageCr(rect, imgComponent, time));
    }

    private IEnumerator PopImageCr(RectTransform rect, Image img, float time)
    {
        for (int i = 1; i < 7; i++)
        {
            img.color = new Color(img.color.r, img.color.g, img.color.b, (i + 1) / 7);
            Vector3 scale = new Vector3((i / 6f), rect.localScale.y, rect.localScale.z);
            rect.localScale = scale;
            yield return new WaitForEndOfFrame();
        }
        rect.sizeDelta = img.sprite.rect.size;
        yield return new WaitForSeconds(time);
        for (int i = 6; i > 0; i--)
        {
            Vector3 scale = new Vector3(rect.localScale.x * (16 - i) / 10, ((rect.localScale.y) * i) / 10, rect.localScale.z);
            rect.localScale = scale;
            yield return new WaitForEndOfFrame();
        }
        Destroy(gameObject);
    }
}
