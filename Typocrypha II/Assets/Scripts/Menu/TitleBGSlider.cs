using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleBGSlider : MonoBehaviour
{
    [SerializeField] float scrollSpeed;
    [SerializeField] RectTransform rt;
    void FixedUpdate()
    {
        rt.anchoredPosition -= new Vector2(scrollSpeed, 0);
        //shift by screen width
        if (rt.anchoredPosition.x <= -640)
        {
            rt.anchoredPosition = new Vector2(640, 0);
        }
    }
}
