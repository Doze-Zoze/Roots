using Microsoft.Xna.Framework;
using RootsBeta.Projectiles;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using RootsCore;
using Roots.Config;

namespace RootsBeta.Items.Weapons
{
    public class TrueNightsEdge : ConfigurableItemRework<TrueNightsEdge>, IConfigurableContent<TrueNightsEdge>
    {
        public static ConfigGroup ConfigGroups => ConfigGroup.WeaponReworks;

        #region Parameters

        public static int ManaCost => 15;
        public static float NoManaProjectileDamage => 0.5f;
        public static int NoManaProjectileLifetimeFrames => 50;
        #endregion

        public override int[] ItemIds => [ItemID.TrueNightsEdge];

        public override void SetStaticDefaults()
        {
            ItemSets.DontConsumeManaOnSwing[ItemID.TrueNightsEdge] = true;
        }

        public override void SetDefaults(Item item)
        {
            item.mana = ManaCost;
        }

        public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.CheckMana((int)Math.Max(0, item.mana * player.manaCost), true))
            {
                return true;
            }
            Projectile.NewProjectile(source, player.MountedCenter, new Vector2(player.direction, 0f),
                ProjectileID.NightsEdge, damage, knockback, player.whoAmI, player.direction * player.gravDir,
                player.itemAnimationMax, player.GetAdjustedItemScale(item));
            var p = Projectile.NewProjectileDirect(source, player.MountedCenter, Vector2.Zero, type, (int)(damage * NoManaProjectileDamage),
                knockback, player.whoAmI, player.direction * player.gravDir, 32f,
                player.GetAdjustedItemScale(item));
            p.timeLeft = NoManaProjectileLifetimeFrames;
            p.GetGlobalProjectile<RootsGlobalProjectile>().IsManaProjectile = false;
            NetMessage.SendData(MessageID.PlayerControls, -1, -1, null, player.whoAmI);
            return false;
        }
    }
}
