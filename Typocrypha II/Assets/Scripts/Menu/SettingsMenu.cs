using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingsMenu : MonoBehaviour
{
    [SerializeField] private MenuSlider first;
    [SerializeField] private MenuSlider uiSpeedSlider;
    [SerializeField] private MenuSlider gameplaySpeedSlider;
    [SerializeField] private MenuSlider textScrollSpeedSlider;

    public event System.Action OnClose;
    public void Open()
    {
        gameObject.SetActive(true);
        first.InitializeSelection();
        uiSpeedSlider.SetValue(Mathf.FloorToInt(Settings.UISpeed * 100));
        gameplaySpeedSlider.SetValue(Mathf.FloorToInt(Settings.GameplaySpeed * 100));
        textScrollSpeedSlider.SetValue(Mathf.FloorToInt(Settings.TextScrollSpeed * 100));
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

    public void OnUISpeedSliderChanged(float value)
    {
        Settings.UISpeed = value / 100;
    }

    public void OnGameplaySpeedSliderChanged(float value)
    {
        Settings.GameplaySpeed = value / 100;
    }

    public void OnTextScrollSpeedSliderChanged(float value)
    {
        Settings.TextScrollSpeed = value / 100;
    }
}
