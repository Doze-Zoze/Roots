using Roots.Config;
using RootsBeta.Utilities;
using RootsCore;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RootsBeta.Items.ArmorSets
{
    public class OrichalcumHelmets : GlobalItem, IConfigurableContent<OrichalcumHelmets>
    {
        public static ConfigGroup ConfigGroups => ConfigGroup.ArmorReworks;

        #region Parameters
        public static int Defense => 6;
        public static int CritChanceBonus => 17;
        public static float MoveSpeedBonus => 0.08f;
        public static int ManaMaxBonus => 60;
        #endregion

        private List<int> _itemsToApplyTo =
        [
            ItemID.OrichalcumHelmet,
            ItemID.OrichalcumMask,
            ItemID.OrichalcumHeadgear
        ];
        public override bool IsLoadingEnabled(Mod mod) => ConfigHelpers.ConfigEnabled<OrichalcumHelmets>();
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
            player.GetCritChance<GenericDamageClass>() += CritChanceBonus;
            player.moveSpeed += MoveSpeedBonus;
            player.statManaMax2 += ManaMaxBonus;
        }

        public override string IsArmorSet(Item head, Item body, Item legs)
        {
            if (_itemsToApplyTo.Contains(head.type) && body.type == ItemID.OrichalcumBreastplate && legs.type == ItemID.OrichalcumLeggings)
                return "OrichalcumSet";
            return string.Empty;
        }

        public override void UpdateArmorSet(Player player, string set)
        {
            if (set != "OrichalcumSet") return;
            player.setBonus = RootsUtils.GetLocalizedTextValue("Armor.Orichalcum.SetBonus");
            player.onHitPetal = true;
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            tooltips.ReplaceTooltipWith("Armor.Orichalcum.HelmetTooltip");
        }

    }
}
