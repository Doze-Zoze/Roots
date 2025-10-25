using Microsoft.Xna.Framework;
using Roots.Utilities;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Roots.Projectiles.Weapons
{
    public class SkyDragonsFuryShot : GlobalProjectile
    {
        public override bool AppliesToEntity(Projectile entity, bool lateInstantiation) => entity.type == ProjectileID.MonkStaffT3_AltShot;

        public override bool PreKill(Projectile projectile, int timeLeft)
        {
            //Modified version of the vanilla kill code. Ensures that this counts as a Mana projectile.
            int smokeAmount = 3;
            int electricAmount = 10;
            int goreAmount = 0;
            if (projectile.scale >= 1f)
            {
                projectile.position = projectile.Center;
                projectile.width = (projectile.height = 144);
                projectile.Center = projectile.position;
                smokeAmount = 7;
                electricAmount = 30;
                goreAmount = 2;
                projectile.Damage();
            }

            for (int i = 0; i < smokeAmount; i++)
            {
                int d = Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.Smoke, 0f, 0f, 100, default, 1.5f);
                Main.dust[d].position = new Vector2(projectile.width / 2, 0f).RotatedBy(6.2831854820251465 * Main.rand.NextDouble()) * (float)Main.rand.NextDouble() + projectile.Center;
            }

            for (int i = 0; i < electricAmount; i++)
            {
                int d = Dust.NewDust(projectile.Center, projectile.width, projectile.height, DustID.Electric, 0f, 0f, 0, default, 1.5f);
                Main.dust[d].position = new Vector2(projectile.width / 2, 0f).RotatedBy(6.2831854820251465 * Main.rand.NextDouble()) * (float)Main.rand.NextDouble() + projectile.Center;
                Main.dust[d].noGravity = true;
                Dust dust2 = Main.dust[d];
                dust2.velocity *= 1f;
            }

            for (int i = 0; i < goreAmount; i++)
            {
                int g = Gore.NewGore(projectile.GetSource_FromThis(), projectile.position + new Vector2((projectile.width * Main.rand.Next(100)) / 100f, (projectile.height * Main.rand.Next(100)) / 100f) - Vector2.One * 10f, default, Main.rand.Next(61, 64));
                Gore gore2 = Main.gore[g];
                gore2.velocity *= 0.3f;
                Main.gore[g].velocity.X += (float)Main.rand.Next(-10, 11) * 0.05f;
                Main.gore[g].velocity.Y += (float)Main.rand.Next(-10, 11) * 0.05f;
            }

            //Checking for other electrospheres in the area and spawning the sphere
            if (Main.myPlayer == projectile.owner)
            {
                Rectangle sphereHitboxToCheck = new Rectangle((int)projectile.Center.X - 40, (int)projectile.Center.Y - 40, 80, 80);
                for (int i = 0; i < 1000; i++)
                {
                    if (i != projectile.whoAmI && Main.projectile[i].active && Main.projectile[i].owner == projectile.owner && Main.projectile[i].type == ProjectileID.Electrosphere && Main.projectile[i].getRect().Intersects(sphereHitboxToCheck))
                    {
                        Main.projectile[i].ai[1] = 1f;
                        Main.projectile[i].velocity = (projectile.Center - Main.projectile[i].Center) / 5f;
                        Main.projectile[i].netUpdate = true;
                    }
                }

                int projID = Projectile.NewProjectile(projectile.GetSource_FromThis(), projectile.Center.X, projectile.Center.Y, 0f, 0f, ProjectileID.Electrosphere, projectile.damage, 0f, projectile.owner);
                Main.projectile[projID].timeLeft = 30 * Main.rand.Next(2, 6);
                Main.projectile[projID].localAI[0] = SoundEngine.PlaySound(SoundID.DD2_SkyDragonsFuryCircle, projectile.Center).ToFloat();
                Main.projectile[projID].Roots().isManaProjectile = true;
            }
            return false;
        }
    }
}
