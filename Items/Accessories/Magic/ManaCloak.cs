using Microsoft.Xna.Framework;
using RootsBeta.Utilities;
using RootsCore;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RootsBeta.Items.Accessories.Magic
{
    public class ManaCloak : GlobalItem
    {
        #region Parameters
            public static int Defense => MagicCuffs.Defense;
            public static float ManaRegenDelayBonus => MagicCuffs.ManaRegenDelayBonus;
            public static int ManaRegen => MagicCuffs.ManaRegen;
            public static int FramesBetweenStarAttacks => 180;
            public static int Damage => 90;
            public static ((int Min, int Max) X, (int Min, int Max) Y) StarOffset => ((-400, 400), (-1600, -1000));
            public static int Velocity => 18;
            public static float Knockback => 5f;
            public static int StarUpdatesPerFrame => 3;
            public static int ManaStarConversionCooldownFrames => 60;
        #endregion

        public override bool IsLoadingEnabled(Mod mod) => Configs.Instance.ManaChanges;
        public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.ManaCloak;
        public override void SetStaticDefaults() => ItemSets.DontUseVanillaEquipEffects[ItemID.ManaCloak] = true;
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips) =>
            tooltips.ReplaceTooltipWith("Accessories.ManaCloak.Tooltip");

        public override void SetDefaults(Item entity)
        {
            entity.defense = Defense;
        }

        public override void UpdateEquip(Item item, Player player)
        {
            player.starCloakItem = item;
            player.starCloakItem_manaCloakOverrideItem = item;
            player.manaMagnet = true;
            player.magicCuffs = true;
            player.Roots().OnHitNPCWithProjectileFuncs.Add(OnHitNPC);
            player.manaRegenDelayBonus += ManaRegenDelayBonus;
            player.manaRegenBonus += ManaRegen;
        }

        private void OnHitNPC(Player player, Projectile proj, NPC target, NPC.HitInfo hitInfo, int dmgDone)
        {
            if (player.starCloakItem_manaCloakOverrideItem is null ||
                player.Roots().TimeSinceManaCloakStarAttack < FramesBetweenStarAttacks || !proj.Roots().IsManaProjectile) return;
            player.Roots().TimeSinceManaCloakStarAttack = 0;
            Vector2 spawnPos = target.Center + new Vector2(Main.rand.Next(StarOffset.X.Min, StarOffset.X.Max), Main.rand.Next(StarOffset.Y.Min, StarOffset.Y.Max));
            var p = Projectile.NewProjectileDirect(
                player.GetSource_Accessory(player.starCloakItem_manaCloakOverrideItem),
                spawnPos,
                Velocity * target.DirectionFrom(spawnPos),
                ProjectileID.ManaCloakStar,
                Damage.ScaledWithDifficulty(),
                Knockback,
                player.whoAmI,
                0f,
                target.Center.Y);
            p.MaxUpdates = StarUpdatesPerFrame;
            p.timeLeft *= p.MaxUpdates;
        }
    }
}
