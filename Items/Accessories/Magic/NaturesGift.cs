using RootsBeta.Utilities;
using System.Collections.Generic;
using RootsCore;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RootsBeta.Items.Accessories.Magic
{
    public class NaturesGift : GlobalItem
    {
        #region Parameters
        private static float ManaCostReduction => 0.15f;
        #endregion

        public override bool IsLoadingEnabled(Mod mod) => Configs.Instance.ManaChanges;
        public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.NaturesGift;
        public override void SetStaticDefaults() => ItemSets.DontUseVanillaEquipEffects[ItemID.NaturesGift] = true;
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips) =>
            tooltips.ReplaceTooltipWith("Accessories.NaturesGift.Tooltip");

        public override void UpdateEquip(Item item, Player player)
        {
            player.manaCost -= ManaCostReduction;
        }
    }
}
