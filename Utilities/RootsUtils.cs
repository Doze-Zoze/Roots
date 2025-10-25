using Roots.Items;
using Roots.Players;
using Roots.Projectiles;
using System.Collections.Generic;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Roots.Utilities
{
    public static class RootsUtils
    {
        public static string GetLocalizedTextValue(string path)
        {
            return Language.GetTextValue("Mods.Roots." + path);
        }
        public static LocalizedText GetLocalizedText(string path)
        {
            return Language.GetText("Mods.Roots." + path);
        }

        public static void ReplaceTooltipWith(this List<TooltipLine> tooltips, string path)
        {
            int tooltipIndex = 0;
            for (var i = 0; i < tooltips.Count; i++)
            {
                var tooltip = tooltips[i];
                if (tooltip.Name.Contains("Tooltip"))
                {
                    tooltip.Hide();
                    tooltipIndex = i;
                }
            }
            if (tooltipIndex > 0)
                tooltips.Insert(tooltipIndex, new TooltipLine(ModLoader.GetMod("Roots"), "Tooltip", GetLocalizedTextValue(path)));
        }
        public static void AppendTooltipWith(this List<TooltipLine> tooltips, string path)
        {
            int tooltipIndex = 0;
            for (var i = 0; i < tooltips.Count; i++)
            {
                var tooltip = tooltips[i];
                if (tooltip.Name.Contains("Tooltip"))
                {
                    tooltipIndex = i+1;
                }
                if (tooltip.Name.Contains("Prefix") && tooltipIndex == 0)
                {
                    tooltipIndex = i;
                }
                if (tooltip.Name.Contains("OneDrop") && tooltipIndex == 0)
                {
                    tooltipIndex = i;
                }
            }
            tooltips.Insert(tooltipIndex > 0 ? tooltipIndex : tooltips.Count - 1, new TooltipLine(ModLoader.GetMod("Roots"), "Tooltip", GetLocalizedTextValue(path)));
        }

        public static RootsPlayer Roots(this Player player) => player.GetModPlayer<RootsPlayer>();
        public static RootsGlobalProjectile Roots(this Projectile proj) => proj.GetGlobalProjectile<RootsGlobalProjectile>();
        public static RootsGlobalItem Roots(this Item item) => item.GetGlobalItem<RootsGlobalItem>();

        public static int ScaledWithDifficulty(this int dmg, float amountPerDifficulty = 1f) => (int)(Main.masterMode ? dmg * amountPerDifficulty * 2 : Main.expertMode ? dmg * amountPerDifficulty : dmg);
    }
}
