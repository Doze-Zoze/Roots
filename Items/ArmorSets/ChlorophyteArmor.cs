using Roots.Utilities;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Roots.Items.ArmorSets
{
    public class ChlorophyteHelmets : GlobalItem
    {
        List<int> ItemsToApplyTo = 
        [
            ItemID.ChlorophyteHeadgear,
            ItemID.ChlorophyteHelmet,
            ItemID.ChlorophyteMask
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
            item.defense = 13;
        }

        public override void UpdateEquip(Item item, Player player)
        {
            player.GetDamage<GenericDamageClass>() += 0.16f;
            player.chloroAmmoCost80 = true;
            player.manaCost *= (1 - 0.17f);
            player.statManaMax2 += 80;
        }

        public override string IsArmorSet(Item head, Item body, Item legs)
        {
            if (ItemsToApplyTo.Contains(head.type) && body.type == ItemID.ChlorophytePlateMail && legs.type == ItemID.ChlorophyteGreaves)
                return "ChlorophyteSet";
            return string.Empty;
        }

        public override void UpdateArmorSet(Player player, string set)
        {
            if (set == "ChlorophyteSet")
                player.AddBuff(60, 18000);
            else if (player.crystalLeaf)
            {
                for (int n = 0; n < player.buffType.Length; n++)
                {
                    if (player.buffType[n] == 60)
                    {
                        player.DelBuff(n);
                    }
                }
            }
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            tooltips.ReplaceTooltipWith("Armor.ChlorophyteHelmets.Tooltip");
        }

    }
}
