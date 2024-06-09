﻿//using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;
using TMPro;
using UnityEngine.UI;
using Utilities.Unity;
using System.Globalization;
using DG.Tweening;

public class ShopMenu : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private bool openOnStart;
    [SerializeField] private AudioClip purchaseValidSFX;
    [SerializeField] private AudioClip purchaseInvalidSFX;
    [SerializeField] private Color purchaseValidColor;
    [SerializeField] private Color purchaseInvalidColor;
    [SerializeField] private Color purchaseValidSelectColor;
    [SerializeField] private Color purchaseInvalidSelectColor;

    [Header("Child Object References")]
    [SerializeField] private MenuButton firstLandingSelection;
    [SerializeField] private MenuButton firstPurchaseSelection;
    [SerializeField] private MenuButton ItemButtonPrefab;
    [SerializeField] private GameObject[] landingExclusiveUI;
    [SerializeField] private GameObject[] purchaseExclusiveUI;
    [SerializeField] private TextMeshProUGUI MainDialogText;
    [SerializeField] private LabeledContent ItemDescriptionLeft;
    [SerializeField] private LabeledContent ItemDescriptionRight;
    [SerializeField] private GameObject purchaseButtonContainer;
    [SerializeField] private TextMeshProUGUI currencyUI;
    [SerializeField] private GameObject confirmationWindow;

    [Header("Events")]
    [SerializeField] private UnityEvent onExit;

    [Header("Badges")]
    [SerializeField] private BadgeBundle allBadges;

    private enum PurchaseMode { None, Buy, Upgrade }
    private PurchaseMode currentMode = PurchaseMode.None;

    private NumberFormatInfo formatInfo;


    public static IEnumerable<BadgeWord> FilterBadges(IEnumerable<BadgeWord> badges, System.Func<BadgeWord, bool> predicate)
    {
        return badges.Where(predicate).OrderBy(b => b.NextCost);
    }

    public static IEnumerable<BadgeWord> FilterCanPurchase(IEnumerable<BadgeWord> badges)
    {
        return FilterBadges(badges, b => b.CanPurchase);
    }

    public static IEnumerable<BadgeWord> FilterCanUpgrade(IEnumerable<BadgeWord> badges)
    {
        return FilterBadges(badges, b => b.IsUnlocked && b.HasUpgrade);
    }

    private void Start()
    {
        if (openOnStart) OpenLanding();

        // create buttons as needed
        while (purchaseButtonContainer.transform.childCount < allBadges.badges.Count)
        {
            Instantiate(ItemButtonPrefab, purchaseButtonContainer.transform);
        }

        // initialize currency display
        formatInfo = CultureInfo.CurrentCulture.NumberFormat.Clone() as NumberFormatInfo;
        formatInfo.CurrencyGroupSeparator = ""; // remove the group separator
        PrintCurrency();
    }

    public void OpenLanding()
    {
        gameObject.SetActive(true);
        foreach (var go in purchaseExclusiveUI) go.SetActive(false);
        foreach (var go in landingExclusiveUI) go.SetActive(true);
        firstLandingSelection.InitializeSelection();
        currentMode = PurchaseMode.None;
    }

    private void OpenPurchase()
    {
        gameObject.SetActive(true);
        foreach (var go in landingExclusiveUI) go.SetActive(false);
        foreach (var go in purchaseExclusiveUI) go.SetActive(true);
        firstPurchaseSelection.InitializeSelection();

        // disable all buttons except first (return button)
        foreach (Transform buttonTransform in purchaseButtonContainer.transform)
        {
            buttonTransform.gameObject.SetActive(buttonTransform.GetSiblingIndex() == 0);
        }
    }

    private void InspectBadge(BadgeWord badge)
    {
        if (badge == null) PrintLeftAndRight("", "", "", "");
        else if (badge.CanPurchase) PrintLeft(badge.DisplayName, badge.Description);
        else if (badge.IsUnlocked) PrintLeftAndRight(badge.DisplayName, badge.Description, badge.HasUpgrade ? badge.NextUpgrade.DisplayName : "", badge.HasUpgrade ? badge.NextUpgrade.Description : "");
    }

    private bool CanPurchaseOrUpgradeBadge(BadgeWord purchase)
    {
        return (purchase.CanPurchase || purchase.HasUpgrade) && PlayerDataManager.instance.currency >= purchase.NextCost;
    }

    private void PurchaseOrUpgradeBadge(BadgeWord purchase)
    {
        if (!CanPurchaseOrUpgradeBadge(purchase)) return;

        if (purchase.CanPurchase)
        {
            PlayerDataManager.instance.currency -= purchase.Cost;
            PlayerDataManager.instance.equipment.UnlockBadge(purchase);
            PrintCurrency();
        }
        else if (purchase.IsUnlocked)
        {
            PlayerDataManager.instance.currency -= purchase.UpgradeCost;
            purchase.Upgrade();
            PrintCurrency();
        }
    }

    private void ItemButtonAction(BadgeWord badge, MenuButton button)
    {
        if (CanPurchaseOrUpgradeBadge(badge))
        {
            OpenConfirmation(badge, button);
            AudioManager.instance.PlaySFX(purchaseValidSFX);
        }
        else
        {
            AudioManager.instance.PlaySFX(purchaseInvalidSFX);
            ShakeCurrency();
        }
    }

    private void InitializeItemButtons(IEnumerable<BadgeWord> badges)
    {
        int i = 1;
        var previousMenuButton = purchaseButtonContainer.GetComponentInChildren<MenuButton>();
        foreach (var badge in badges)
        {
            var buttonGO = purchaseButtonContainer.transform.GetChild(i).gameObject;
            buttonGO.SetActive(true);
            var textUI = buttonGO.GetComponentInChildren<TextMeshProUGUI>();
            var menuButton = buttonGO.GetComponent<MenuButton>();

            //set text
            string nameText = badge.ToString();
            string costText = badge.NextCost.ToString("C0");
            const int maxTotalLength = 23;
            const int maxCostLength = 4;
            string result = $"{nameText} {costText.PadLeft(maxTotalLength - (nameText.Length + 1) - maxCostLength + costText.Length)}";
            textUI.SetText(result);

            //set color
            bool canBuy = CanPurchaseOrUpgradeBadge(badge);
            menuButton.defaultColor = canBuy ? purchaseValidColor : purchaseInvalidColor;
            menuButton.selectedColor = canBuy ? purchaseValidSelectColor : purchaseInvalidSelectColor;
            textUI.color = menuButton.defaultColor;

            //set callbacks
            menuButton.button.onClick.ReplaceAllListeners(()=>ItemButtonAction(badge, menuButton));
            menuButton.onSelect.ReplaceAllListeners(() => InspectBadge(badge));


            //set previous navigation
            var tempNavigation = previousMenuButton.button.navigation;
            tempNavigation.mode = Navigation.Mode.Explicit;
            tempNavigation.selectOnDown = menuButton.button;
            previousMenuButton.button.navigation = tempNavigation;

            //set current navigation
            tempNavigation = menuButton.button.navigation;
            tempNavigation.mode = Navigation.Mode.Explicit;
            tempNavigation.selectOnUp = previousMenuButton.button;
            menuButton.button.navigation = tempNavigation;

            previousMenuButton = menuButton;
            ++i;
        }
    }

    public void OpenBuy()
    {
        OpenPurchase();
        InitializeItemButtons(FilterCanPurchase(allBadges.badges.Values));
        currentMode = PurchaseMode.Buy;
    }

    public void OpenUpgrade()
    {
        OpenPurchase();
        InitializeItemButtons(FilterCanUpgrade(allBadges.badges.Values));
        currentMode = PurchaseMode.Upgrade;
    }

    public void OpenConfirmation(BadgeWord purchase, MenuButton previousSelection)
    {
        confirmationWindow.SetActive(true);
        DOTween.Complete("OpenConfirmation");
        DOTween.Complete("CloseConfirmation");
        (confirmationWindow.transform as RectTransform).DOScaleX(1f, 0.1f).From(0f).SetId("OpenConfirmation");

        PrintConfirmation(purchase.DisplayName, purchase.NextCost);

        var buttons = confirmationWindow.GetComponentsInChildren<MenuButton>();
        buttons[0].button.onClick.ReplaceAllListeners(()=> {
            PurchaseOrUpgradeBadge(purchase);
            CloseConfirmation(previousSelection, true);
        });
        buttons[1].button.onClick.ReplaceAllListeners(()=>CloseConfirmation(previousSelection));
        buttons[0].InitializeSelection(); //select yes by default
    }

    public void CloseConfirmation(MenuButton previousSelection = null, bool redraw = false)
    {
        DOTween.Complete("OpenConfirmation");
        DOTween.Complete("CloseConfirmation");
        (confirmationWindow.transform as RectTransform).DOScaleX(0f, 0.1f).From(1f).SetId("CloseConfirmation")
            .OnComplete(() => confirmationWindow.SetActive(false));

        if (!redraw)
        {
            previousSelection.InitializeSelection();
        }
        else
        {
            var selectionIndex = previousSelection.transform.GetSiblingIndex();
            if (currentMode == PurchaseMode.Buy) OpenBuy();
            if (currentMode == PurchaseMode.Upgrade) OpenUpgrade();
            var newSelection = purchaseButtonContainer.transform.GetChild(selectionIndex);
            if (!newSelection.gameObject.activeSelf)
                newSelection = purchaseButtonContainer.transform.GetChild(selectionIndex - 1);
            newSelection.GetComponent<MenuButton>().InitializeSelection();
        }
    }

    public void Exit()
    {
        onExit?.Invoke();
        gameObject.SetActive(false);
    }

    public void PrintMain(string text)
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

    private void PrintCurrency()
    {
        currencyUI.text = PlayerDataManager.instance.currency.ToString("C0", formatInfo);
    }

    private void ShakeCurrency()
    {
        var rectTransform = currencyUI.transform.parent as RectTransform;
        DOTween.Complete("ShakeCurrency");
        DOTween.Sequence()
            .SetId("ShakeCurrency")
            .Append(rectTransform.DOAnchorPosX(5, 0.075f).SetRelative(true))
            .Append(rectTransform.DOAnchorPosX(-10, 0.075f).SetRelative(true))
            .Append(rectTransform.DOAnchorPosX(10, 0.075f).SetRelative(true))
            .Append(rectTransform.DOAnchorPosX(-5, 0.075f).SetRelative(true));
    }

    private void PrintConfirmation(string badgeName, int cost)
    {
        var text = string.Format("Purchase {0} for {1}?", badgeName, cost.ToString("C0", formatInfo));
        confirmationWindow.GetComponentInChildren<TextMeshProUGUI>().text = text;
    }
}
