using Daybreak.Common.Rendering;
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

namespace Roots.Items.Weapons;

public class RainbowGun : ConfigurableItemRework<RainbowGun>
{
    #region Parameters
    #region Balancing
    public static int RainHomingRange => 1600;
    public static int HitsToRechargeMana => 10;
    public static int ManaRechargeSize => 10;
    public static float ChargeFrames => 200;
    public static float DmgMultPerCharge => 0.2f;
    public static float MaxChargeBonusDmgMult => 1.1f;
    public static float MaxBurstRange => 200;
    public static float MaxBurstRangeRaindrops => 300;
    public static float MaxBurstAngle => 1f;
    public static float MaxBurstAngleRaindrops => 1.25f;
    #endregion

    #region Visuals
    public static int MinimumRainbowLength => 80;
    public static float SparkleSpawnVariancePixels => 16;
    public static int RainbowSegments => 20;
    public static float RainbowHeightMult => 0.25f;
    #endregion
    #endregion
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
        if (player.altFunctionUse != 2)
            return base.Shoot(item, player, source, position, velocity, type, damage, knockback);
        Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<RainbowGunBurst>(), damage, knockback, player.whoAmI);
        return false;
    }
}

public class RainbowGunRainbow : ModProjectile
{
    #region Private Fields/Properties
    private bool _init;
    private List<Vector2> ArcPositions { get; set; } = [];
    private float Completion;
    private Player Owner => Main.player[Projectile.owner];
    private float _opacity2;
    private int _frameCounter2;
    private int _frame2;
    #endregion
    public override string Texture => Core.AssetReferences.Items.Weapons.RainbowGunRainbow.KEY;
    public override void SetStaticDefaults()
    {
        ProjectileID.Sets.DrawScreenCheckFluff[Type] = 1600;
        ProjSets.ManaSpawnedProjectile[Type] = true; //Technically automatically set by being a Magic damage projectile
    }
    public override void SetDefaults()
    {
        Projectile.width = Projectile.height = 16;
        Projectile.tileCollide = false;
        Projectile.penetrate = -1;
        Projectile.DamageType = DamageClass.Magic;
        Projectile.Opacity = 0;
    }
    public override void AI()
    {
        if (!_init)
        {
            _init = true;

            foreach (var item in Main.projectile)
            {
                if (item.active && item.whoAmI != Projectile.whoAmI && item.owner == Projectile.owner && item.type == Type)
                    item.Kill();
            }

            SoundEngine.PlaySound(SoundID.Item67, Projectile.Center);

            var startPos = Projectile.Center;
            var endPos = Owner.MouseWorld;
            if (startPos.Distance(endPos) < RainbowGun.MinimumRainbowLength)
                endPos = startPos + startPos.DirectionTo(endPos) * RainbowGun.MinimumRainbowLength;
            Projectile.velocity = Vector2.Zero;
            float maxHeight = Math.Abs(startPos.X - endPos.X) * RainbowGun.RainbowHeightMult;
            for (int i = 0; i < RainbowGun.RainbowSegments; i++)
            {
                float complete = i / (RainbowGun.RainbowSegments - 1f);
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
            Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), ArcPositions.MultiLerp(Main.rand.NextFloat(0.05f, 0.95f)) + Main.rand.NextVector2Circular(RainbowGun.SparkleSpawnVariancePixels, RainbowGun.SparkleSpawnVariancePixels), Vector2.Zero, ModContent.ProjectileType<RainbowGunRain>(), Projectile.damage, Projectile.knockBack, Projectile.owner);

            if (_frameCounter2++ % 12 == 0)
                _frame2 = (_frame2 + 1) % 3;
            if (_opacity2 < 1)
                _opacity2 += 0.125f;
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

        //Define the actual path that's going to be drawn
        //This shortens the total vector path based on the rainbow's completion
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

        //Create and draw the rainbow primitive to the half-size screen target
        using (lease.Scope(clearColor: Color.Transparent))
        {
            using var scope = PrimitiveRenderer.BeginShaderScope(shader, world, view, projection, samplerState: SamplerState.LinearWrap);
            using var mesh = TriangleStripBuilder.BuildStripPooled(
                        path,
                        10 + 30 * MathF.Pow(Completion, 4),
                        Color.White,
                        PrimitiveMeshCache.Shared,
                        upHint: ArcPositions[0].X - ArcPositions[^1].X > 0 ? -Vector3.UnitZ : Vector3.UnitZ);

            scope.Draw(mesh.View);
        }

        //Draw the target at 2x scale; combined with the half-size screen target this gives pixelation
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

        frame = texture.Frame(1, 3, 0, _frame2);

        Main.EntitySpriteDraw(texture, ArcPositions[^1] - Main.screenPosition, frame,
      Color.White * _opacity2, 0, frame.Size() * 0.5f, Projectile.scale,
      SpriteEffects.FlipHorizontally);
        return false;
    }
}

public class RainbowGunRain : ModProjectile
{
    #region Private Fields/Properties
    private static int MaxLifetime => 1200;
    private static int FadeTime => 60;
    private Player Owner => Main.player[Projectile.owner];
    private Color? Color => field ??= Colors[Main.rand.Next(Colors.Length)];
    private ref float TwinkleSpeed => ref Projectile.localAI[0];
    private ref float TwinkleTimer => ref Projectile.localAI[1];
    private int _attackTarget = -1;
    private static RenderTargetLease? _target;
    private static int _manaHitCounter = 0; //This is only touched on the client side, so it's safe being a static var instead of using a ModPlayer
    #endregion
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
    public bool IsHoming;
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
        Projectile.timeLeft = MaxLifetime;
        Projectile.Opacity = 0;
    }
    public override void SetStaticDefaults()
    {
        DrawToLayer += DrawStarBatch;
        ProjectileID.Sets.TrailCacheLength[Type] = 10;
        ProjectileID.Sets.TrailingMode[Type] = 2;
        ProjSets.ManaSpawnedProjectile[Type] = true;
    }
    public override void AI()
    {
        if (TwinkleSpeed == 0)
        {
            TwinkleSpeed = Main.rand.NextFloat(0.75f, 1.25f);
        }
        TwinkleTimer += TwinkleSpeed;
        Projectile.Opacity = IsHoming ? 1 : 0.5f + 0.5f * RootsUtils.Sine0To1(TwinkleTimer * 0.1f);

        float fadeIntensity = MathHelper.Clamp(1 - (Projectile.timeLeft - (MaxLifetime - FadeTime)) / (float)FadeTime, 0, 1);
        Projectile.Opacity *= MathHelper.Clamp(fadeIntensity * 4, 0, 1);
        Projectile.velocity += VelocityChange() * fadeIntensity;
        Projectile.velocity *= _attackTarget > -1 ? 0.95f : 0.99f;
        Projectile.tileCollide = _attackTarget == -1;
    }
    private Vector2 VelocityChange()
    {
        if (!IsHoming)
            return Vector2.UnitY * 0.15f;

        _attackTarget = -1;
        Projectile.Minion_FindTargetInRange(RainbowGun.RainHomingRange, ref _attackTarget, true, (enty, _) => enty is NPC nPC && Projectile.localNPCImmunity[enty.whoAmI] == 0 && nPC.CanBeChasedBy(Projectile));
        if (_attackTarget < 0)
            return Vector2.UnitY * 0.15f;
        var targetNPC = Main.npc[_attackTarget];
        return Projectile.DirectionTo(targetNPC.Center) * 0.75f;
    }
    public override void OnHitNPC(NPC targetNPC, NPC.HitInfo hit, int damageDone)
    {
        if (Projectile.numHits > 0 || Owner.statMana >= Owner.statManaMax2)
            return;
        _manaHitCounter++;
        if (_manaHitCounter > RainbowGun.HitsToRechargeMana)
        {
            _manaHitCounter = 0;
            Owner.ManaEffect(RainbowGun.ManaRechargeSize);
            Owner.statMana = Math.Min(Owner.statMana + RainbowGun.ManaRechargeSize, Owner.statManaMax2);
        }
    }
    public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
    {
        width = height = 2;
        return true;
    }
    public override void OnKill(int timeLeft)
    {
        for (var i = 0; i < 3; i++)
        {
            ParticleSystem.SpawnParticle(new Particle(TextureAssets.BlackTile, Projectile.Center + Projectile.velocity, Main.rand.Next(20, 40))
            {
                Scale = new Vector2(0.125f), //one 2x2 pixel in size
                Color = Color!.Value,
                UpdateLogic = SplashUpdate,
                Velocity = Vector2.UnitY.RotatedByRandom(1) * Main.rand.NextFloat(-4, -1)
            });
        }
    }
    private void SplashUpdate(Particle p)
    {
        p.Velocity.Y += 0.2f;
    }
    public override bool PreDraw(ref Color lightColor) => false;

    #region Batched Drawing
    public override void Load()
    {
        if (!Main.dedServ) Main.RunOnMainThread(() => _target = ScreenspaceTargetPool.Shared.Rent(Main.graphics.GraphicsDevice, (w, h) => (w / 2, h / 2)));
    }
    public override void Unload()
    {
        Main.RunOnMainThread(() => _target?.Dispose());
    }
    public static void DrawStarBatch(DrawLayer layer)
    {
        if (layer != DrawLayer.BeforeProjectiles)
            return;

        using (_target!.Scope(clearColor: Microsoft.Xna.Framework.Color.Transparent))
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
        Main.spriteBatch.Draw(_target!.Target, Vector2.Zero, null, Microsoft.Xna.Framework.Color.White, 0, Vector2.Zero, 2, 0, 0);
        Main.spriteBatch.End();
    }
    public static void DrawSingleStar(RainbowGunRain mp)
    {
        var projectile = mp.Projectile;
        Texture2D texture = ParticleTextures.Transparent.Circle[4].Value;

        int trailCount = Math.Min((int)(projectile.velocity.Length() * 4), 30);
        if (trailCount < 2)
            trailCount = 2;
        for (int i = trailCount - 1; i >= 0; i--)
        {
            float completion = 1 - (i / (float)(trailCount - 1));
            Vector2 scaleMult = new(0.03f);
            float opacityMult = 0.6f;
            if (i == 0)
            {
                texture = TextureAssets.Projectile[mp.Type].Value;
                scaleMult = new Vector2(0.4f, 0.4f);
                opacityMult = 1;
            }
            Vector2 pos = projectile.oldPos.MultiLerp(1 - completion);

            Main.EntitySpriteDraw(texture, pos + projectile.Size * 0.5f - Main.screenPosition, texture.Frame(),
          ((mp.Color!.Value with { A = 200 }) * projectile.Opacity * opacityMult * completion), projectile.velocity.ToRotation() + MathHelper.PiOver2, texture.Size() * new Vector2(0.5f, 0.5f), projectile.scale * mp.TwinkleSpeed * completion * scaleMult,
          projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally);
        }
    }
    #endregion
}

public class RainbowGunBurst : ModProjectile
{
    #region Private Fields/Properties
    private bool _release;
    private bool _init;
    private float _maxCharge;
    private float _charge;
    private float _attemptToCharge;
    private bool _extraLongDetection;
    private Player Owner => Main.player[Projectile.owner];
    private const float MaxLightCharge = 1.5f;
    private const float MaxLightBurst = 2;
    #endregion
    public static Color[] Colors =>
    [
        Color.Red,
        Color.Orange,
        Color.Yellow,
        Color.Lime,
        Color.SkyBlue,
        Color.Violet,
    ];
    public override string Texture => RootsCoreUtils.InvisiblePixelPath;
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
    public override void AI()
    {
        if (!_init)
        {
            _init = true;
            _maxCharge = Owner.statManaMax2;
            _charge += (int)(Owner.HeldItem.mana * Owner.manaCost);
        }
        float chargeCompletion = _charge / _maxCharge;
        if (!Owner.controlUseTile || _release)
        {
            Lighting.AddLight(Projectile.Center, new Vector3(MaxLightBurst) * chargeCompletion);
            if (_release) return;
            _release = true;
            BurstLogic(chargeCompletion);
            return;
        }

        if (_charge < _maxCharge)
        {
            _attemptToCharge += (Owner.statManaMax2 * (1 / RainbowGun.ChargeFrames)) / Owner.manaCost;
            while (_attemptToCharge >= 1)
            {
                _attemptToCharge--;
                var a = Owner.statMana;
                if (!Owner.CheckMana((int)Math.Ceiling(1 / Owner.manaCost), true)) continue;
                _charge += 1;
                if (_charge >= _maxCharge)
                    SoundEngine.PlaySound(SoundID.DD2_DarkMageCastHeal);
            }
        }
        chargeCompletion = _charge / _maxCharge;
        ChargeStateLogic(chargeCompletion);
    }
    private void ChargeStateLogic(float chargeCompletion)
    {
        Owner.SetDummyItemTime(16);
        Projectile.velocity = Vector2.Zero;
        Projectile.timeLeft = 16;
        Projectile.Center = Owner.MountedCenter + Owner.DirectionTo(Owner.MouseWorld) * 48 + new Vector2(0, Owner.gfxOffY);
        Owner.direction = Owner.Center.X - Owner.MouseWorld.X > 0 ? -1 : 1;
        Owner.itemRotation = (Owner.DirectionTo(Owner.MouseWorld) * Owner.direction).ToRotation() + Main.rand.NextFloat(-0.03f, 0.03f) * MathF.Pow(chargeCompletion, 3);
        Lighting.AddLight(Projectile.Center, new Vector3(MaxLightCharge) * chargeCompletion);

        float rotOff = -Owner.miscCounter / 150f * MathHelper.TwoPi;
        for (int i = 0; i < Colors.Length; i++)
        {
            ParticleSystem.SpawnParticle(new Particle(ParticleTextures.Transparent.Circle[4],
                Projectile.Center + chargeCompletion * 10 * Vector2.UnitX.RotatedBy(rotOff + MathHelper.TwoPi * (i / (float)Colors.Length)),
                1)
            {
                Scale = new Vector2(0.05f + 0.1f * (chargeCompletion)),
                Color = Colors[i],
            });

            if (chargeCompletion >= 1)
                ParticleSystem.SpawnParticle(new Particle(ParticleTextures.Transparent.Star[7],
                Projectile.Center + 1 * Vector2.UnitX.RotatedBy(rotOff + MathHelper.TwoPi * (i / (float)Colors.Length)),
                1)
                {
                    Scale = new Vector2(0.2f),
                    Color = Colors[i] with { A = 0 },
                });
        }
        ParticleSystem.SpawnParticle(new Particle(ParticleTextures.Transparent.Circle[4], Projectile.Center, 1)
        {
            Scale = new Vector2(0.05f + 0.1f * (chargeCompletion)),
            Color = Color.White,
        });
    }
    private void BurstLogic(float chargeCompletion)
    {
        var dir = Owner.DirectionTo(Owner.MouseWorld).ToRotation();
        SoundEngine.PlaySound(chargeCompletion >= 1 ? SoundID.DD2_ExplosiveTrapExplode : SoundID.DD2_PhantomPhoenixShot);
        Owner.velocity -= Owner.DirectionTo(Owner.MouseWorld) * 10 * chargeCompletion;
        var tex = ParticleTextures.Transparent.Muzzle[chargeCompletion >= 1 ? 0 : 1];

        //Detect Raindrops
        _extraLongDetection = true;
        foreach (var item in Main.projectile)
        {
            var hitbox = item.Hitbox;
            if (!item.active || item.type != ModContent.ProjectileType<RainbowGunRain>() ||
                (!(Colliding(new Rectangle(), hitbox) ?? false)) ||
                item.ModProjectile is not RainbowGunRain rain) continue;
            item.velocity = item.DirectionFrom(Projectile.Center) * (6 + 8 * chargeCompletion);
            rain.IsHoming = true;
            for (int i = 0; i < item.localNPCImmunity.Length; i++)
                item.localNPCImmunity[i] = 30;
        }
        _extraLongDetection = false;

        //Draw Burst particles
        for (int i = 0; i < Colors.Length; i++)
        {
            ParticleSystem.SpawnParticle(new Particle(tex,
            Projectile.Center + chargeCompletion * 16 * Vector2.UnitX.RotatedBy(dir + MathHelper.Pi + MathHelper.TwoPi * (i / (float)Colors.Length)), 15, ParticlePreset.ExplodeAndFade)
            {
                Scale = new Vector2(0.5f * chargeCompletion),
                Color = Colors[i],
                Rotation = dir + MathHelper.PiOver2,
                Origin = tex.Size() * new Vector2(0.5f, chargeCompletion >= 1 ? 0.9f : 0.8f)
            });
        }
        ParticleSystem.SpawnParticle(new Particle(tex, Projectile.Center, 15, ParticlePreset.ExplodeAndFade)
        {
            Scale = new Vector2(0.5f * chargeCompletion),
            Color = Color.White,
            Rotation = dir + MathHelper.PiOver2,
            Origin = tex.Size() * new Vector2(0.5f, chargeCompletion >= 1 ? 0.9f : 0.8f)
        });
    }
    public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
    {
        modifiers.SourceDamage *= _charge * RainbowGun.DmgMultPerCharge;
        if (_charge >= _maxCharge)
            modifiers.SourceDamage *= RainbowGun.MaxChargeBonusDmgMult;
    }
    public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
    {
        if (!_release)
            return false;
        float maxRange = _extraLongDetection ? RainbowGun.MaxBurstRangeRaindrops : RainbowGun.MaxBurstRange;
        float maxAngle = _extraLongDetection ? RainbowGun.MaxBurstAngleRaindrops : RainbowGun.MaxBurstAngle;
        return targetHitbox.IntersectsConeSlowMoreAccurate(Projectile.Center, maxRange * _charge / _maxCharge, Owner.DirectionTo(Owner.MouseWorld).ToRotation(), maxAngle);
    }
}
