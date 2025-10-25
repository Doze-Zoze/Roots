using Microsoft.Xna.Framework;
using Roots.Players;
using Roots.Utilities;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Roots.Items.Accessories.Magic
{
    public class ManaFlower : GlobalItem
    {
        public override bool IsLoadingEnabled(Mod mod) => Configs.instance.ManaChanges;
        public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.ManaFlower;
        public override void SetStaticDefaults()
        {
            ItemSets.DontUseVanillaEquipEffects[ItemID.ManaFlower] = true;
        }

        public override void UpdateEquip(Item item, Player player)
        {
            player.GetModPlayer<RootsPlayer>().manaFlowerReduction *= 0.75f;
            player.manaRegenCount += 40;
            HashSet<int> projToHide = [ProjectileID.TerraBlade2Shot,ProjectileID.Starfury,ProjectileID.EnchantedBeam,ProjectileID.TrueNightsEdge];
            foreach (var item1 in projToHide)
            {
                ProjSets.ManaSpawnedProjectile[item1] = true;
            }
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
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
                tooltips.Insert(tooltipIndex, new TooltipLine(Mod, "Tooltip", RootsUtils.GetLocalizedTextValue("Accessories.ManaFlower.Tooltip")));
        }
    }
}
