using UnityEngine;

public static class ColorUtils
{
    public static Color WithAlpha(this Color c, float a)
    {
        return new Color(c.r, c.g, c.b, a);
    }

    public static Color32 WithAlpha(this Color32 c, float a)
    {
        return new Color32(c.r, c.g, c.b, (byte)Mathf.RoundToInt(Mathf.Lerp(0, byte.MaxValue, a)));
    }
}
