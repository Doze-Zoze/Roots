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
    public class SniperScope : GlobalItem
    {
        public override bool IsLoadingEnabled(Mod mod) => Configs.instance.RemoveClasses;
        public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.SniperScope;
        public override void SetStaticDefaults()
        {
            ItemSets.DontUseVanillaEquipEffects[ItemID.SniperScope] = true;
        }

        public override void UpdateAccessory(Item item, Player player, bool hideVisual)
        {
            if (!hideVisual && player.HeldItem.damage > 0)
                player.scope = true;
            player.Roots().ModifyHitNPCWithProjectileFuncs.Add(SniperScopeScaling);
        }

        NPC.HitModifiers SniperScopeScaling(Player player, Projectile projectile, NPC npc, NPC.HitModifiers modifiers)
        {
            projectile.CritChance += (int)(player.Distance(npc.Center) / 32f);
            return modifiers;
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            tooltips.ReplaceTooltipWith("Accessories.SniperScope.Tooltip");
        }
    }
}
