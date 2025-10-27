using Roots.Utilities;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Roots.Items.ArmorSets
{
    public class HallowedHelmets : GlobalItem
    {
        List<int> ItemsToApplyTo =
        [
            ItemID.HallowedMask,
            ItemID.HallowedHelmet,
            ItemID.HallowedHeadgear,
            ItemID.HallowedHood,
            ItemID.AncientHallowedMask,
            ItemID.AncientHallowedHelmet,
            ItemID.AncientHallowedHeadgear,
            ItemID.AncientHallowedHood,
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
            item.defense = 9;
        }

        public override void UpdateEquip(Item item, Player player)
        {
            player.GetDamage<GenericDamageClass>() += 0.15f;
            player.GetCritChance<GenericDamageClass>() += 10;
            player.statManaMax2 += 100;
        }

        public override string IsArmorSet(Item head, Item body, Item legs)
        {
            if (ItemsToApplyTo.Contains(head.type) && (body.type == ItemID.HallowedPlateMail || body.type == ItemID.AncientHallowedPlateMail) && (legs.type == ItemID.HallowedGreaves || legs.type == ItemID.AncientHallowedGreaves))
                return "HallowedSet";
            return string.Empty;
        }

        public override void UpdateArmorSet(Player player, string set)
        {
            if (set == "HallowedSet")
            {
                player.setBonus = RootsUtils.GetLocalizedTextValue("Armor.Hallowed.SetBonus");
                player.onHitDodge = true;
            }
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            tooltips.ReplaceTooltipWith("Armor.Hallowed.HelmetTooltip");
        }

    }
}
