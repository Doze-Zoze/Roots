using Microsoft.Xna.Framework;
using Roots.Projectiles;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Roots.Items.Weapons
{
    public class TrueNightsEdge : GlobalItem
    {
        public override bool IsLoadingEnabled(Mod mod) => Configs.instance.ManaChanges;
        public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.TrueNightsEdge;
        public override void SetStaticDefaults()
        {
            ItemSets.DontConsumeManaOnSwing[ItemID.TrueNightsEdge] = true;
        }

        public override void SetDefaults(Item item)
        {
            item.mana = 15;
        }
        public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.CheckMana((int)Math.Max(0, item.mana * player.manaCost), true))
            {
                return true;
            }
            Projectile.NewProjectile(source, player.MountedCenter, new Vector2(player.direction, 0f), ProjectileID.NightsEdge, damage, knockback, player.whoAmI, (float)player.direction * player.gravDir, player.itemAnimationMax, player.GetAdjustedItemScale(item));
            var p = Projectile.NewProjectileDirect(source, player.MountedCenter, Vector2.Zero, type, damage / 2, knockback, player.whoAmI, (float)player.direction * player.gravDir, 32f, player.GetAdjustedItemScale(item));
            p.timeLeft = 50;
            p.GetGlobalProjectile<RootsGlobalProjectile>().isManaProjectile = false;
            NetMessage.SendData(MessageID.PlayerControls, -1, -1, null, player.whoAmI);
            return false;
        }
    }
}
