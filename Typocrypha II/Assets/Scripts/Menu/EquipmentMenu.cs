using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentMenu : MonoBehaviour, IPausable
{
    public bool IsShowing { get; private set; }

    public PauseHandle PH => new PauseHandle();

    [SerializeField] private MenuButton first;
    [SerializeField] private GameObject menuObject;
    [SerializeField] private GameObject equipmentNotice;

    public void Enable()
    {
        IsShowing = false;
        menuObject.SetActive(false);
        equipmentNotice.SetActive(true);
        gameObject.SetActive(true);
    }

    public void Close()
    {
        IsShowing = false;
        menuObject.SetActive(false);
        gameObject.SetActive(false);
        equipmentNotice.SetActive(false);
    }

    private void Update()
    {
        if (PH.Pause)
        {
            return;
        }
        if (IsShowing)
        {
            return;
        }
        else if (Input.GetKeyDown(KeyCode.Return))
        {
            Open();
        }
    }

    public void Open()
    {
        IsShowing = true;
        menuObject.SetActive(true);
        equipmentNotice.SetActive(false);
        first.InitializeSelection();
        Debug.Log("Showing EquipmentMenu");
    }
}
