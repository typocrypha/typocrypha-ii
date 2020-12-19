using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages spell cooldown icons.
/// </summary>
public class CooldownIcons : MonoBehaviour
{
    public Image[] Icons; // Hourglass icons.
    public Color OnColor; // Color when on cooldown.
    public Color OffColor; // Color when off cooldown.

    int totalCooldown; // Total cooldown.

    public void SetTotalCooldown(int total)
    {
        Debug.Log("total:" + total);
        totalCooldown = total;
        if (total > Icons.Length || total < 0)
            throw new UnityException("Bad cooldown amount");
        for (int i = 1; i <= Icons.Length - total; i++)
            Icons[Icons.Length - i].gameObject.SetActive(false);
    }

    public void SetCurrCooldown(int curr)
    {
        for (int i = 0; i < totalCooldown; i++)
        {
            if (i < curr)
                Icons[i].color = OnColor;
            else
                Icons[i].color = OffColor;
        }
    }
}
