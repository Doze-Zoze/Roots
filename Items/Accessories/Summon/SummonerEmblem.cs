using Roots.Config;
using RootsBeta.Utilities;
using RootsCore;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RootsBeta.Items.Accessories.Summon
{
    public class SummonerEmblem : ConfigurableItemRework
    {
        #region Parameter
        public static float DamageBonus => 0.15f;
        #endregion

        public override int[] ItemIds => [ItemID.SummonerEmblem];
        public override void SetStaticDefaults() => ItemSets.DontUseVanillaEquipEffects[ItemID.SummonerEmblem] = true;
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips) =>
            tooltips.ReplaceTooltipWith("Accessories.SummonerEmblem.Tooltip");

        public override void UpdateAccessory(Item item, Player player, bool hideVisual)
        {
            player.Roots().ModifyHitNPCWithProjectileFuncs.Add(EmblemScaling);
        }

        private NPC.HitModifiers EmblemScaling(Player player, Projectile projectile, NPC npc, NPC.HitModifiers modifiers)
        {
            if (projectile.IsMinionOrSentryRelated)
                player.Roots().AdditiveDamageMultipliersToApplyOnHit += DamageBonus;
            return modifiers;
        }
    }
}
