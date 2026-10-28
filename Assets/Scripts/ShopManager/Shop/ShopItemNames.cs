public static class ShopItemNames
{
    public static string GetItemName(string itemId)
    {
        switch (LocalizationManager.CurrentLanguage)
        {
            case LocalizationManager.Language.English:
                return itemId switch
                {
                    ShopItemIds.UnlockMap => "Map",
                    ShopItemIds.UnlockDash => "Dash",
                    ShopItemIds.UnlockAnatomy => "Anatomy",
                    ShopItemIds.UnlockArmor => "Armor",
                    ShopItemIds.UnlockSwampDamageBonus => "Dirty",
                    ShopItemIds.ActivateLastChance => "Last Dance",
                    ShopItemIds.RestoreArmor => "Plates",
                    ShopItemIds.UnlockSkeletonDamageBonus => "Rotten",
                    ShopItemIds.UnlockDemonDamageBonus => "Nine Circles",
                    ShopItemIds.UnlockSpiderDamageBonus => "Arachnohate",
                    ShopItemIds.UnlockPassiveHealthRegeneration => "Time Heals",
                    ShopItemIds.UnlockRobocopRegeneration => "Robocop",
                    ShopItemIds.UnlockVampireAbility => "Dracula",
                    ShopItemIds.UnlockOnePunchManAbility => "One Punch",
                    ShopItemIds.UnlockBossDamageBonus => "Godslayer",
                    _ => string.Empty
                };

            case LocalizationManager.Language.Turkish:
                return itemId switch
                {
                    ShopItemIds.UnlockMap => "Harita",
                    ShopItemIds.UnlockDash => "Atılma",
                    ShopItemIds.UnlockAnatomy => "Anatomi",
                    ShopItemIds.UnlockArmor => "Zırh",
                    ShopItemIds.UnlockSwampDamageBonus => "Kirli",
                    ShopItemIds.ActivateLastChance => "Son Dans",
                    ShopItemIds.RestoreArmor => "Plakalar",
                    ShopItemIds.UnlockSkeletonDamageBonus => "Çürük",
                    ShopItemIds.UnlockDemonDamageBonus => "Dokuz Çember",
                    ShopItemIds.UnlockSpiderDamageBonus => "Araknofobi",
                    ShopItemIds.UnlockPassiveHealthRegeneration => "Zaman İyileştirir",
                    ShopItemIds.UnlockRobocopRegeneration => "Robokop",
                    ShopItemIds.UnlockVampireAbility => "Drakula",
                    ShopItemIds.UnlockOnePunchManAbility => "Tek Yumruk",
                    ShopItemIds.UnlockBossDamageBonus => "Tanrı Katili",
                    _ => string.Empty
                };

            default: 
                return itemId switch
                {
                    ShopItemIds.UnlockMap => "карта",
                    ShopItemIds.UnlockDash => "рывок",
                    ShopItemIds.UnlockAnatomy => "анатомия",
                    ShopItemIds.UnlockArmor => "броня",
                    ShopItemIds.UnlockSwampDamageBonus => "грязный",
                    ShopItemIds.ActivateLastChance => "ласт дэнс",
                    ShopItemIds.RestoreArmor => "пластины",
                    ShopItemIds.UnlockSkeletonDamageBonus => "гнилой",
                    ShopItemIds.UnlockDemonDamageBonus => "девять кругов",
                    ShopItemIds.UnlockSpiderDamageBonus => "арахнофобия",
                    ShopItemIds.UnlockPassiveHealthRegeneration => "время лечит",
                    ShopItemIds.UnlockRobocopRegeneration => "робокоп",
                    ShopItemIds.UnlockVampireAbility => "дракула",
                    ShopItemIds.UnlockOnePunchManAbility => "ван панч мэн",
                    ShopItemIds.UnlockBossDamageBonus => "годСлэер",
                    _ => string.Empty
                };
        }
    }
}
