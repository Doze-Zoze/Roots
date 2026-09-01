using Roots.Config;
using RootsBeta.Items.Accessories.Magic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using tModPorter;

namespace RootsBeta.Projectiles.Accessories
{
    public class ManaCloakStar : GlobalProjectile
    {
        public override bool IsLoadingEnabled(Mod mod) => ConfigHelpers.ConfigEnabled(typeof(ManaCloak).Name);
        public override bool AppliesToEntity(Projectile entity, bool lateInstantiation) => entity.type == ProjectileID.ManaCloakStar;

        public override void SetDefaults(Projectile entity)
        {
            entity.penetrate = 1;
        }
        public override void OnKill(Projectile projectile, int timeLeft)
        {
            if (Main.player[projectile.owner].starCloakCooldown != 0) return;
            Main.player[projectile.owner].starCloakCooldown = ManaCloak.ManaStarConversionCooldownFrames;
            int number2 = Item.NewItem(projectile.GetSource_DropAsItem(), projectile.Center, ItemID.ManaCloakStar);
            if (Main.netMode == NetmodeID.MultiplayerClient)
                NetMessage.SendData(MessageID.SyncItem, -1, -1, null, number2, 1f);
        }
    }
}
