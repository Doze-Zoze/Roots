using Roots.Config;
using RootsBeta.Utilities;
using RootsCore;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.ItemDropRules;
using Terraria.ID;
using Terraria.ModLoader;

namespace RootsBeta.Items.Accessories.Ranger
{
    public class RifleScope : ConfigurableItemRework<RifleScope>, IConfigurableContent<RifleScope>
    {
        public static ConfigGroup ConfigGroups => ConfigGroup.AccessoryReworks;

        public override int[] ItemIds => [ItemID.RifleScope];
        public override void SetStaticDefaults() => ItemSets.DontUseVanillaEquipEffects[ItemID.RifleScope] = true;
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips) =>
            tooltips.ReplaceTooltipWith("Accessories.RifleScope.Tooltip");

        public override void UpdateAccessory(Item item, Player player, bool hideVisual)
        {
            if (!hideVisual && player.HeldItem.damage > 0)
                player.scope = true;
        }
    }
}
