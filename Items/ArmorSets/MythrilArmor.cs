using RootsBeta.Utilities;
using RootsCore;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RootsBeta.Items.ArmorSets
{
    public class MythrilHelmets : GlobalItem
    {
        #region Parameters
        public static int Defense => 3;
        public static float DamageBonus => 0.14f;
        public static int ManaMaxBonus => 80;
        public static float ManaCostReductionSet => 0.2f;
        public static float CritDamageBonusSet => 0.15f;
        #endregion

        private List<int> _itemsToApplyTo =
        [
            ItemID.MythrilHelmet,
            ItemID.MythrilHat,
            ItemID.MythrilHood
        ];
        public override bool IsLoadingEnabled(Mod mod) => Configs.Instance.RemoveClasses;
        public override bool AppliesToEntity(Item item, bool lateInstantiation) => _itemsToApplyTo.Contains(item.type);
        public override bool InstancePerEntity => true;
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips) =>
            tooltips.ReplaceTooltipWith("Armor.Mythril.HelmetTooltip");

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
            player.ammoCost80 = true;
        }

        public override string IsArmorSet(Item head, Item body, Item legs)
        {
            if (_itemsToApplyTo.Contains(head.type) && body.type == ItemID.MythrilChainmail && legs.type == ItemID.MythrilGreaves)
                return "MythrilSet";
            return string.Empty;
        }

        public override void UpdateArmorSet(Player player, string set)
        {
            if (set != "MythrilSet") return;
            player.setBonus = RootsUtils.GetLocalizedTextValue("Armor.Mythril.SetBonus");
            player.manaCost -= ManaCostReductionSet;
            player.Roots().ModifyHitNPCFuncs.Add((_, _, modifiers) =>
            {
                modifiers.CritDamage += CritDamageBonusSet;
                return modifiers;
            });
        }
    }
}
