using Roots.Config;
using RootsBeta.Utilities;
using RootsCore;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RootsBeta.Items.ArmorSets
{
    public class TitaniumHelmets : GlobalItem, IConfigurableContent
    {
        #region Parameters
        public static int Defense => 18;
        public static float DamageBonus => 0.12f;
        public static int CritChanceBonus => 5;
        public static int ManaMaxBonus => 100;
        #endregion

        private List<int> _itemsToApplyTo =
        [
            ItemID.TitaniumHeadgear,
            ItemID.TitaniumMask,
            ItemID.TitaniumHelmet
        ];
        public override bool IsLoadingEnabled(Mod mod) => this.ConfigEnabled;
        public override bool AppliesToEntity(Item item, bool lateInstantiation) => _itemsToApplyTo.Contains(item.type);
        public override bool InstancePerEntity => true;
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips) =>
            tooltips.ReplaceTooltipWith("Armor.Titanium.HelmetTooltip");

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
            if (_itemsToApplyTo.Contains(head.type) && body.type == ItemID.TitaniumBreastplate && legs.type == ItemID.TitaniumLeggings)
                return "TitaniumSet";
            return string.Empty;
        }

        public override void UpdateArmorSet(Player player, string set)
        {
            if (set != "TitaniumSet") return;
            player.setBonus = RootsUtils.GetLocalizedTextValue("Armor.Titanium.SetBonus");
            player.onHitTitaniumStorm = true;
        }
    }
}
