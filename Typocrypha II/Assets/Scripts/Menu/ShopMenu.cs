using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ShopMenu : MonoBehaviour
{
    [SerializeField] private UnityEvent onClose;
    public void Open()
    {
        Close(); // Temp
    }

    public void Close()
    {
        onClose?.Invoke();
    }
}
