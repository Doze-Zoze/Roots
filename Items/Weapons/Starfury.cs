using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace RootsBeta.Items.Weapons
{
    public class Starfury : GlobalItem
    {
        public override bool IsLoadingEnabled(Mod mod) => Configs.instance.ManaChanges;
        public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.Starfury;
        public override void SetStaticDefaults()
        {
            ItemSets.DontConsumeManaOnSwing[ItemID.Starfury] = true;
        }

        public override void SetDefaults(Item item)
        {
            item.mana = 20;
        }
        public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.CheckMana((int)Math.Max(0, item.mana * player.manaCost), true))
            {
                float collisionPoint = Main.MouseWorld.Y;
                var point = Main.MouseWorld;
                for (var i = 0; i < 34; i++)
                {
                    if (!Collision.SolidCollision(point - Vector2.One * 0.5f, 1, 1))
                    {
                        collisionPoint = point.Y;
                        break;
                    }
                    else
                    {
                        point -= new Vector2(0, 16);
                    }
                }
                for (var i = 0; i < 2; i++)
                {
                    var target = Main.MouseWorld + Main.rand.NextVector2Circular(100f, 100f);
                    var spawn = new Vector2(target.X - Main.rand.Next(500, 900) * player.direction, player.Center.Y - Main.rand.Next(600, 800));
                    var p = Projectile.NewProjectileDirect(source, spawn, spawn.DirectionTo(target) * velocity.Length() * Main.rand.NextFloat(0.5f, 1f), type, (int)(player.GetWeaponDamage(item) * 0.75f), knockback, player.whoAmI, ai1: collisionPoint);
                    p.usesIDStaticNPCImmunity = true;
                    p.idStaticNPCHitCooldown = 20;
                }
            }
            return false;
        }
    }
}
