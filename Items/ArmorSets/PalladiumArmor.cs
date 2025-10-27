using Roots.Utilities;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Roots.Items.ArmorSets
{
    public class PalladiumHelmets : GlobalItem
    {
        List<int> ItemsToApplyTo =
        [
            ItemID.PalladiumMask,
            ItemID.PalladiumHeadgear,
            ItemID.PalladiumHelmet
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
            item.defense = 14;
        }

        public override void UpdateEquip(Item item, Player player)
        {
            player.GetDamage<GenericDamageClass>() += 0.1f;
            player.statManaMax2 += 20;
        }

        public override string IsArmorSet(Item head, Item body, Item legs)
        {
            if (ItemsToApplyTo.Contains(head.type) && body.type == ItemID.PalladiumBreastplate && legs.type == ItemID.PalladiumLeggings)
                return "PalladiumSet";
            return string.Empty;
        }

        public override void UpdateArmorSet(Player player, string set)
        {
            if (set == "PalladiumSet")
            {
                player.setBonus = RootsUtils.GetLocalizedTextValue("Armor.Palladium.SetBonus");
                player.onHitRegen = true;
            }
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            tooltips.ReplaceTooltipWith("Armor.Palladium.HelmetTooltip");
        }

    }
}
