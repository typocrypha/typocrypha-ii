using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class PopupBase : MonoBehaviour
{
    public abstract Coroutine PopText(string text, Vector2 pos, float time);
    public abstract Coroutine PopImage(Texture2D image, Vector2 pos, float time);
}
