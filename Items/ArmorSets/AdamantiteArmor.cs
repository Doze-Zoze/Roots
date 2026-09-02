using Roots.Config;
using RootsBeta.Utilities;
using RootsCore;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RootsBeta.Items.ArmorSets
{
    public class AdamantiteHelmets : GlobalItem, IConfigurableContent<AdamantiteHelmets>
    {
        public static ConfigGroup ConfigGroups => ConfigGroup.ArmorReworks;

        #region Parameters
        public static int Defense => 10;
        public static float DamageBonus => 0.12f;
        public static int CritChanceBonus => 12;
        public static int MaxManaBonus => 80;
        public static float MoveSpeedBonusSet => 0.2f;
        public static float ManaCostReductionSet => 0.2f;
        #endregion

        private List<int> _itemsToApplyTo =
        [
            ItemID.AdamantiteHelmet,
            ItemID.AdamantiteMask,
            ItemID.AdamantiteHeadgear
        ];
        public override bool IsLoadingEnabled(Mod mod) => ConfigHelpers.ConfigEnabled<AdamantiteHelmets>();
        public override bool AppliesToEntity(Item item, bool lateInstantiation) => _itemsToApplyTo.Contains(item.type);
        public override bool InstancePerEntity => true;
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips) =>
            tooltips.ReplaceTooltipWith("Armor.Adamantite.HelmetTooltip");

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
            player.statManaMax2 += MaxManaBonus;
        }

        public override string IsArmorSet(Item head, Item body, Item legs)
        {
            if (_itemsToApplyTo.Contains(head.type) && body.type == ItemID.AdamantiteBreastplate && legs.type == ItemID.AdamantiteLeggings)
                return "AdamantiteSet";
            return string.Empty;
        }

        public override void UpdateArmorSet(Player player, string set)
        {
            if (set != "AdamantiteSet") return;
            player.setBonus = RootsUtils.GetLocalizedTextValue("Armor.Adamantite.SetBonus");
            player.moveSpeed += MoveSpeedBonusSet;
            player.ammoCost75 = true;
            player.manaCost -= ManaCostReductionSet;
        }
    }
}
