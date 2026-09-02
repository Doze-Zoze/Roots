using Roots.Config;
using RootsBeta.Utilities;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RootsBeta.Items.Consumables
{
    public class ManaPotions : ConfigurableItemRework<ManaPotions>, IConfigurableContent<ManaPotions>
    {
        public override int[] ItemIds =>
        [
            ItemID.LesserManaPotion,
            ItemID.ManaPotion,
            ItemID.GreaterManaPotion,
            ItemID.SuperManaPotion
        ];
        public override bool CanUseItem(Item item, Player player) => !player.manaSick;
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips) =>
            tooltips.ReplaceTooltipWith("Consumables.ManaPotions.Tooltip");
    }
}
