using RootsBeta.Utilities;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using RootsCore;
using Roots.Config;

namespace RootsBeta.Items.ArmorSets
{
    public class ChlorophyteHelmets : GlobalItem, IConfigurableContent<ChlorophyteHelmets>
    {
        public static ConfigGroup ConfigGroups => ConfigGroup.ArmorReworks;

        #region Parameters
        private static int Defense => 13;
        private static float DamageBonus => 0.16f;
        private static float ManaCostReduction => 0.17f;
        public static int ManaMaxBonus => 80;
        #endregion

        private List<int> _itemsToApplyTo =
        [
            ItemID.ChlorophyteHeadgear,
            ItemID.ChlorophyteHelmet,
            ItemID.ChlorophyteMask
        ];
        public override bool AppliesToEntity(Item item, bool lateInstantiation) => _itemsToApplyTo.Contains(item.type);
        public override bool InstancePerEntity => true;
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips) =>
            tooltips.ReplaceTooltipWith("Armor.Chlorophyte.HelmetTooltip");

        public override void SetStaticDefaults()
        {
            foreach (var item in _itemsToApplyTo)
            {
                ItemSets.DontUseVanillaEquipEffects[item] = true;
                ItemSets.DontUseVanillaSetBonus[item] = true;
            }
        }

        public override void SetDefaults(Item item)
        {
            item.defense = Defense;
        }

        public override void UpdateEquip(Item item, Player player)
        {
            player.GetDamage<GenericDamageClass>() += DamageBonus;
            player.chloroAmmoCost80 = true;
            player.manaCost -= ManaCostReduction;
            player.statManaMax2 += ManaMaxBonus;
        }

        public override string IsArmorSet(Item head, Item body, Item legs)
        {
            if (_itemsToApplyTo.Contains(head.type) && body.type == ItemID.ChlorophytePlateMail && legs.type == ItemID.ChlorophyteGreaves)
                return "ChlorophyteSet";
            return string.Empty;
        }

        public override void UpdateArmorSet(Player player, string set)
        {
            if (set == "ChlorophyteSet")
            {
                player.setBonus = RootsUtils.GetLocalizedTextValue("Armor.Chlorophyte.SetBonus");
                player.AddBuff(BuffID.LeafCrystal, 18000);
            }
            else if (player.crystalLeaf)
            {
                for (int n = 0; n < player.buffType.Length; n++)
                {
                    if (player.buffType[n] == BuffID.LeafCrystal)
                    {
                        player.DelBuff(n);
                    }
                }
            }
        }
    }
}
