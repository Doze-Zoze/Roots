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
    public class ReconScope : GlobalItem
    {
        public override bool IsLoadingEnabled(Mod mod) => Configs.instance.RemoveClasses;
        public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.ReconScope;
        public override void SetStaticDefaults()
        {
            ItemSets.DontUseVanillaEquipEffects[ItemID.ReconScope] = true;
        }

        public override void UpdateAccessory(Item item, Player player, bool hideVisual)
        {
            if (!hideVisual && player.HeldItem.damage > 0)
                player.scope = true;
            player.GetDamage<GenericDamageClass>() += 0.10f;
            player.GetCritChance <GenericDamageClass>() += 10;
            player.aggro -= 400;
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            tooltips.ReplaceTooltipWith("Accessories.ReconScope.Tooltip");
        }
    }
}
