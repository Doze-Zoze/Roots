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
    public class ManaCloak : GlobalItem
    {
        public override bool IsLoadingEnabled(Mod mod) => Configs.instance.ManaChanges;
        public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.ManaCloak;
        public override void SetStaticDefaults()
        {
            ItemSets.DontUseVanillaEquipEffects[ItemID.ManaCloak] = true;
        }

        public override void UpdateEquip(Item item, Player player)
        {
            player.starCloakItem = item;
            player.starCloakItem_manaCloakOverrideItem = item;
            player.manaMagnet = true;
            player.magicCuffs = true;
            player.Roots().OnHitNPCWithProjectileFuncs.Add(OnHitNPC);
        }

        void OnHitNPC(Player player, Projectile proj, NPC target, NPC.HitInfo HitInfo, int dmgDone)
        {

            if (player.starCloakItem_manaCloakOverrideItem is not null && player.Roots().TimeSinceManaCloakStarAttack >= 180 && proj.Roots().isManaProjectile)
            {
                player.Roots().TimeSinceManaCloakStarAttack = 0;
                int baseDamage = 90;
                Vector2 spawnPos = target.Center + new Vector2(Main.rand.Next(-400, 400), Main.rand.Next(-1600, -1000));
                var p = Projectile.NewProjectileDirect(player.GetSource_Accessory(player.starCloakItem_manaCloakOverrideItem), spawnPos, 18 * target.DirectionFrom(spawnPos), ProjectileID.ManaCloakStar, baseDamage.ScaledWithDifficulty(), 5f, player.whoAmI, 0f, target.Center.Y);
                p.MaxUpdates = 3;
                p.timeLeft *= p.MaxUpdates;
            }
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            int tooltipIndex = 0;
            for (var i = 0; i < tooltips.Count; i++)
            {
                var tooltip = tooltips[i];
                if (tooltip.Name.Contains("Tooltip"))
                {
                    tooltip.Hide();
                    tooltipIndex = i;
                }
            }
            if (tooltipIndex > 0)
                tooltips.Insert(tooltipIndex, new TooltipLine(Mod, "Tooltip", RootsUtils.GetLocalizedTextValue("Accessories.ManaCloak.Tooltip")));
        }
    }
}
