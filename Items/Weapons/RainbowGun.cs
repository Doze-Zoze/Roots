using Daybreak.Common.Rendering;
using Microsoft.CodeAnalysis;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Roots.Config;
using Roots.Graphics;
using Roots.Graphics.Shaders;
using RootsBeta.Utilities;
using RootsCore;
using RootsCore.Extensions;
using RootsCore.ParticleSystem;
using RootsCore.ParticleTextures;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using static RootsCore.DrawLayerSystem;

namespace Roots.Items.Weapons
{
    public class RainbowGun : ConfigurableItemRework<RainbowGun>
    {
        public override int[] ItemIds => [ItemID.RainbowGun];

        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[ItemID.RainbowGun] = true;
        }
        public override void SetDefaults(Item entity)
        {
            base.SetDefaults(entity);
            entity.shoot = ModContent.ProjectileType<RainbowGunRainbow>();
            entity.UseSound = null;
        }

        public override bool AltFunctionUse(Item item, Player player) => true;
        public override bool CanUseItem(Item item, Player player)
        {
            return player.ownedProjectileCounts[ModContent.ProjectileType<RainbowGunBurst>()] < 1;
        }
        public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<RainbowGunBurst>(), damage, knockback, player.whoAmI);
                return false;
            }
            return base.Shoot(item, player, source, position, velocity, type, damage, knockback);
        }
    }

    public class RainbowGunRainbow : ModProjectile
    {
        public override string Texture => Core.AssetReferences.Items.Weapons.RainbowGunRainbow.KEY;
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.DamageType = DamageClass.Magic;
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 1600;
            Projectile.Opacity = 0;
        }
        bool init;
        public List<Vector2> ArcPositions { get; set; } = [];

        public float Completion = 0;
        public Player Owner => Main.player[Projectile.owner];

        float opacity2;

        int frameCounter2;
        int frame2;
        public override void AI()
        {
            if (!init)
            {
                init = true;

                foreach (var item in Main.projectile)
                {
                    if (item.active && item.whoAmI != Projectile.whoAmI && item.owner == Projectile.owner && item.type == Type)
                        item.Kill();
                }

                SoundEngine.PlaySound(SoundID.Item67, Projectile.Center);

                var startPos = Projectile.Center;
                var endPos = Owner.MouseWorld;
                if (startPos.Distance(endPos) < 80)
                    endPos = startPos + startPos.DirectionTo(endPos) * 80;
                float segments = 20;
                Projectile.velocity = Vector2.Zero;
                float maxHeight = Math.Abs(startPos.X - endPos.X) * 0.25f;
                for (int i = 0; i < segments; i++)
                {
                    float complete = i / (segments - 1f);
                    var pos = Vector2.Lerp(startPos, endPos, complete) - maxHeight * new Vector2(0, 1 - MathF.Pow(2 * (complete - 0.5f), 2));

                    //check i here to ensure even in solid tiles you get 2 segments
                    if (i > 1 && Collision.SolidCollision(pos - new Vector2(4, 4), 8, 8))
                        break;
                    ArcPositions.Add(pos);
                }
                Owner.itemRotation = (ArcPositions[0].DirectionTo(ArcPositions[1]) * Owner.direction).ToRotation();
            }
            if (Completion >= 1)
            {
                Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), ArcPositions.MultiLerp(Main.rand.NextFloat(0.05f, 0.95f)) + Main.rand.NextVector2Circular(16, 16), Vector2.UnitY * 0 /*+ new Vector2(Main.windSpeedCurrent*11,0)*/, ModContent.ProjectileType<RainbowGunRain>(), Projectile.damage, Projectile.knockBack, Projectile.owner);

                if (frameCounter2++ % 12 == 0)
                    frame2 = (frame2 + 1) % 3;
                if (opacity2 < 1)
                    opacity2 += 0.125f;
            }
            else
                Completion += 0.05f;

            if (Projectile.frameCounter++ % 12 == 0)
                Projectile.frame = (Projectile.frame + 1) % 3;
            if (Projectile.Opacity < 1)
                Projectile.Opacity += 0.125f;

        }

        public override bool PreDraw(ref Color lightColor)
        {
            var device = Main.instance.GraphicsDevice;

            using var lease = RenderTargetPool.Shared.Rent(
                device,
                Main.screenWidth / 2,
                Main.screenHeight / 2,
                RenderTargetDescriptor.Default
            );

            List<Vector3> path =
                [..
                    from item in ArcPositions.Select((x, indx) => new { x, indx }) where (item.indx <= Completion * ArcPositions.Count || item.indx <= 1)
                    select new Vector3(item.x.X - Main.screenPosition.X, item.x.Y - Main.screenPosition.Y, 1f)
                ];
            var shader = Shaders.RainbowShader!.Value;
            Matrix world = Matrix.Identity;
            Matrix view = Matrix.Identity;
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -200f, 200f);
            Main.spriteBatch.End(out var ss);
            using (lease.Scope(clearColor: Color.Transparent))
            {
                using var scope = PrimitiveRenderer.BeginShaderScope(shader, world, view, projection);
                using var mesh = TriangleStripBuilder.BuildStripPooled(
                            path,
                            10 + 30 * MathF.Pow(Completion, 4),
                            Color.White,
                            PrimitiveMeshCache.Shared,
                            upHint: ArcPositions[0].X - ArcPositions[^1].X > 0 ? -Vector3.UnitZ : Vector3.UnitZ);

                scope.Draw(mesh.View);
            }

            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp,
                DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.ZoomMatrix);
            Main.spriteBatch.Draw(lease.Target, Vector2.Zero, null, Color.White with { A = 127 }, 0, Vector2.Zero, 2, SpriteEffects.None, 0);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(ss);

            Texture2D texture = TextureAssets.Projectile[Type].Value;
            var frame = texture.Frame(1, 3, 0, Projectile.frame);
            Main.EntitySpriteDraw(texture, ArcPositions[0] - Main.screenPosition, frame,
          Color.White * Projectile.Opacity, 0, frame.Size() * 0.5f, Projectile.scale,
          SpriteEffects.None);

            frame = texture.Frame(1, 3, 0, frame2);

            Main.EntitySpriteDraw(texture, ArcPositions[^1] - Main.screenPosition, frame,
          Color.White * opacity2, 0, frame.Size() * 0.5f, Projectile.scale,
          SpriteEffects.FlipHorizontally);
            return false;
        }



    }

    public class RainbowGunRain : ModProjectile
    {
        public override string Texture => $"Terraria/Images/Projectile_{ProjectileID.RainbowRodBullet}";
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.penetrate = 3;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 60;
            Projectile.MaxUpdates = 2;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.timeLeft = 1200;
            Projectile.Opacity = 0;
            ProjectileID.Sets.DrawScreenCheckFluff[Type] = 16000;
        }
        public override void SetStaticDefaults()
        {
            DrawLayerSystem.DrawToLayer += DrawStarBatch;
            ProjectileID.Sets.TrailCacheLength[Type] = 10;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public static Color[] Colors =>
        [
            new Color(252, 125, 109),
            new Color(252, 147, 109),
            new Color(255, 184, 97),
            new Color(255, 226, 97),
            new Color(217, 255, 94),
            new Color(172, 255, 94),
            new Color(94, 255, 148),
            new Color(94, 255, 228),
            new Color(138, 179, 255),
            new Color(145, 130, 255),
            new Color(173, 126, 252),
            new Color(225, 115, 250),
            new Color(250, 115, 196),
            new Color(252, 109, 130),
        ];
        public Player Owner => Main.player[Projectile.owner];

        public Color? color
        {
            get => field ??= Colors[Main.rand.Next(Colors.Length)];
        }
        public bool isHoming;

        public ref float TwinkleSpeed => ref Projectile.localAI[0];
        public ref float TwinkleTimer => ref Projectile.localAI[1];
        public override void AI()
        {
            if (TwinkleSpeed == 0)
            {
                TwinkleSpeed = Main.rand.NextFloat(0.75f, 1.25f);
            }
            TwinkleTimer += TwinkleSpeed;
            Projectile.Opacity = isHoming ? 1 : 0.5f + 0.5f * RootsUtils.Sine0To1(TwinkleTimer * 0.1f);
            int fadeTime = 60;
            float fadeIntensity = MathHelper.Clamp(1 - (Projectile.timeLeft - (1200 - fadeTime)) / (float)fadeTime, 0, 1);
            Projectile.Opacity *= MathHelper.Clamp(fadeIntensity * 4, 0, 1);
            Projectile.velocity += VelocityChange() * fadeIntensity;
            Projectile.velocity *= attackTarget > -1 ? 0.95f : 0.99f;
            Projectile.tileCollide = attackTarget == -1;
        }
        int attackTarget = -1;
        Vector2 VelocityChange()
        {
            if (!isHoming)
                return Vector2.UnitY * 0.15f;

            attackTarget = -1;
            Projectile.Minion_FindTargetInRange(1200, ref attackTarget, true, (enty, _) => enty is NPC nPC && Projectile.localNPCImmunity[enty.whoAmI] == 0 && nPC.CanBeChasedBy(Projectile));
            if (attackTarget < 0)
                return Vector2.UnitY * 0.15f;
            var target = Main.npc[attackTarget];
            return Projectile.DirectionTo(target.Center) * 0.75f;
        }

        void SplashUpdate(Particle p)
        {
            p.Velocity.Y += 0.2f;
        }
        public override void OnKill(int timeLeft)
        {
            for (var i = 0; i < 3; i++)
            {
                ParticleSystem.SpawnParticle(new(TextureAssets.BlackTile, Projectile.Center + Projectile.velocity, Main.rand.Next(20, 40))
                {
                    Scale = new(0.125f),
                    Color = color!.Value,
                    UpdateLogic = SplashUpdate,
                    Velocity = Vector2.UnitY.RotatedByRandom(1) * Main.rand.NextFloat(-4, -1)
                });
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Projectile.numHits > 0 || Owner.statMana >= Owner.statManaMax2 || !Main.rand.NextBool(10))
                return;

            Owner.ManaEffect(10);
            Owner.statMana += 10;
            Owner.statMana = Math.Min(Owner.statMana, Owner.statManaMax2);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
        private static RenderTargetLease? target;

        public override void Load()
        {
            if (!Main.dedServ) Main.RunOnMainThread(() => target = ScreenspaceTargetPool.Shared.Rent(Main.graphics.GraphicsDevice, (w, h) => (w / 2, h / 2)));
        }

        public override void Unload()
        {
            Main.RunOnMainThread(() => target?.Dispose());
        }
        public static void DrawStarBatch(DrawLayerSystem.DrawLayer layer)
        {
            if (layer != DrawLayer.BeforeProjectiles)
                return;

            using (target!.Scope(clearColor: Color.Transparent))
            {
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, Matrix.CreateScale(0.5f, 0.5f, 1.0f));
                foreach (var item in Main.projectile)
                {
                    if (!item.active || item.ModProjectile is not RainbowGunRain rain)
                        continue;
                    DrawSingleStar(rain);
                }
                Main.spriteBatch.End();
            }
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, Main.Transform);
            Main.spriteBatch.Draw(target!.Target, Vector2.Zero, null, Color.White, 0, Vector2.Zero, 2, 0, 0);
            Main.spriteBatch.End();
        }
        public static void DrawSingleStar(RainbowGunRain mp)
        {
            var Projectile = mp.Projectile;
            Texture2D texture = ParticleTextures.Transparent.Circle[4].Value;

            int trailCount = Math.Min((int)(Projectile.velocity.Length() * 4), 30);
            if (trailCount < 2)
                trailCount = 2;
            for (int i = trailCount - 1; i >= 0; i--)
            {
                float completion = 1 - (i / (float)(trailCount - 1));
                Vector2 scaleMult = new(0.03f);
                float opacityMult = true ? 0.6f : 0.4f;
                if (i == 0)
                {
                    texture = TextureAssets.Projectile[mp.Type].Value;
                    scaleMult = new Vector2(0.4f, 0.4f);
                    opacityMult = 1;
                }
                Vector2 pos = Projectile.oldPos.MultiLerp(1 - completion);

                Main.EntitySpriteDraw(texture, pos + Projectile.Size * 0.5f - Main.screenPosition, texture.Frame(),
              ((mp.color!.Value with { A = 200 }) * Projectile.Opacity * opacityMult * completion), Projectile.velocity.ToRotation() + MathHelper.PiOver2, texture.Size() * new Vector2(0.5f, 0.5f), Projectile.scale * mp.TwinkleSpeed * completion * scaleMult,
              Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally);

            }
        }
        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            width = height = 2;
            return true;
        }
    }

    public class RainbowGunBurst : ModProjectile
    {
        public override string Texture => RootsCoreUtils.InvisiblePixelPath;
        public Player Owner => Main.player[Projectile.owner];
        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.tileCollide = false;
            Projectile.DamageType = DamageClass.Magic;
        }
        bool release;
        bool init;
        float maxCharge;
        float charge;
        float attemptToCharge;
        bool extraLongDection;
        public static Color[] Colors =>
        [
            Color.Red,
            Color.Orange,
            Color.Yellow,
            Color.Lime,
            Color.SkyBlue,
            Color.Violet,
        ];
        public override void AI()
        {
            if (!init)
            {
                init = true;
                maxCharge = Owner.statManaMax2;
                charge += (int)(Owner.HeldItem.mana * Owner.manaCost);
            }
            float chargeCompletion = charge / maxCharge;
            if (!Owner.controlUseTile || release)
            {
                Lighting.AddLight(Projectile.Center, new Vector3(2f, 2f, 2) * chargeCompletion);
                if (release) return;
                release = true;
                var dir = Owner.DirectionTo(Owner.MouseWorld).ToRotation();
                SoundEngine.PlaySound(chargeCompletion >= 1 ? SoundID.DD2_ExplosiveTrapExplode : SoundID.DD2_PhantomPhoenixShot);
                Owner.velocity -= Owner.DirectionTo(Owner.MouseWorld) * 10 * chargeCompletion;
                var tex = ParticleTextures.Transparent.Muzzle[chargeCompletion >= 1 ? 0 : 1];

                extraLongDection = true;
                foreach (var item in Main.projectile)
                {
                    var hitbox = item.Hitbox;
                    if (item.active && item.type == ModContent.ProjectileType<RainbowGunRain>() && (Colliding(new(), hitbox) ?? false) && item.ModProjectile is RainbowGunRain rain)
                    {
                        item.velocity = item.DirectionFrom(Projectile.Center) * (6 + 8 * chargeCompletion);
                        rain.isHoming = true;
                        for (int i = 0; i < item.localNPCImmunity.Length; i++)
                            item.localNPCImmunity[i] = 30;
                    }
                }
                extraLongDection = false;

                for (int i = 0; i < Colors.Length; i++)
                {
                    ParticleSystem.SpawnParticle(new(tex,
                    Projectile.Center + chargeCompletion * 16 * Vector2.UnitX.RotatedBy(dir + MathHelper.Pi + MathHelper.TwoPi * (i / (float)Colors.Length)), 15, ParticlePreset.ExplodeAndFade)
                    {
                        Scale = new(0.5f * chargeCompletion),
                        Color = Colors[i],
                        Rotation = dir + MathHelper.PiOver2,
                        Origin = tex.Size() * new Vector2(0.5f, chargeCompletion >= 1 ? 0.9f : 0.8f)
                    });
                }
                ParticleSystem.SpawnParticle(new(tex, Projectile.Center, 15, ParticlePreset.ExplodeAndFade)
                {
                    Scale = new(0.5f * chargeCompletion),
                    Color = Color.White,
                    Rotation = dir + MathHelper.PiOver2,
                    Origin = tex.Size() * new Vector2(0.5f, chargeCompletion >= 1 ? 0.9f : 0.8f)
                });
                return;
            }
            Owner.SetDummyItemTime(16);
            Projectile.velocity = Vector2.Zero;
            Projectile.timeLeft = 16;
            Projectile.Center = Owner.MountedCenter + Owner.DirectionTo(Owner.MouseWorld) * 48 + new Vector2(0, Owner.gfxOffY);
            Owner.direction = Owner.Center.X - Owner.MouseWorld.X > 0 ? -1 : 1;
            Owner.itemRotation = (Owner.DirectionTo(Owner.MouseWorld) * Owner.direction).ToRotation() + Main.rand.NextFloat(-0.03f, 0.03f) * MathF.Pow(chargeCompletion, 3);

            if (charge < maxCharge)
            {
                attemptToCharge += (Owner.statManaMax2 * 0.005f) / Owner.manaCost;
                while (attemptToCharge >= 1)
                {
                    attemptToCharge--;
                    var a = Owner.statMana;
                    if (Owner.CheckMana((int)Math.Ceiling(1 / Owner.manaCost), true))
                    {
                        charge += 1;
                        if (charge >= maxCharge)
                            SoundEngine.PlaySound(SoundID.DD2_DarkMageCastHeal);
                    }
                }
            }
            chargeCompletion = charge / maxCharge;
            float rotOff = -Owner.miscCounter / 150f * MathHelper.TwoPi;
            for (int i = 0; i < Colors.Length; i++)
            {
                ParticleSystem.SpawnParticle(new(ParticleTextures.Transparent.Circle[4],
                    Projectile.Center + chargeCompletion * 10 * Vector2.UnitX.RotatedBy(rotOff + MathHelper.TwoPi * (i / (float)Colors.Length)),
                    1)
                {
                    Scale = new(0.05f + 0.1f * (chargeCompletion)),
                    Color = Colors[i],
                });

                if (chargeCompletion >= 1)
                    ParticleSystem.SpawnParticle(new(ParticleTextures.Transparent.Star[7],
                    Projectile.Center + 1 * Vector2.UnitX.RotatedBy(rotOff + MathHelper.TwoPi * (i / (float)Colors.Length)),
                    1)
                    {
                        Scale = new(0.2f),
                        Color = Colors[i] with { A = 0 },
                    });
            }
            ParticleSystem.SpawnParticle(new(ParticleTextures.Transparent.Circle[4], Projectile.Center, 1)
            {
                Scale = new(0.05f + 0.1f * (chargeCompletion)),
                Color = Color.White,
            });
            Lighting.AddLight(Projectile.Center, new Vector3(1.5f, 1.5f, 1.5f) * chargeCompletion);
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.SourceDamage *= charge * 0.2f;
            if (charge >= maxCharge)
                modifiers.SourceDamage *= 1.1f;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (!release)
                return false;
            float maxRange = extraLongDection ? 300 : 200;
            float maxAngle = extraLongDection ? 1.25f : 1f;
            return targetHitbox.IntersectsConeSlowMoreAccurate(Projectile.Center, maxRange * charge / maxCharge, Owner.DirectionTo(Owner.MouseWorld).ToRotation(), 1);
        }
    }
}
