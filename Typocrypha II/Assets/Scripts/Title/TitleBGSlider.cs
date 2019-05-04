using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitleBGSlider : MonoBehaviour
{
    Rigidbody2D rb;
    RectTransform rt;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = new Vector2 (-32, 0);
        rt = GetComponent<RectTransform>();

    }
    void FixedUpdate()
    {
        //shift by screen width
        if (rt.anchoredPosition.x < -1366)
            rt.anchoredPosition = new Vector2 (rt.anchoredPosition.x + 1366*2, 0);
    }
}
