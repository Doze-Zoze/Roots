using Microsoft.Xna.Framework;
using Roots.Config;
using RootsCore;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace RootsBeta.Items.Weapons
{
    public class StarWrath : ConfigurableItemRework
    {
        #region Parameters
        public static int ManaCost => 10;
        public static int ShootSpeed => 16;
        public static int StarsSpawned => 4;
        public static float Inaccuracy => 64f;
        public static ((int Min, int Max) X, (int Min, int Max) Y) StarOffset => ((100, 900), (600, 800));
        public static float MinVelocityMultiplier => 0.5f;
        public static float MaxVelocityMultiplier => 1f;
        public static int SolidCollisionHeightOffset => 700;
        public static int StarUpdatesPerFrame => 2;
        #endregion

        public override int[] ItemIds => [ItemID.StarWrath];

        public override void SetStaticDefaults()
        {
            ItemSets.DontConsumeManaOnSwing[ItemID.StarWrath] = true;
        }

        public override void SetDefaults(Item item)
        {
            item.mana = ManaCost;
            item.shootSpeed = ShootSpeed;
        }

        public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (!player.CheckMana((int)Math.Max(0, item.mana * player.manaCost), true)) return false;
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
                    player.GetWeaponDamage(item),
                    knockback,
                    player.whoAmI,
                    ai1: player.Center.Y + SolidCollisionHeightOffset);
                p.usesLocalNPCImmunity = true;
                p.localNPCHitCooldown = -1;
                p.penetrate = -1;
                p.MaxUpdates = StarUpdatesPerFrame;
            }
            return false;
        }
    }
}
