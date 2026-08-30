using RootsBeta.Utilities;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using RootsCore;

namespace RootsBeta.Items.ArmorSets
{
    public class CobaltHelmets : GlobalItem
    {
        #region Parameters
        public static int Defense => 5;
        public static float DamageBonus => 0.15f;
        public static float MoveSpeedBonus => 0.1f;
        public static int ManaMaxBonus => 40;
        #endregion

        private List<int> _itemsToApplyTo =
        [
            ItemID.CobaltHelmet,
            ItemID.CobaltMask,
            ItemID.CobaltHat
        ];
        public override bool IsLoadingEnabled(Mod mod) => Configs.Instance.RemoveClasses;
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
            player.moveSpeed += MoveSpeedBonus;
            player.statManaMax2 += ManaMaxBonus;
        }

        public override string IsArmorSet(Item head, Item body, Item legs)
        {
            if (_itemsToApplyTo.Contains(head.type) && body.type == ItemID.CobaltBreastplate && legs.type == ItemID.CobaltLeggings)
                return "CobaltSet";
            return string.Empty;
        }

        public override void UpdateArmorSet(Player player, string set)
        {
            if (set != "CobaltSet") return;
            player.setBonus = RootsUtils.GetLocalizedTextValue("Armor.Cobalt.SetBonus");
            player.moonLordLegs = true;
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            tooltips.ReplaceTooltipWith("Armor.Cobalt.HelmetTooltip");
        }
    }
}
