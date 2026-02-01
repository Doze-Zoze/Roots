using RootsBeta.Utilities;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RootsBeta.Items.ArmorSets
{
    public class AdamantiteHelmets : GlobalItem
    {
        List<int> ItemsToApplyTo =
        [
            ItemID.AdamantiteHelmet,
            ItemID.AdamantiteMask,
            ItemID.AdamantiteHeadgear
        ];
        public override bool IsLoadingEnabled(Mod mod) => Configs.instance.RemoveClasses;

        public override bool AppliesToEntity(Item item, bool lateInstantiation) => ItemsToApplyTo.Contains(item.type);

        public override bool InstancePerEntity => true;

        public override void SetStaticDefaults()
        {
            foreach (var item in ItemsToApplyTo)
            {
                ItemSets.DontUseVanillaEquipEffects[item] = true;
                ItemSets.DontUseVanillaSetBonus[item] = true;
            }
        }

        public override void SetDefaults(Item item)
        {
            item.defense = 10;
        }

        public override void UpdateEquip(Item item, Player player)
        {
            player.GetDamage<GenericDamageClass>() += 0.12f;
            player.GetCritChance<GenericDamageClass>() += 12;
            player.statManaMax2 += 80;
        }

        public override string IsArmorSet(Item head, Item body, Item legs)
        {
            if (ItemsToApplyTo.Contains(head.type) && body.type == ItemID.AdamantiteBreastplate && legs.type == ItemID.AdamantiteLeggings)
                return "AdamantiteSet";
            return string.Empty;
        }

        public override void UpdateArmorSet(Player player, string set)
        {
            if (set == "AdamantiteSet")
            {
                player.setBonus = RootsUtils.GetLocalizedTextValue("Armor.Adamantite.SetBonus");
                player.moveSpeed += 0.2f;
                player.ammoCost75 = true;
                player.manaCost *= 0.8f;
            }
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            tooltips.ReplaceTooltipWith("Armor.Adamantite.HelmetTooltip");
        }

    }
}
