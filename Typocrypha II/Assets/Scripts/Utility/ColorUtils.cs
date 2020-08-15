using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ColorUtils
{
    /// <summary>
    /// Modify alpha.
    /// </summary>
    /// <param name="sr">New Alpha.</param>
    /// <param name="a">New alpha value.</param>
    public static void SetAlpha (this SpriteRenderer sr, float a)
    {
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, a);
    }
}
