using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetActionMenuPosition : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Vector2 position = GetComponent<RectTransform>().anchoredPosition;
        if (transform.parent.gameObject.GetComponent<RectTransform>().anchoredPosition.x < 0)
        {
            position.x = 6.25f;

        }
        else position.x = -6.25f;

        GetComponent<RectTransform>().anchoredPosition = position;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
