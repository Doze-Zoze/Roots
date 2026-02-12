using RootsBeta.Utilities;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RootsBeta.Items.Consumables
{
    public class ManaPotions : GlobalItem
    {
        static List<int> ItemsToCount => new List<int> {
                ItemID.LesserManaPotion,
                ItemID.ManaPotion,
                ItemID.GreaterManaPotion,
                ItemID.SuperManaPotion
            };
        public override bool IsLoadingEnabled(Mod mod) => Configs.instance.ManaChanges;
        public override bool AppliesToEntity(Item item, bool lateInstantiation) => ItemsToCount.Contains(item.type);

        public override bool CanUseItem(Item item, Player player)
        {
            return !player.manaSick;
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            tooltips.ReplaceTooltipWith("Consumables.ManaPotions.Tooltip");
        }
    }
}
