using RootsBeta.Utilities;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RootsBeta.Items
{
    public partial class RootsGlobalItem
    {
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            foreach (var tooltip in tooltips)
            {
                float addedCrit = 0;
                item.ModItem?.ModifyWeaponCrit(Main.LocalPlayer, ref addedCrit);
                if (tooltip.Name != "CritChance") continue;
                if (ProjectileID.Sets.MinionTargettingFeature[item.shoot])
                    tooltip.Text = 
                        RootsUtils.GetLocalizedText("Tips.SummonSlots").Format(Main.LocalPlayer.maxMinions -
                        (int)(Main.LocalPlayer.statManaMax2 / 40f) - Main.LocalPlayer.slotsMinions)
                        + $"\n" 
                        + RootsUtils.GetLocalizedTextValue("Tips.SummonManaCost");
            }
            
            //TODO - Make configurable
            if (item.type == ItemID.MagmaStone)
                tooltips.ReplaceTooltipWith("Accessories.MagmaStone.Tooltip");
        }
    }
}
