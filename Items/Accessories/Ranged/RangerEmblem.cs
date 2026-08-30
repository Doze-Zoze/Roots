using RootsBeta.Utilities;
using RootsCore;
using System.Collections.Generic;
using RootsBeta.Players;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RootsBeta.Items.Accessories.Ranger
{
    public class RangerEmblem : GlobalItem
    {
        #region Parameters
        public static float DamageBonus => 0.15f;
        #endregion

        public override bool IsLoadingEnabled(Mod mod) => Configs.Instance.RemoveClasses;
        public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.RangerEmblem;
        public override void SetStaticDefaults() => ItemSets.DontUseVanillaEquipEffects[ItemID.RangerEmblem] = true;
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips) =>
            tooltips.ReplaceTooltipWith("Accessories.RangerEmblem.Tooltip");

        public override void UpdateAccessory(Item item, Player player, bool hideVisual)
        {
            player.Roots().ModifyHitNPCWithProjectileFuncs.Add(EmblemScaling);
        }

        private NPC.HitModifiers EmblemScaling(Player player, Projectile projectile, NPC npc, NPC.HitModifiers modifiers)
        {
            if (player.Distance(npc.Center) > RootsPlayer.CloseRangeDistance)
                player.Roots().AdditiveDamageMultipliersToApplyOnHit += DamageBonus;
            return modifiers;
        }
    }
}
