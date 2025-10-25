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
    public class SummonerEmblem : GlobalItem
    {
        public override bool IsLoadingEnabled(Mod mod) => Configs.instance.RemoveClasses;
        public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.SummonerEmblem;
        public override void SetStaticDefaults()
        {
            ItemSets.DontUseVanillaEquipEffects[ItemID.SummonerEmblem] = true;
        }

        public override void UpdateAccessory(Item item, Player player, bool hideVisual)
        {
            player.Roots().ModifyHitNPCWithProjectileFuncs.Add(EmblemScaling);
        }

        NPC.HitModifiers EmblemScaling(Player player, Projectile projectile, NPC npc, NPC.HitModifiers modifiers)
        {
            if (projectile.IsMinionOrSentryRelated)
                player.Roots().AdditiveDamageMultipliersToApplyOnHit += 0.15f;
            return modifiers;
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            tooltips.ReplaceTooltipWith("Accessories.SummonerEmblem.Tooltip");
        }
    }
}
