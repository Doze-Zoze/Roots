using Roots.Utilities;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Roots.Items.ArmorSets
{
    public class OrichalcumHelmets : GlobalItem
    {
        List<int> ItemsToApplyTo =
        [
            ItemID.OrichalcumHelmet,
            ItemID.OrichalcumMask,
            ItemID.OrichalcumHeadgear
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
            item.defense = 6;
        }

        public override void UpdateEquip(Item item, Player player)
        {
            player.GetCritChance<GenericDamageClass>() += 17;
            player.moveSpeed += 0.08f;
            player.statManaMax2 += 60;
        }

        public override string IsArmorSet(Item head, Item body, Item legs)
        {
            if (ItemsToApplyTo.Contains(head.type) && body.type == ItemID.OrichalcumBreastplate && legs.type == ItemID.OrichalcumLeggings)
                return "OrichalcumSet";
            return string.Empty;
        }

        public override void UpdateArmorSet(Player player, string set)
        {
            if (set == "OrichalcumSet")
            {
                player.setBonus = RootsUtils.GetLocalizedTextValue("Armor.Orichalcum.SetBonus");
                player.onHitPetal = true;
            }
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            tooltips.ReplaceTooltipWith("Armor.Orichalcum.HelmetTooltip");
        }

    }
}
