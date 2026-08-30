using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using RootsCore;

namespace RootsBeta.Items.Weapons
{
    public class Starfury : GlobalItem
    {
        #region Parameters
        public static int ManaCost => 20;
        public static int StarsSpawned => 2;
        public static float Inaccuracy => 100f;
        public static ((int Min, int Max) X, (int Min, int Max) Y) StarOffset => ((500, 900), (600, 800));
        public static float MinVelocityMultiplier => 0.5f;
        public static float MaxVelocityMultiplier => 1f;
        public static float StarDamageMultiplier => 0.75f;
        public static int StaticImmunityFrames => 20;
        #endregion

        public override bool IsLoadingEnabled(Mod mod) => Configs.Instance.ManaChanges;
        public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.Starfury;

        public override void SetStaticDefaults()
        {
            ItemSets.DontConsumeManaOnSwing[ItemID.Starfury] = true;
        }

        public override void SetDefaults(Item item)
        {
            item.mana = ManaCost;
        }

        public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (!player.CheckMana((int)Math.Max(0, item.mana * player.manaCost), true)) return false;
            float collisionPoint = Main.MouseWorld.Y;
            var point = Main.MouseWorld;
            const int tilesAboveCheckedForCollisions = 34;
            for (var i = 0; i < tilesAboveCheckedForCollisions; i++)
            {
                if (!Collision.SolidCollision(point - Vector2.One * 0.5f, 1, 1))
                {
                    collisionPoint = point.Y;
                    break;
                }
                point -= new Vector2(0, 16);
            }
            for (var i = 0; i < StarsSpawned; i++)
            {
                var target = Main.MouseWorld + Main.rand.NextVector2Circular(Inaccuracy, Inaccuracy);
                var spawn = new Vector2(
                    target.X - Main.rand.Next(StarOffset.X.Min, StarOffset.X.Max) * player.direction,
                    player.Center.Y - Main.rand.Next(StarOffset.Y.Min, StarOffset.Y.Max));
                var p = Projectile.NewProjectileDirect(source,
                    spawn,
                    spawn.DirectionTo(target) * velocity.Length() * Main.rand.NextFloat(MinVelocityMultiplier, MaxVelocityMultiplier),
                    type,
                    (int)(player.GetWeaponDamage(item) * StarDamageMultiplier),
                    knockback,
                    player.whoAmI,
                    ai1: collisionPoint);
                p.usesIDStaticNPCImmunity = true;
                p.idStaticNPCHitCooldown = StaticImmunityFrames;
            }
            return false;
        }
    }
}
