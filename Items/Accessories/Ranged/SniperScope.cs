using RootsBeta.Utilities;
using RootsCore;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RootsBeta.Items.Accessories.Magic
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
