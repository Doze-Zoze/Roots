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

namespace Roots.Items.Consumables
{
    public class Mushroom : GlobalItem
    {
        public override bool IsLoadingEnabled(Mod mod) => Configs.instance.LifeChanges;
        public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.Mushroom;

        public override void SetDefaults(Item entity)
        {
            entity.buffTime = 15 * 60;
            entity.buffType = BuffID.PotionSickness;
            entity.healLife = 20;
        }

        public override void OnConsumeItem(Item item, Player player)
        {
           if (player.HasBuff(BuffID.PotionSickness))
               player.buffTime[player.FindBuffIndex(BuffID.PotionSickness)] = 15 * 60;
            player.potionDelay = 15 * 60;
            player.potionDelayTime = 15 * 60;
            if (player.respawnTimer > 60 * 5)
                player.respawnTimer = 60 * 5;
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            return;
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
                tooltips.Insert(tooltipIndex, new TooltipLine(Mod, "Tooltip", RootsUtils.GetLocalizedTextValue("Accessories.ManaCloak.Tooltip")));
        }
    }
}
