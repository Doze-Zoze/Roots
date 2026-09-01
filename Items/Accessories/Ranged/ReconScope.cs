using Roots.Config;
using RootsBeta.Utilities;
using RootsCore;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RootsBeta.Items.Accessories.Ranger
{
    public class ReconScope : ConfigurableItemRework
    {
        #region Parameters
        public static float DamageBonus => 0.1f;
        public static int CritChanceBonus => 10;
        public static int AggroReduction => 400;
        #endregion

        public override int[] ItemIds => [ItemID.ReconScope];
        public override void SetStaticDefaults() => ItemSets.DontUseVanillaEquipEffects[ItemID.ReconScope] = true;
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips) =>
            tooltips.ReplaceTooltipWith("Accessories.ReconScope.Tooltip");

        public override void UpdateAccessory(Item item, Player player, bool hideVisual)
        {
            if (!hideVisual && player.HeldItem.damage > 0)
                player.scope = true;
            player.GetDamage<GenericDamageClass>() += DamageBonus;
            player.GetCritChance<GenericDamageClass>() += CritChanceBonus;
            player.aggro -= AggroReduction;
        }
    }
}
