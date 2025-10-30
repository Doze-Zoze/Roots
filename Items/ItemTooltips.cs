using Roots.Utilities;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Roots.Items
{
    public partial class RootsGlobalItem : GlobalItem
    {
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            foreach (var tooltip in tooltips)
            {
                float addedCrit = 0;
                item.ModItem?.ModifyWeaponCrit(Main.LocalPlayer, ref addedCrit);
                if (tooltip.Name == "CritChance")
                {
                    if (ProjectileID.Sets.MinionTargettingFeature[item.shoot])
                        tooltip.Text = $"{Main.LocalPlayer.maxMinions - (int)(Main.LocalPlayer.statManaMax2 / 40f) - Main.LocalPlayer.slotsMinions} Empty Minion Slots\n" + RootsUtils.GetLocalizedTextValue("Tips.SummonManaCost");
                }
                if (tooltip.Name == "JourneyResearch")
                    tooltip.Hide();
            }

            if (item.type == ItemID.MagmaStone)
                tooltips.ReplaceTooltipWith("Accessories.MagmaStone.Tooltip");
        }
    }
}
