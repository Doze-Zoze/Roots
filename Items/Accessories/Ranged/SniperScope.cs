using Roots.Config;
using RootsBeta.Utilities;
using RootsCore;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RootsBeta.Items.Accessories.Ranger
{
    public class SniperScope : ConfigurableItemRework
    {
        #region Parameters
        public static float DistancePerCritBoost => 32f;
        #endregion

        public override int[] ItemIds => [ItemID.SniperScope];
        public override void SetStaticDefaults() => ItemSets.DontUseVanillaEquipEffects[ItemID.SniperScope] = true;
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips) =>
            tooltips.ReplaceTooltipWith("Accessories.SniperScope.Tooltip");

        public override void UpdateAccessory(Item item, Player player, bool hideVisual)
        {
            if (!hideVisual && player.HeldItem.damage > 0)
                player.scope = true;
            player.Roots().ModifyHitNPCWithProjectileFuncs.Add(SniperScopeScaling);
        }

        private NPC.HitModifiers SniperScopeScaling(Player player, Projectile projectile, NPC npc, NPC.HitModifiers modifiers)
        {
            projectile.CritChance += (int)(player.Distance(npc.Center) / DistancePerCritBoost);
            return modifiers;
        }
    }
}
