//using System.Collections;
//using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using TMPro;
using System.Collections.Generic;
using UnityEngine.UI;

public class ShopMenu : MonoBehaviour
{
    [SerializeField] private bool openOnStart;
    [SerializeField] private MenuButton firstLandingSelection;
    [SerializeField] private MenuButton firstPurchaseSelection;
    [SerializeField] private MenuButton ItemButtonPrefab;

    [Header("Child Object References")]
    [SerializeField] private GameObject[] landingExclusiveUI;
    [SerializeField] private GameObject[] purchaseExclusiveUI;
    [SerializeField] private TextMeshProUGUI MainDialogText;
    [SerializeField] private LabeledContent ItemDescriptionLeft;
    [SerializeField] private LabeledContent ItemDescriptionRight;
    [SerializeField] private GameObject purchaseButtonContainer;

    [Header("Events")]
    [SerializeField] private UnityEvent onExit;

    [Header("Testing")]
    [SerializeField] private BadgeWord[] testBadgeWords;

    private GameObject previousSelection;

    private void Start()
    {
        if (openOnStart) OpenLanding();
    }

    [ContextMenu("OpenLanding")]
    public void OpenLanding()
    {
        if (EventSystem.current) EventSystem.current.SetSelectedGameObject(null); //prevent visual bug
        gameObject.SetActive(true);
        foreach (var go in purchaseExclusiveUI) go.SetActive(false);
        foreach (var go in landingExclusiveUI) go.SetActive(true);
        if (previousSelection) previousSelection.GetComponent<MenuButton>().InitializeSelection();
        else firstLandingSelection.InitializeSelection();
        PrintMain("Welcome to my funny little shoppe!");
    }

    private void OpenPurchase()
    {
        if (EventSystem.current)
        {
            previousSelection = EventSystem.current.currentSelectedGameObject;
            EventSystem.current.SetSelectedGameObject(null); //prevent visual bug
        }
        gameObject.SetActive(true);
        foreach (var go in landingExclusiveUI) go.SetActive(false);
        foreach (var go in purchaseExclusiveUI) go.SetActive(true);
        firstPurchaseSelection.InitializeSelection();
    }

    [ContextMenu("OpenBuy")]
    public void OpenBuy()
    {
        OpenPurchase();

        // create buttons as needed
        while (purchaseButtonContainer.transform.childCount < testBadgeWords.Length)
        {
            Instantiate(ItemButtonPrefab, purchaseButtonContainer.transform);
        }
        // disable all buttons except first (return button)
        foreach (Transform buttonTransform in purchaseButtonContainer.transform)
        {
            buttonTransform.gameObject.SetActive(buttonTransform.GetSiblingIndex() == 0);
        }
        // initialize buttons
        for (int i = 0; i < testBadgeWords.Length; i++)
        {
            var buttonGameObject = purchaseButtonContainer.transform.GetChild(i + 1).gameObject;
            buttonGameObject.SetActive(true);
            var textUI = buttonGameObject.GetComponentInChildren<TextMeshProUGUI>();
            textUI.SetText(testBadgeWords[i].ToString());
            int j = i;
            buttonGameObject.GetComponent<Button>().onClick.AddListener(()=>PurchaseBadge(testBadgeWords[j]));
            buttonGameObject.GetComponent<MenuButton>().onSelect.AddListener(()=>InspectBadge(testBadgeWords[j]));
        }
    }

    [ContextMenu("OpenUpgrade")]
    public void OpenUpgrade()
    {
        OpenPurchase();
    }

    public void ClearInspection()
    {
        PrintLeftAndRight("Current", "", "Next", "");
    }    

    public void InspectBadge(BadgeWord badge)
    {
        if (badge == null) PrintLeftAndRight("Current", "", "Next", "");
        else PrintLeftAndRight("Current", badge.Description, "Next", badge.HasUpgrade ? badge.NextUpgrade.Description : "");
    }

    public void PurchaseBadge(BadgeWord purchase)
    {
        //var equipment = PlayerDataManager.instance.equipment;
        //var unlocked = equipment.IsBadgeUnlocked(purchase);

        //checks
        //if (PlayerDataManager.instance.currency <= purchase.cost) return;
        //if (!purchase.HasUpgrade) return;

        //if (!unlocked) equipment.UnlockBadge(purchase);
        //if (unlocked) purchase.Upgrade();
        //PlayerDataManager.instance.currency -= purchase.cost;
    }

    public void Exit()
    {
        onExit?.Invoke();
        gameObject.SetActive(false);
    }

    private void PrintMain(string text)
    {
        ItemDescriptionLeft.transform.gameObject.SetActive(false);
        ItemDescriptionRight.transform.gameObject.SetActive(false);
        MainDialogText.transform.parent.gameObject.SetActive(true);
        MainDialogText.text = text;
    }

    private void PrintLeft(string heading, string content)
    {
        MainDialogText.transform.parent.gameObject.SetActive(false);
        ItemDescriptionLeft.transform.gameObject.SetActive(true);
        ItemDescriptionLeft.SetLabel(heading);
        ItemDescriptionLeft.SetContent(content);
    }

    private void PrintRight(string heading, string content)
    {
        MainDialogText.transform.parent.gameObject.SetActive(false);
        ItemDescriptionRight.transform.gameObject.SetActive(true);
        ItemDescriptionRight.SetLabel(heading);
        ItemDescriptionRight.SetContent(content);
    }

    private void PrintLeftAndRight(string headingLeft, string contentLeft, string headingRight, string contentRight)
    {
        PrintLeft(headingLeft, contentLeft);
        PrintRight(headingRight, contentRight);
    }
}
