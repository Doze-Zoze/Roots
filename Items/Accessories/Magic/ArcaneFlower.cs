using Microsoft.Xna.Framework;
using RootsBeta.Players;
using RootsBeta.Utilities;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace RootsBeta.Items.Accessories.Magic
{
    public class ArcaneFlower : GlobalItem
    {
        public override bool IsLoadingEnabled(Mod mod) => Configs.instance.ManaChanges;
        public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.ArcaneFlower;
        public override void SetStaticDefaults()
        {
            ItemSets.DontUseVanillaEquipEffects[ItemID.ArcaneFlower] = true;
        }

        public override void UpdateEquip(Item item, Player player)
        {
            float playerManaRatio = player.statMana / (float)player.statManaMax2;
            player.Roots().manaFlowerReduction *= MathHelper.Lerp(0.5f,1, playerManaRatio);
            player.manaRegenCount += (int)(120 * (1-playerManaRatio));
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
                tooltips.Insert(tooltipIndex, new TooltipLine(Mod, "Tooltip", RootsUtils.GetLocalizedTextValue("Accessories.ArcaneFlower.Tooltip")));
        }
    }
}
