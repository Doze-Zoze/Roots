using RootsBeta.Items;
using RootsBeta.Players;
using RootsBeta.Projectiles;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace RootsBeta.Utilities
{
    public static class RootsUtils
    {
        public static RootsPlayer Roots(this Player player) => player.GetModPlayer<RootsPlayer>();
        public static RootsGlobalProjectile Roots(this Projectile proj) => proj.GetGlobalProjectile<RootsGlobalProjectile>();
        public static RootsGlobalItem Roots(this Item item) => item.GetGlobalItem<RootsGlobalItem>();

        public static int ScaledWithDifficulty(this int dmg, float amountPerDifficulty = 1f) => (int)(Main.masterMode ? dmg * amountPerDifficulty * 2 : Main.expertMode ? dmg * amountPerDifficulty : dmg);

        public static float Sine0To1(float input) => (MathF.Sin(input) + 1) * 0.5f;
        
        public static string GetLocalizedTextValue(string path)
        {
            return Language.GetTextValue("Mods.RootsBeta." + path);
        }

        public static LocalizedText GetLocalizedText(string path)
        {
            return Language.GetText("Mods.RootsBeta." + path);
        }

        extension(List<TooltipLine> tooltips)
        {
            public void ReplaceTooltipWith(string path, bool rootsPath = true)
            {
                int tooltipIndex = 0;
                for (var i = 0; i < tooltips.Count; i++)
                {
                    var tooltip = tooltips[i];
                    if (tooltip.Name.Contains("Consumable"))
                    {
                        tooltipIndex = i + 1;
                    }
                    if (tooltip.Name.Contains("Material"))
                    {
                        tooltipIndex = i+1;
                    }
                    if (tooltip.Name.Contains("Tooltip"))
                    {
                        tooltip.Hide();
                        tooltipIndex = i;
                    }
                }

                if (tooltipIndex <= 0) return;
                if (tooltipIndex < tooltips.Count)
                    tooltips.Insert(tooltipIndex, new TooltipLine(ModLoader.GetMod("RootsBeta"), "Tooltip", rootsPath ? GetLocalizedTextValue(path) : Language.GetTextValue(path)));
                else
                    tooltips.Add(new TooltipLine(ModLoader.GetMod("RootsBeta"), "Tooltip", rootsPath ? GetLocalizedTextValue(path) : Language.GetTextValue(path)));
            }

            public void AppendTooltipWith(string path, int offset = 0, bool rootsPath = true)
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
                tooltipIndex -= offset;
                tooltips.Insert(tooltipIndex > 0 ? tooltipIndex : tooltips.Count - 1, new TooltipLine(ModLoader.GetMod("RootsBeta"), "Tooltip", rootsPath ? GetLocalizedTextValue(path) : Language.GetTextValue(path)));
            }
        }
    }
}
