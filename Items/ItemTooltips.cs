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
                if (tooltip.Name == "CritChance" && (Main.LocalPlayer.GetCritChance(item.DamageType) + item.crit + addedCrit == 0))
                    tooltip.Hide();
                if (tooltip.Name == "JourneyResearch")
                    tooltip.Hide();
            }

            if (ProjectileID.Sets.MinionTargettingFeature[item.shoot])
            {
                RootsUtils.AppendTooltipWith(tooltips, "Roots.Tips.SummonManaCost");
            }
        }
    }
}
