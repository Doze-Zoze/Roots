using RootsBeta.Utilities;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RootsBeta.Items.Consumables
{
    public class ManaPotions : GlobalItem
    {
        private static List<int> ItemsToCount =>
        [
            ItemID.LesserManaPotion,
            ItemID.ManaPotion,
            ItemID.GreaterManaPotion,
            ItemID.SuperManaPotion
        ];
        public override bool IsLoadingEnabled(Mod mod) => Configs.Instance.ManaChanges;
        public override bool AppliesToEntity(Item item, bool lateInstantiation) => ItemsToCount.Contains(item.type);
        public override bool CanUseItem(Item item, Player player) => !player.manaSick;
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips) =>
            tooltips.ReplaceTooltipWith("Consumables.ManaPotions.Tooltip");
    }
}
