using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Roots.Config;
using RootsBeta.Utilities;
using RootsCore;
using RootsCore.ContentBaseClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using Daybreak.Common.Rendering;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace RootsBeta.Items.Weapons
{
    public class VampireKnives : ConfigurableCustomSwing<VampireKnives>, IConfigurableContent<VampireKnives>
    {
        public static ConfigGroup ConfigGroups => ConfigGroup.WeaponReworks;

        #region Parameters
        #region Balancing
        public static int KnivesShot => 5;
        public static int ManaCost => 40;
        public static int Damage => 44;
        public static float TrueMeleeDamageModifier => 4f;
        public static float TrueMeleeRange => 60f;
        public static float KnivesSpeed => 23f;
        public static int KnivesLifetime => 36;
        public static float KnivesFadeoutPercent => 0.5f;
        #endregion
        #region Visuals
        public static float KnivesDistance => 3f;
        public static float MaxRotation => 0.5f;
        public static float RandomRotation => 0.05f;
        public static float RotationOffset => 0.45f;
        public static float StartupWristSnapback => 0.5f;
        public static float AttackWristSnap => 0.5f;
        public static int TrailFrames => 15;
        public static int TrailWidth => 16;
        public static int TrailOffsetSize => 12;
        public static Color TrailColorStart => Color.Red;
        public static Color TrailColorEnd => Color.DarkRed;
        #endregion
        #endregion

        public override int[] ItemIds => [ItemID.VampireKnives];
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips) =>
            tooltips.AppendTooltipWith("Weapons.VampireKnives.Tooltip");

        public override bool SizeModifiers => false;

        public override void SetDefaults(Item item)
        {
            base.SetDefaults(item);
            item.shoot = ModContent.ProjectileType<VampireKnivesHoldout>();
            item.useTime = item.useAnimation = 30;
            item.damage = Damage;
        }
    }

    public class VampireKnivesHoldout : BaseCustomSwingProjectile<VampireKnivesHoldout>
    {
        public override bool UseMeleeSize => false;

        private Vector2 KnivesOrigin => new(Projectile.spriteDirection == -1 ? 28 : 4, 28);
        private static int Offset => 12;

        public AttackState PreStartup = new()
        {
            Time = 6,
            SwingWidth = 2f,
            CanDamage = false,
            SwingOffsetAngle = proj =>
                proj.State.SwingWidth - 0.3f * RootsCoreUtils.Ease.InSine(1 - proj.StateCompletion) + VampireKnives.RotationOffset,
            OffsetDistance = Offset,
            AlternateSwings = false
        };
        public AttackState Startup = new()
        {
            Time = 6,
            SwingWidth = 2.0f,
            RotationSpeed = 0.25f,
            CanDamage = true,
            SwingOffsetAngle = proj =>
                proj.State.SwingWidth * RootsCoreUtils.Ease.InOutCirc(1 - proj.StateCompletion) + VampireKnives.RotationOffset,
            OffsetDistance = Offset,
            AlternateSwings = false
        };
        public AttackState Attack = new()
        {
            Time = 8,
            SwingWidth = 0f,
            CanDamage = false,
            SwingOffsetAngle = _ =>
                VampireKnives.RotationOffset,
            OffsetDistance = Offset,
            AlternateSwings = false,
            Sound = SoundID.DD2_MonkStaffSwing with { Volume = 0.5f, Pitch = 0.2f },
            AdditionalAI = proj =>
            {
                if (proj.Projectile.numUpdates != -1) return;
                for (int i = 0; i < VampireKnives.KnivesShot; i++)
                {
                    if (!(proj.StateCompletion >= (i + 1) / (float)(VampireKnives.KnivesShot + 1))) continue;
                    if (proj._knivesShot[i]) continue;
                    int index = proj.Projectile.spriteDirection != 1 ? i : VampireKnives.KnivesShot - i;

                    Vector2 direction = proj.Projectile.DirectionTo(proj.Player.Center).RotatedBy(MathHelper.PiOver2);
                    Vector2 offset = direction * (VampireKnives.KnivesShot * 0.5f - index) * VampireKnives.KnivesDistance;

                    Vector2 relativePlayerSpeed = proj.Player.velocity;
                    relativePlayerSpeed.X = proj.Angle.X < 0 ? Math.Max(relativePlayerSpeed.X, 0) : Math.Min(relativePlayerSpeed.X, 0);
                    relativePlayerSpeed.Y = proj.Angle.Y < 0 ? Math.Max(relativePlayerSpeed.Y, 0) : Math.Min(relativePlayerSpeed.Y, 0);

                    float rotation = (index - VampireKnives.KnivesShot * 0.5f) * (VampireKnives.MaxRotation / VampireKnives.KnivesShot);

                    var knife = Projectile.NewProjectileDirect(proj.Projectile.GetSource_FromThis(), proj.Projectile.Center + offset,
                        proj.Angle.RotatedBy(rotation).RotatedByRandom(VampireKnives.RandomRotation) *
                        (-VampireKnives.KnivesSpeed * Main.rand.NextFloat(0.98f, 1.02f)) + relativePlayerSpeed, ModContent.ProjectileType<VampireKnife>(),
                        proj.Projectile.damage, proj.Projectile.knockBack,
                        proj.Projectile.owner);
                    SoundEngine.PlaySound(SoundID.Item39 with { MaxInstances = 10, Pitch = 0.2f + -0.3f * ((float)i / (VampireKnives.KnivesShot - 1)), PitchVariance = 0.1f }, proj.Projectile.Center + offset);

                    knife.ai[0] = 30 - VampireKnives.KnivesLifetime;

                    proj._knivesShot[i] = true;
                }
            }
        };
        public AttackState Endlag = new()
        {
            Time = 15,
            SwingWidth = -0.1f,
            CanDamage = false,
            RotationSpeed = 0.25f,
            SwingOffsetAngle = proj =>
                proj.State.SwingWidth * RootsCoreUtils.Ease.Linear(proj.StateCompletion) + VampireKnives.RotationOffset,
            OffsetDistance = Offset,
            AlternateSwings = false
        };

        public AttackState PostEndlag = new()
        {
            Time = 6,
            SwingWidth = 1.8f,
            CanDamage = false,
            SwingOffsetAngle = proj =>
                -0.1f + proj.State.SwingWidth * RootsCoreUtils.Ease.InOutCirc(proj.StateCompletion) + VampireKnives.RotationOffset,
            OffsetDistance = Offset,
            AlternateSwings = false
        };
        public override List<AttackState> StateList => [PreStartup, Startup, Attack, Endlag, PostEndlag];
        public override Item BaseItem => ContentSamples.ItemsByType[ItemID.VampireKnives];
        public override string Texture => "RootsBeta/Items/Weapons/VampireKnivesBald";
        public override float LineCollisionLength => VampireKnives.TrueMeleeRange;
        public override int AfterImageCount => 0;
        private bool[] _knivesShot = new bool[VampireKnives.KnivesShot];
        public static Asset<Texture2D> VampireKnives1 => field ??= ModContent.Request<Texture2D>($"RootsBeta/Items/Weapons/VampireKnives1");
        public static Asset<Texture2D> VampireKnives2 => field ??= ModContent.Request<Texture2D>($"RootsBeta/Items/Weapons/VampireKnives2");
        public static Asset<Texture2D> VampireKnives3 => field ??= ModContent.Request<Texture2D>($"RootsBeta/Items/Weapons/VampireKnives3");
        public static Asset<Texture2D> VampireKnives4 => field ??= ModContent.Request<Texture2D>($"RootsBeta/Items/Weapons/VampireKnives4");
        /// <summary>
        /// Used instead of Projectile.oldPos so we can remove the player position when storing it and re-add it when drawing<br/>
        /// This means that it'll stay tied to the player position in motion instead of the sword world position
        /// </summary>
        public List<Vector2> OldPositionPlayerOffset { get; set; } = new();
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;

            if (AfterImageCount > 0)
            {
                for (int i = 0; i < OldProjectileRot.Count; i++)
                {
                    var col = Projectile.Opacity * (i / (float)AfterImageCount) * 0.1f;
                    Main.EntitySpriteDraw(texture, OldProjectilePos[i] - Main.screenPosition, null,
                        AfterImageColor * col, OldProjectileRot[i], KnivesOrigin, OldScale[i],
                        Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally);
                }
            }
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, texture.Frame(),
                lightColor, Projectile.rotation, KnivesOrigin, Projectile.scale,
                Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally);
            if (State == Startup || State == Attack)
                VampireKnivesSwordTrailDrawer.Draw(Projectile);
            return false;
        }

        public override void AdditionalAI()
        {
            if (State == PreStartup || State == Startup)
            {
                Projectile.rotation += VampireKnives.StartupWristSnapback * Projectile.spriteDirection;
            }
            else if (State == Attack)
            {
                Projectile.rotation += MathHelper.Lerp(VampireKnives.StartupWristSnapback, -VampireKnives.AttackWristSnap, RootsCoreUtils.Ease.OutCirc(StateCompletion)) * Projectile.spriteDirection;
            }
            else if (State == Endlag)
            {
                Projectile.rotation -= VampireKnives.AttackWristSnap * Projectile.spriteDirection;
            }
            else if (State == PostEndlag)
            {
                Projectile.rotation +=
                    MathHelper.Lerp(-VampireKnives.AttackWristSnap, VampireKnives.StartupWristSnapback, RootsCoreUtils.Ease.InOutCirc(StateCompletion)) * Projectile.spriteDirection;
            }
        }

        public override void PostAI()
        {
            OldPositionPlayerOffset.Insert(0, Projectile.Center - Player.Center);
            if (OldPositionPlayerOffset.Count > ProjectileID.Sets.TrailCacheLength[Type])
                OldPositionPlayerOffset.RemoveAt(OldPositionPlayerOffset.Count - 1);
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.SourceDamage *= VampireKnives.TrueMeleeDamageModifier;
        }

        public override void PostDraw(Color lightColor)
        {
            if (State == Endlag) return;
            float progress = (float)Timer / ((PreStartup.Time + Startup.Time) * Projectile.MaxUpdates);
            Color color = lightColor;
            if (State != PostEndlag)
                color = Color.Lerp(lightColor, Color.White with { A = (byte)((255 - Projectile.alpha) / 3f) }, RootsCoreUtils.Ease.InQuad(progress));
            bool[] shown = Enumerable.Repeat(State != PostEndlag, 4).ToArray();
            if (State == Attack || State == PostEndlag)
            {
                for (int i = 0; i < shown.Length; i++)
                {
                    if (StateCompletion >= (i + 1) / (float)(shown.Length + 1))
                    {
                        shown[^(i + 1)] = State != Attack;
                    }
                }
            }
            if (shown[0])
            {
                Main.EntitySpriteDraw(VampireKnives1.Value, Projectile.Center - Main.screenPosition, VampireKnives1.Frame(),
                    color, Projectile.rotation, KnivesOrigin, Projectile.scale,
                    Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally);
            }
            if (shown[1])
            {
                Main.EntitySpriteDraw(VampireKnives2.Value, Projectile.Center - Main.screenPosition, VampireKnives2.Frame(),
                    color, Projectile.rotation, KnivesOrigin, Projectile.scale,
                    Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally);
            }
            if (shown[2])
            {
                Main.EntitySpriteDraw(VampireKnives3.Value, Projectile.Center - Main.screenPosition, VampireKnives3.Frame(),
                    color, Projectile.rotation, KnivesOrigin, Projectile.scale,
                    Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally);
            }
            if (shown[3])
            {
                Main.EntitySpriteDraw(VampireKnives4.Value, Projectile.Center - Main.screenPosition, VampireKnives4.Frame(),
                    color, Projectile.rotation, KnivesOrigin, Projectile.scale,
                    Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally);
            }
        }
    }

    public class VampireKnife : ModProjectile
    {
        public override string Texture => $"Terraria/Images/Projectile_{ProjectileID.VampireKnife}";

        public int Timer;
        private Vector2 _initVelocity;
        private int _dieDirection = Main.rand.NextBool() ? -1 : 1;
        private Player Player => Main.player[Projectile.owner];

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.width = Projectile.height = 30;
            Projectile.light = 0.2f;
            Projectile.ignoreWater = true;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, texture.Frame(),
                (Color.White * Projectile.Opacity) with { A = (byte)((255 - Projectile.alpha) / 3f) }, Projectile.rotation, texture.Size() * 0.5f, Projectile.scale,
                Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally);
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            for (int num493 = 0; num493 < 3; num493++)
            {
                Dust dust = Dust.NewDustDirect(new Vector2(Projectile.position.X, Projectile.position.Y), Projectile.width, Projectile.height, DustID.TheDestroyer, 0f, 0f, 100, default, 0.8f);
                dust.noGravity = true;
                dust.velocity *= 1.2f;
                dust.velocity -= Projectile.oldVelocity * 0.3f;
            }
        }

        public override void AI()
        {
            if (Timer == 0)
            {
                _initVelocity = Projectile.velocity;
                Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            }

            if (Timer == VampireKnives.KnivesLifetime)
            {
                Projectile.active = false;
                return;
            }
            Projectile.timeLeft = 5;
            if (Projectile.numUpdates != -1) return;

            if ((float)Timer / VampireKnives.KnivesLifetime >= VampireKnives.KnivesFadeoutPercent)
            {
                var percentageFade = Utils.Remap(Timer, (int)(VampireKnives.KnivesLifetime * VampireKnives.KnivesFadeoutPercent), VampireKnives.KnivesLifetime, 1, 0);
                Projectile.Opacity = RootsCoreUtils.Ease.Linear(percentageFade);
                Projectile.velocity = _initVelocity * RootsCoreUtils.Ease.InSine(percentageFade);
                Projectile.rotation += 0.2f * (1 - RootsCoreUtils.Ease.InCirc(percentageFade)) * _dieDirection;
            }
            Timer++;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Player.CheckMana((int)(VampireKnives.ManaCost * Player.manaCost), true))
                Projectile.vampireHeal(damageDone, target.Center, target);
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac)
        {
            width = 10;
            height = 10;
            return true;
        }
    }

    public struct VampireKnivesSwordTrailDrawer
    {
        private static readonly VertexStrip VertexStrip = new();

        private static Color StripColors(float progressOnStrip) =>
            Color.Lerp(VampireKnives.TrailColorStart, VampireKnives.TrailColorEnd,
                RootsCoreUtils.Ease.OutQuint(progressOnStrip));
            

        public static void Draw(Projectile proj)
        {
            // TODO: Make generic for anything that uses a custom swing
            if (proj.ModProjectile is not VampireKnivesHoldout v)
                return;

            // TODO: figure out how to make this start smoothly even though we're starting 1 state in (fade out better early in the animation)
            int amountOfFrames = Math.Min(VampireKnives.TrailFrames * proj.MaxUpdates, v.Timer - v.PreStartup.Time);
            if (amountOfFrames < 1)
                return;
            
            using (Main.spriteBatch.Scope())
            {
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, null, null, null, null, Main.GameViewMatrix.TransformationMatrix);
                MiscShaderData miscShaderData = GameShaders.Misc["EmpressBlade"];
                miscShaderData.UseShaderSpecificData(new Vector4(1, 0, 0, 0.6f)); //idk what this does i'm just following vanilla
                miscShaderData.Apply();
                
                // TODO: if player flips around, the rotations get all fucky wucky. Flip the rotations before a flip 
                Vector2[] posToDraw = v.OldPositionPlayerOffset.Take(amountOfFrames).ToArray();
                float[] rotToDraw = proj.oldRot.Take(amountOfFrames).ToArray();
                for (int i = 1; i < rotToDraw.Length; i++)
                {
                    float delta = MathHelper.WrapAngle(rotToDraw[i] - rotToDraw[i - 1]);
                    rotToDraw[i] = rotToDraw[i - 1] + delta;
                }
                SmoothSpike(rotToDraw, 6);

                //Pushing the draw position towards the tip of the knives & re-adding player position
                posToDraw = posToDraw.Select((pos, idx) =>
                    pos + v.Player.Center +
                    (rotToDraw[idx] + (proj.spriteDirection == 0 ? MathHelper.PiOver4 : -MathHelper.PiOver2))
                    .ToRotationVector2() * VampireKnives.TrailOffsetSize).ToArray();
                // TODO: backface culling removal but make it work when facing left
                VertexStrip.PrepareStrip(posToDraw, rotToDraw, StripColors, _ => VampireKnives.TrailWidth, -Main.screenPosition, posToDraw.Length, true);
                VertexStrip.DrawTrail();
                Main.pixelShader.CurrentTechnique.Passes[0].Apply();
                Main.spriteBatch.End();
            }
        }

        private static void SmoothSpike(float[] values, int radius)
        {
            int spikeIndex = -1;
            float spikeSize = 0f;

            for (int i = 1; i < values.Length - 2; i++)
            {
                float distance = Math.Abs(values[i + 1] - values[i]);

                if (!(distance > spikeSize)) continue;
                spikeSize = distance;
                spikeIndex = i;

            }

            if (spikeIndex == -1) return;
            
            int lo = Math.Max(1, spikeIndex - radius);
            int hi = Math.Min(spikeIndex + radius, values.Length - 2);
            if (hi - lo < 2) return;

            for (int i = lo; i <= hi; i++)
            {
                float s = (float)(i - lo) / (hi - lo);
                values[i] = MathHelper.CatmullRom(values[lo - 1], values[lo], values[hi], values[hi + 1], s);
            }
        }
    }
}
