using Roots.Config;
using RootsBeta.Utilities;
using RootsCore;
using System.Collections.Generic;
using RootsBeta.Items.Accessories.Melee;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RootsBeta.Items.Accessories.Ranger
{
    public class MoltenQuiver : ConfigurableItemRework<MoltenQuiver>, IConfigurableContent<MoltenQuiver>
    {
        public static ConfigGroup ConfigGroups => ConfigGroup.AccessoryReworks;
        public override int[] ItemIds => [ItemID.MoltenQuiver];

        public override void UpdateAccessory(Item item, Player player, bool hideVisual)
        {
            if (ConfigHelpers.ConfigEnabled<MagmaStone>())
                player.Roots().PhysicalOnHitNPCFuncs.Add(MagmaStone.OnHit);
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (ConfigHelpers.ConfigEnabled<MagmaStone>()) 
                tooltips.AppendTooltipWith("Accessories.MagmaStone.Tooltip", 1);
        }
    }
}