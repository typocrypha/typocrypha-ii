using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetArrowKeyDirection : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Vector2 scale = GetComponent<RectTransform>().localScale;
        if (transform.parent.parent.gameObject.GetComponent<RectTransform>().anchoredPosition.x < 0)
        {
            scale.x = 1f;

        }
        else scale.x = -1f;

        GetComponent<RectTransform>().localScale = scale;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
