using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Roots.Projectiles.Accessories
{
    public class ManaCloakStar : GlobalProjectile
    {
        public override bool AppliesToEntity(Projectile entity, bool lateInstantiation) => entity.type == ProjectileID.ManaCloakStar;

        public override void SetDefaults(Projectile entity)
        {
            entity.penetrate = 1;
        }
        public override void OnKill(Projectile projectile, int timeLeft)
        {
            if (Main.player[projectile.owner].starCloakCooldown == 0)
            {
                Main.player[projectile.owner].starCloakCooldown = 60;
                int number2 = Item.NewItem(projectile.GetSource_DropAsItem(), projectile.Center, 4143);
                if (Main.netMode == NetmodeID.MultiplayerClient)
                    NetMessage.SendData(MessageID.SyncItem, -1, -1, null, number2, 1f);
            }
        }
    }
}
