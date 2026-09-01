using Roots.Config;
using RootsBeta.Utilities;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RootsBeta.Items.Consumables
{
    public class ManaPotions : ConfigurableItemRework
    {
        public override int[] ItemIds =>
        [
            ItemID.LesserManaPotion,
            ItemID.ManaPotion,
            ItemID.GreaterManaPotion,
            ItemID.SuperManaPotion
        ];
        public override bool IsLoadingEnabled(Mod mod) => RootsModConfig.Instance.ManaChanges;
        public override bool AppliesToEntity(Item item, bool lateInstantiation) => ItemIds.Contains(item.type);
        public override bool CanUseItem(Item item, Player player) => !player.manaSick;
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips) =>
            tooltips.ReplaceTooltipWith("Consumables.ManaPotions.Tooltip");
    }
}
