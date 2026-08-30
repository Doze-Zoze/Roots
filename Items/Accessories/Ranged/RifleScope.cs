using RootsBeta.Utilities;
using RootsCore;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RootsBeta.Items.Accessories.Ranger
{
    public class RifleScope : GlobalItem
    {
        public override bool IsLoadingEnabled(Mod mod) => Configs.Instance.RemoveClasses;
        public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.RifleScope;
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
