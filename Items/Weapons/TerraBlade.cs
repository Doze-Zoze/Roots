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
    public class TerraBlade : ConfigurableItemRework
    {
        #region Parameters
        public static int ManaCost => 10;
        #endregion

        public override int[] ItemIds => [ItemID.TerraBlade];

        public override void SetStaticDefaults()
        {
            ItemSets.DontConsumeManaOnSwing[ItemID.TerraBlade] = true;
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
                ProjectileID.TerraBlade2, damage, knockback, player.whoAmI, player.direction * player.gravDir,
                player.itemAnimationMax, player.GetAdjustedItemScale(item));
            NetMessage.SendData(MessageID.PlayerControls, -1, -1, null, player.whoAmI);
            return false;
        }
    }
}
