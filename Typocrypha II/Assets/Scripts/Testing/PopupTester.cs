using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PopupTester : MonoBehaviour
{
    public Sprite spr;
    public string text;
    public GameObject popupPrefab;
    public void Spawn()
    {
        var p = Instantiate(popupPrefab).GetComponent<PopupBase>();
        p?.PopImage(spr, Vector2.zero, 1);
    }

    public void SpawnText()
    {
        var p = Instantiate(popupPrefab).GetComponent<PopupBase>();
        p?.PopText(text, Vector2.zero, 1);
    }
}
