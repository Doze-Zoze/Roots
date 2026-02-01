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
    public class MagicCuffs : GlobalItem
    {
        public override bool IsLoadingEnabled(Mod mod) => Configs.instance.ManaChanges;
        public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.MagicCuffs || item.type == ItemID.CelestialCuffs;

        public override void SetDefaults(Item entity)
        {
            entity.defense = 1;
        }
        public override void UpdateEquip(Item item, Player player)
        {
            player.manaRegenDelayBonus += 1f;
            player.manaRegenBonus += 25;
        }
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            tooltips.AppendTooltipWith("Accessories.MagicCuffs.Tooltip");
        }
    }
}
