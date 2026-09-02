using RootsBeta.Utilities;
using System.Collections.Generic;
using RootsCore;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Roots.Config;

namespace RootsBeta.Items.Accessories.Magic
{
    public class NaturesGift : ConfigurableItemRework<NaturesGift>, IConfigurableContent<NaturesGift>
    {
        public static ConfigGroup ConfigGroups => ConfigGroup.AccessoryReworks;

        #region Parameters
        private static float ManaCostReduction => 0.15f;
        #endregion

        public override int[] ItemIds => [ItemID.NaturesGift];
        public override void SetStaticDefaults() => ItemSets.DontUseVanillaEquipEffects[ItemID.NaturesGift] = true;
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips) =>
            tooltips.ReplaceTooltipWith("Accessories.NaturesGift.Tooltip");

        public override void UpdateEquip(Item item, Player player)
        {
            player.manaCost -= ManaCostReduction;
        }
    }
}
