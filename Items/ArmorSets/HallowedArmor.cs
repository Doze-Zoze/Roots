using RootsBeta.Utilities;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using RootsCore;
using Roots.Config;

namespace RootsBeta.Items.ArmorSets
{
    public class HallowedHelmets : GlobalItem, IConfigurableContent
    {
        #region Parameters
        public static int Defense => 9;
        public static float DamageBonus => 0.15f;
        public static int CritChanceBonus => 10;
        public static int ManaMaxBonus => 100;
        #endregion

        private List<int> _itemsToApplyTo =
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
        public override bool IsLoadingEnabled(Mod mod) => this.ConfigEnabled;
        public override bool AppliesToEntity(Item item, bool lateInstantiation) => _itemsToApplyTo.Contains(item.type);
        public override bool InstancePerEntity => true;
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips) =>
            tooltips.ReplaceTooltipWith("Armor.Hallowed.HelmetTooltip");

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
            player.GetCritChance<GenericDamageClass>() += CritChanceBonus;
            player.statManaMax2 += ManaMaxBonus;
        }

        public override string IsArmorSet(Item head, Item body, Item legs)
        {
            if (_itemsToApplyTo.Contains(head.type) &&
                body.type is ItemID.HallowedPlateMail or ItemID.AncientHallowedPlateMail &&
                legs.type is ItemID.HallowedGreaves or ItemID.AncientHallowedGreaves)
                return "HallowedSet";
            return string.Empty;
        }

        public override void UpdateArmorSet(Player player, string set)
        {
            if (set != "HallowedSet") return;
            player.setBonus = RootsUtils.GetLocalizedTextValue("Armor.Hallowed.SetBonus");
            player.onHitDodge = true;
        }
    }
}
