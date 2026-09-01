using Roots.Config;
using RootsBeta.Utilities;
using RootsCore;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RootsBeta.Items.Accessories.Magic
{
    public class SorcererEmblem : GlobalItem
    {
        #region Parameters
        public static float DamageBoost => 0.15f;
        #endregion

        public override bool IsLoadingEnabled(Mod mod) => RootsModConfig.Instance.RemoveClasses;
        public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.SorcererEmblem;
        public override void SetStaticDefaults() => ItemSets.DontUseVanillaEquipEffects[ItemID.SorcererEmblem] = true;
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips) =>
            tooltips.ReplaceTooltipWith("Accessories.SorcererEmblem.Tooltip");

        public override void UpdateAccessory(Item item, Player player, bool hideVisual)
        {
            player.Roots().ModifyHitNPCWithProjectileFuncs.Add(EmblemScaling);
        }

        private NPC.HitModifiers EmblemScaling(Player player, Projectile projectile, NPC npc, NPC.HitModifiers modifiers)
        {
            if (projectile.Roots().IsManaProjectile)
                player.Roots().AdditiveDamageMultipliersToApplyOnHit += DamageBoost;
            return modifiers;
        }
    }
}
