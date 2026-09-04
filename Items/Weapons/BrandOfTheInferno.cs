using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Roots.Config;
using RootsBeta.Utilities;
using RootsCore.ContentBaseClasses;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace RootsBeta.Items.Weapons
{
    public class BrandOfTheInferno : ConfigurableItemRework<BrandOfTheInferno>, IConfigurableContent<BrandOfTheInferno>
    {
        public static ConfigGroup ConfigGroups => ConfigGroup.WeaponReworks;

        #region Parameters
        public static float StrongSwingScale => 1.5f;
        public static float StrongSwingBonusDamage => 2f;
        #endregion

        public override int[] ItemIds => [ItemID.DD2SquireDemonSword];
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips) =>
            tooltips.AppendTooltipWith("Weapons.BrandOfTheInferno.Tooltip");

        public override void SetDefaults(Item item)
        {
            item.shoot = ModContent.ProjectileType<BrandOfTheInfernoHoldout>();
            item.useTime = item.useAnimation = 30;
            item.noMelee = true;
            item.noUseGraphic = true;
            item.UseSound = null;

        }
        public override bool CanUseItem(Item item, Player player)
        {
            if (player.itemTime > 0 || player.ownedProjectileCounts[item.shoot] > 0)
            {
                return false;
            }
            return base.CanUseItem(item, player);
        }
    }

    public class BrandOfTheInfernoHoldout : BaseCustomSwingProjectile<BrandOfTheInfernoHoldout>
    {
        public AttackState Startup = new()
        {
            Time = 10,
            SwingWidth = 3.5f,
            RotationSpeed = 0.25f,
            CanDamage = false,
            SwingOffsetAngle = (proj) =>
            {
                return proj.State.SwingWidth * -0.5f - 0.5f * (1 - MathF.Pow(proj.StateCompletion, 0.5f));
            },
            OffsetDistance = 48,
            AlternateSwings = true
        };
        public AttackState Attack = new()
        {
            Time = 10,
            SwingWidth = 3.5f,
            CanDamage = true,
            OffsetDistance = 48,
            SwingOffsetAngle = null,
            AlternateSwings = true,
            Sound = SoundID.Item1
        };
        public AttackState Endlag = new()
        {
            Time = 10,
            SwingWidth = 3.5f,
            CanDamage = false,
            RotationSpeed = 0.25f,
            SwingOffsetAngle = (proj) =>
            {
                return proj.State.SwingWidth * 0.5f + 0.5f * MathF.Pow(proj.StateCompletion, 0.5f);
            },
            OffsetDistance = 48,
            AlternateSwings = true
        };

        public AttackState StrongStartup = new()
        {
            Time = 40,
            SwingWidth = 3.5f,
            RotationSpeed = 0.01f,
            CanDamage = false,
            SwingOffsetAngle = (proj) =>
            {
                return proj.State.SwingWidth * -0.5f + (0.75f - 0.75f * MathF.Pow(proj.StateCompletion, 0.5f));
            },
            OffsetDistance = 48
        };
        public AttackState StrongAttack = new()
        {
            Time = 12,
            SwingWidth = MathHelper.TwoPi,
            CanDamage = true,
            Loops = true,
            OffsetDistance = 48,
            Sound = SoundID.DD2_MonkStaffSwing,
            SwingOffsetAngle = (proj) =>
            {
                return MathHelper.Lerp(-1.75f, proj.SwingWidth - 1.75f, proj.StateCompletion);
            },
            AdditionalAI = (proj) =>
            {
                if (proj.StateCompletion != 1) return;
                proj.Projectile.ResetLocalNPCHitImmunity();
                proj.Projectile.ai[1]++;
                SoundEngine.PlaySound(proj.State.Sound, proj.Projectile.Center);
                if (proj.Projectile.ai[1] >= 2)
                    proj.SetState(proj.StrongEndlag);
            }
        };
        public AttackState StrongEndlag = new()
        {
            Time = 40,
            SwingWidth = 4.5f,
            CanDamage = true,
            SwingOffsetAngle = (proj) =>
            {
                return MathHelper.Lerp(
                MathHelper.Lerp(-1.75f, (MathHelper.TwoPi - 1.75f) * (40f / 12), proj.StateCompletion),
                MathHelper.Lerp(-1.75f, proj.SwingWidth - 1.75f, proj.StateCompletion),
                MathF.Pow(proj.StateCompletion, 0.45f));
            },
            OffsetDistance = 48
        };
        public override List<AttackState> StateList => IsStrongSwing ? [StrongStartup, StrongAttack, StrongEndlag] : [Startup, Attack, Endlag];
        public override Item BaseItem => ContentSamples.ItemsByType[ItemID.DD2SquireDemonSword];
        public override string Texture => $"Terraria/Images/Item_{ItemID.DD2SquireDemonSword}";
        public override float LineCollisionLength => 100;
        public override int AfterImageCount => 15;
        private bool IsStrongSwing { get => Projectile.ai[0] == 1f; set => Projectile.ai[0] = value ? 1f : 0f; }
        public override void FakeOnSpawn()
        {
            if (Player.HasBuff(BuffID.ParryDamageBuff))
            {
                IsStrongSwing = true;
                Projectile.scale *= BrandOfTheInferno.StrongSwingScale;
            }
            base.FakeOnSpawn();
        }
        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            if (IsStrongSwing)
                modifiers.SourceDamage *= BrandOfTheInferno.StrongSwingBonusDamage;
            base.ModifyHitNPC(target, ref modifiers);
        }
        public static Asset<Texture2D> GlowMask => field ??= ModContent.Request<Texture2D>($"Terraria/Images/ItemFlame_{ItemID.DD2SquireDemonSword}");
        public override void PostDraw(Color lightColor)
        {
            Main.EntitySpriteDraw(GlowMask.Value, Projectile.Center - Main.screenPosition, GlowMask.Frame(),
                Color.White with { A = 0}, Projectile.rotation, GlowMask.Size() * 0.5f, Projectile.scale,
                Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally);
        }
    }
}
