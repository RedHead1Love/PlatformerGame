public static class ShopItemDescriptions
{
    public static string GetItemDescription(string itemId)
    {
        switch (LocalizationManager.CurrentLanguage)
        {
            case LocalizationManager.Language.English:
                return itemId switch
                {
                    ShopItemIds.UnlockMap => "prison map",
                    ShopItemIds.UnlockDash => "quick dash",
                    ShopItemIds.UnlockAnatomy => "allows picking up medkits",
                    ShopItemIds.UnlockArmor => "grants armor",
                    ShopItemIds.UnlockSwampDamageBonus => "+100% damage swamp monsters",
                    ShopItemIds.ActivateLastChance => "survive a fatal blow",
                    ShopItemIds.RestoreArmor => "restores armor",
                    ShopItemIds.UnlockSkeletonDamageBonus => "+100% damage to skeletons",
                    ShopItemIds.UnlockDemonDamageBonus => "+100% damage to demons",
                    ShopItemIds.UnlockSpiderDamageBonus => "+100% damage to spiders",
                    ShopItemIds.UnlockPassiveHealthRegeneration => "passive health regeneration",
                    ShopItemIds.UnlockRobocopRegeneration => "passive armor regeneration",
                    ShopItemIds.UnlockVampireAbility => "restores health on kill",
                    ShopItemIds.UnlockOnePunchManAbility => "chance of instant kill",
                    ShopItemIds.UnlockBossDamageBonus => "+100% damage to bosses",
                    _ => string.Empty
                };

            case LocalizationManager.Language.Turkish:
                return itemId switch
                {
                    ShopItemIds.UnlockMap => "hapishane haritası",
                    ShopItemIds.UnlockDash => "hızlı atılma",
                    ShopItemIds.UnlockAnatomy => "sağlık çantası almayı sağlar",
                    ShopItemIds.UnlockArmor => "zırh verir",
                    ShopItemIds.UnlockSwampDamageBonus => "bataklık canavarlarına +%100 hasar",
                    ShopItemIds.ActivateLastChance => "ölümcül darbeden sağ çık",
                    ShopItemIds.RestoreArmor => "zırhı yeniler",
                    ShopItemIds.UnlockSkeletonDamageBonus => "iskeletlere +%100 hasar",
                    ShopItemIds.UnlockDemonDamageBonus => "iblislere +%100 hasar",
                    ShopItemIds.UnlockSpiderDamageBonus => "örümceklere +%100 hasar",
                    ShopItemIds.UnlockPassiveHealthRegeneration => "pasif can yenilenmesi",
                    ShopItemIds.UnlockRobocopRegeneration => "pasif zırh yenilenmesi",
                    ShopItemIds.UnlockVampireAbility => "öldürme başına can yeniler",
                    ShopItemIds.UnlockOnePunchManAbility => "anında öldürme şansı",
                    ShopItemIds.UnlockBossDamageBonus => "boss'lara +%100 hasar",
                    _ => string.Empty
                };

            default: 
                return itemId switch
                {
                    ShopItemIds.UnlockMap => "карта тюрьмы",
                    ShopItemIds.UnlockDash => "быстрый подкат",
                    ShopItemIds.UnlockAnatomy => "позволяет подбирать аптечки",
                    ShopItemIds.UnlockArmor => "получение брони",
                    ShopItemIds.UnlockSwampDamageBonus => "+100% урона по болотным",
                    ShopItemIds.ActivateLastChance => "выживание при смертельном ударе",
                    ShopItemIds.RestoreArmor => "восстановление брони",
                    ShopItemIds.UnlockSkeletonDamageBonus => "+100% урона по скелетам",
                    ShopItemIds.UnlockDemonDamageBonus => "+100% урона по демонам",
                    ShopItemIds.UnlockSpiderDamageBonus => "+100% урона по паукам",
                    ShopItemIds.UnlockPassiveHealthRegeneration => "пассивное восстановление здоровья",
                    ShopItemIds.UnlockRobocopRegeneration => "пассивное восстановление брони",
                    ShopItemIds.UnlockVampireAbility => "получение здоровья за убийство",
                    ShopItemIds.UnlockOnePunchManAbility => "шанс мгновенного убийства",
                    ShopItemIds.UnlockBossDamageBonus => "+100% урона по боссам",
                    _ => string.Empty
                };
        }
    }
}
