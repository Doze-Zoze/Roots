using Roots.Config;
using RootsBeta.Utilities;
using RootsCore;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RootsBeta.Items.ArmorSets
{
    public class PalladiumHelmets : GlobalItem, IConfigurableContent<PalladiumHelmets>
    {
        public static ConfigGroup ConfigGroups => ConfigGroup.ArmorReworks;

        #region Parameters
        public static int Defense => 14;
        public static float DamageBonus => 0.1f;
        public static int ManaMaxBonus => 20;
        #endregion

        private List<int> _itemsToApplyTo =
        [
            ItemID.PalladiumMask,
            ItemID.PalladiumHeadgear,
            ItemID.PalladiumHelmet
        ];
        public override bool IsLoadingEnabled(Mod mod) => ConfigHelpers.ConfigEnabled<PalladiumHelmets>();
        public override bool AppliesToEntity(Item item, bool lateInstantiation) => _itemsToApplyTo.Contains(item.type);
        public override bool InstancePerEntity => true;

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
            player.statManaMax2 += ManaMaxBonus;
        }

        public override string IsArmorSet(Item head, Item body, Item legs)
        {
            if (_itemsToApplyTo.Contains(head.type) && body.type == ItemID.PalladiumBreastplate && legs.type == ItemID.PalladiumLeggings)
                return "PalladiumSet";
            return string.Empty;
        }

        public override void UpdateArmorSet(Player player, string set)
        {
            if (set != "PalladiumSet") return;
            player.setBonus = RootsUtils.GetLocalizedTextValue("Armor.Palladium.SetBonus");
            player.onHitRegen = true;
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            tooltips.ReplaceTooltipWith("Armor.Palladium.HelmetTooltip");
        }

    }
}
