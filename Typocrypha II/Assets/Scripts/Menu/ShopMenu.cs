//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class ShopMenu : MonoBehaviour
{
    [SerializeField] private bool openOnStart;
    [SerializeField] private MenuButton firstLandingSelection;
    [SerializeField] private MenuButton firstPurchaseSelection;
    [SerializeField] private MenuButton ItemButtonPrefab;

    [Header("Child Object References")]
    [SerializeField] private GameObject[] landingExclusiveUI;
    [SerializeField] private GameObject[] purchaseExclusiveUI;

    [Header("Events")]
    [SerializeField] private UnityEvent onExit;



    private void Start()
    {
        if (openOnStart) OpenLanding();
    }

    [ContextMenu("OpenLanding")]
    public void OpenLanding()
    {
        if (EventSystem.current) EventSystem.current.SetSelectedGameObject(null); //avoid ghost selections
        gameObject.SetActive(true);
        foreach (var go in purchaseExclusiveUI) go.SetActive(false);
        foreach (var go in landingExclusiveUI) go.SetActive(true);
        firstLandingSelection.InitializeSelection();
    }

    private void OpenPurchase()
    {
        if (EventSystem.current) EventSystem.current.SetSelectedGameObject(null); //avoid ghost selections
        gameObject.SetActive(true);
        foreach (var go in landingExclusiveUI) go.SetActive(false);
        foreach (var go in purchaseExclusiveUI) go.SetActive(true);
        firstPurchaseSelection.InitializeSelection();
    }

    [ContextMenu("OpenBuy")]
    public void OpenBuy()
    {
        OpenPurchase();
        //populate purchasable
    }

    [ContextMenu("OpenUpgrade")]
    public void OpenUpgrade()
    {
        OpenPurchase();
        //populate upgradable
    }

    public void Exit()
    {
        onExit?.Invoke();
        gameObject.SetActive(false);
    }
}
