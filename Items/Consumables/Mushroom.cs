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

namespace RootsBeta.Items.Consumables
{
    public class Mushroom : GlobalItem
    {
        public override bool IsLoadingEnabled(Mod mod) => Configs.instance.LifeChanges;
        public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.Mushroom;

        public override void SetDefaults(Item entity)
        {
            entity.healLife = 20;
        }
        public override void ModifyPotionDelay(Item item, Player player, ref int baseDelay)
        {
            baseDelay = 15 * 60;
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            tooltips.ReplaceTooltipWith("Consumables.Mushroom.Tooltip");
        }
    }
}
