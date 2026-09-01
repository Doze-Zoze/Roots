using RootsBeta.Utilities;
using RootsCore;
using System.Collections.Generic;
using RootsBeta.Players;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Roots.Config;

namespace RootsBeta.Items.Accessories.Melee
{
    public class WarriorEmblem : ConfigurableItemRework
    {
        #region Parameters
        public static float DamageBonus => 0.15f;
        #endregion

        public override int[] ItemIds => [ItemID.WarriorEmblem];
        public override void SetStaticDefaults() => ItemSets.DontUseVanillaEquipEffects[ItemID.WarriorEmblem] = true;
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips) =>
            tooltips.ReplaceTooltipWith("Accessories.WarriorEmblem.Tooltip");

        public override void UpdateAccessory(Item item, Player player, bool hideVisual)
        {
            player.Roots().ModifyHitNPCFuncs.Add(EmblemScaling);
        }

        private NPC.HitModifiers EmblemScaling(Player player, NPC npc, NPC.HitModifiers modifiers)
        {
            if (player.Distance(npc.Center) <= RootsPlayer.CloseRangeDistance)
                player.Roots().AdditiveDamageMultipliersToApplyOnHit += DamageBonus;
            return modifiers;
        }
    }
}
