using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopData : MonoBehaviour
{
    public BadgeWord.ShopUnlockCodes UnlockedShopCodes { get; private set; } = BadgeWord.ShopUnlockCodes.None;

    public void UnlockShopCodes(BadgeWord.ShopUnlockCodes codes)
    {
        UnlockedShopCodes |= codes;
    }

    public bool IsUnlockedInShop(BadgeWord badge)
    {
        switch (badge.ShopUnlockRequirements)
        {
            case BadgeWord.ShopUnlockCodes.None:
                return true;
            case BadgeWord.ShopUnlockCodes.CannotPurchase:
                return false;
            default:
                return UnlockedShopCodes.HasFlag(badge.ShopUnlockRequirements);
        }
    }
}
