using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] private MenuButton first;

    public event System.Action OnClose;
    public void Open()
    {
        gameObject.SetActive(true);
        first.InitializeSelection();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Close();
        }
    }

    public void Close()
    {
        OnClose?.Invoke();
        gameObject.SetActive(false);
    }
}
