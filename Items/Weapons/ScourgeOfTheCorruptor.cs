using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoMod.Logs;
using Roots.Config;
using RootsBeta.Utilities;
using RootsCore.ContentBaseClasses;
using RootsCore.Extensions;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace RootsBeta.Items.Weapons
{
    public class ScourgeOfTheCorruptor : ConfigurableCustomSwing<ScourgeOfTheCorruptor>, IConfigurableContent<ScourgeOfTheCorruptor>
    {
        public static ConfigGroup ConfigGroups => ConfigGroup.WeaponReworks;

        #region Parameters

        public static int SpearDamage => 500;
        public static int EaterDmg => 30;
        public static int EaterHitrate => 30;
        public static int EaterExplodeMult => 10;
        public static int EaterCountPerSpear => 4;

        public static float EaterMinionSlotCount => 0.25f;
        public static int ParryCooldown => 90;

        public static float ParryProjectileDmgMult => 0.5f;


        #endregion

        public override int[] ItemIds => [ItemID.ScourgeoftheCorruptor];
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips) =>
            tooltips.ReplaceTooltipWith("Weapons.ScourgeoftheCorruptor.Tooltip");
        public override void SetStaticDefaults()
        {
            ItemID.Sets.ItemsThatAllowRepeatedRightClick[ItemID.ScourgeoftheCorruptor] = true;
        }
        public override void SetDefaults(Item item)
        {
            base.SetDefaults(item);
            item.damage = SpearDamage;
            item.shoot = ModContent.ProjectileType<ScourgeOfTheCorruptorHoldout>();
            item.useTime = item.useAnimation = 30;
            item.flame = true;
        }

        public override bool AltFunctionUse(Item item, Player player) => player.Roots().ScourgeParryCooldownTime <= 0;

        public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse == 2)
            {
                Projectile.NewProjectile(source, position, velocity, ModContent.ProjectileType<ScourgeOfTheCorruptorParry>(), damage, knockback, player.whoAmI);
                return false;
            }
            return base.Shoot(item, player, source, position, velocity, type, damage, knockback);
        }
    }

    public class ScourgeOfTheCorruptorHoldout : BaseCustomSwingProjectile<ScourgeOfTheCorruptorHoldout>
    {
        public AttackState Startup = new()
        {
            Time = 20,
            SwingWidth = 3f,
            RotationSpeed = 1f,
            CanDamage = false,
            SwingOffsetAngle = proj =>
                proj.State.SwingWidth * -0.5f - MathHelper.PiOver2 + 0.25f * (1 - proj.StateCompletion),
            OffsetDistance = 16,
            AlternateSwings = false
        };
        public AttackState Swing = new()
        {
            Time = 6,
            SwingWidth = 3f,
            RotationSpeed = 1f,
            CanDamage = false,
            SwingOffsetAngle = proj =>
                proj.State.SwingWidth * -0.5f * (1 - proj.StateCompletion) - MathHelper.PiOver2,
            OffsetDistance = 16,
            AlternateSwings = false
        };
        public AttackState Attack = new()
        {
            Time = 8,
            SwingWidth = 3f,
            CanDamage = false,
            OffsetDistance = 16,
            SwingOffsetAngle = proj =>
                proj.State.SwingWidth * 0.5f * (proj.StateCompletion) - MathHelper.PiOver2,
            Startup = proj =>
                Projectile.NewProjectile(proj.Projectile.GetSource_FromThis(), proj.Projectile.Center + proj.Angle * 16, (proj.Projectile.Center + proj.Angle * 16).DirectionTo(proj.Player.MouseWorld) * 4, ModContent.ProjectileType<ScourgeOfTheCorruptor_ThrownProjectile>(), proj.Projectile.damage, proj.Projectile.knockBack, proj.Projectile.owner),
            AlternateSwings = false,
            Sound = SoundID.Item1
        };
        public override List<AttackState> StateList => [Startup, Swing, Attack];
        public override Item BaseItem => ContentSamples.ItemsByType[ItemID.ScourgeoftheCorruptor];
        public override string Texture => $"Terraria/Images/Item_{ItemID.ScourgeoftheCorruptor}";
        public override bool UseMeleeSize => false;
        public override void AdditionalAI()
        {
            Projectile.rotation += MathHelper.PiOver2 * Projectile.spriteDirection;
            BaseScale = 1;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            if (State == Attack)
                return false;

            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, texture.Frame(),
          lightColor, Projectile.rotation, texture.Size() * 0.5f, Projectile.scale,
          Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally);
            return false;
        }
    }

    public class ScourgeOfTheCorruptorParry : BaseCustomSwingProjectile<ScourgeOfTheCorruptorParry>
    {
        public AttackState Parry = new()
        {
            Time = 20,
            SwingWidth = 0f,
            CanDamage = true,
            OffsetDistance = 16,
            SwingOffsetAngle = proj => 0,
            Startup = proj => { proj.Angle = new Vector2(-proj.Player.direction, 0); },
            AdditionalAI = proj =>
            {

                var sortedProjectiles = (from p in Main.projectile 
                    where p.active && p.hostile && p.damage > 0 
                    select p).OrderByDescending(x => x.damage);

                var hitbox = proj.Player.Hitbox;
                hitbox.Inflate(64, 64);

                foreach (var item in sortedProjectiles)
                {
                    if (!item.Colliding(item.Hitbox, hitbox))
                        continue;
                    item.Roots().DamageModifier *= ScourgeOfTheCorruptor.ParryProjectileDmgMult;
                    proj.TriggerParry();
                    break;
                }
            },
            Sound = SoundID.Item1
        };
        public AttackState Attack = new()
        {
            Time = 10,
            IsTransitionedTo = false,
            SwingWidth = 3f,
            CanDamage = false,
            OffsetDistance = 36,
            SwingOffsetAngle = proj =>
                -proj.State.SwingWidth * ( -0.5f + (proj.StateCompletion)),
            Startup = proj => {
                proj.Angle = -proj.Player.DirectionTo(proj.Player.MouseWorld);
            },
            AlternateSwings = false,
            Sound = SoundID.Item1
        };
        public override List<AttackState> StateList => [Parry, Attack];
        public override Item BaseItem => ContentSamples.ItemsByType[ItemID.ScourgeoftheCorruptor];
        public override string Texture => $"Terraria/Images/Item_{ItemID.ScourgeoftheCorruptor}";
        public override bool UseMeleeSize => false;

        public override void Spawn()
        {
            Player.Roots().ScourgeParryCooldownTime = ScourgeOfTheCorruptor.ParryCooldown;
            base.Spawn();
        }
        public override void AdditionalAI()
        {
            base.AdditionalAI();
        }

        public void TriggerParry() 
        {
            SetState(Attack);
            Projectile.netUpdate = true;
            Projectile.ResetLocalNPCHitImmunity();
            var eaterMinions = (from p in Main.projectile
                                     where p.active && p.owner == Projectile.owner && p.type == ModContent.ProjectileType<ScourgeOfTheCorruptor_Minion>()
                                     select p);
            foreach (var item in eaterMinions)
            {
                for (var i =0; i < 20; i++)
                    Dust.NewDustPerfect(item.Center, DustID.ScourgeOfTheCorruptor, Main.rand.NextVector2Circular(64,64), Scale: 1.2f);
                SoundEngine.PlaySound(SoundID.DD2_BetsyFireballImpact, Projectile.Center);
                item.Resize(64, 64);
                item.damage *= ScourgeOfTheCorruptor.EaterExplodeMult;
                item.ResetLocalNPCHitImmunity();
                item.Damage();
                item.Kill();
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (State == Parry) 
            {
                Player.SetImmuneTimeForAllTypes(Player.longInvince ? 60 : 30); //TODO - make a util that can handle common iframe amounts automatically from enum
                TriggerParry();
            }
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            if (State == Parry)
            {
                var hitbox = Player.Hitbox;
                hitbox.Inflate(64, 64);
                return targetHitbox.Intersects(hitbox);
            }
            return base.Colliding(projHitbox, targetHitbox);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, texture.Frame(),
          lightColor, Projectile.rotation, texture.Size() * 0.5f, Projectile.scale,
          Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally);
            return false;
        }
    }
    public class ScourgeOfTheCorruptor_ThrownProjectile : ModProjectile
    {
        public override string Texture => $"Terraria/Images/Item_{ItemID.ScourgeoftheCorruptor}";

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.width = Projectile.height = 32;
            Projectile.MaxUpdates = 8;
            Projectile.timeLeft = 120 * Projectile.MaxUpdates;
        }

        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver4;
            Dust.NewDustPerfect(Projectile.Center, DustID.ScourgeOfTheCorruptor, Vector2.Zero, Scale: 0.75f);
        }

        public override void OnKill(int timeLeft)
        {
            if (Main.myPlayer == Projectile.owner)
            {
                for (var i = 0; i < ScourgeOfTheCorruptor.EaterCountPerSpear; i++)
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Main.rand.NextVector2CircularEdge(4, 4), ModContent.ProjectileType<ScourgeOfTheCorruptor_Minion>(), (int)(Projectile.damage * (ScourgeOfTheCorruptor.EaterDmg / (float)ScourgeOfTheCorruptor.SpearDamage)), 0, Projectile.owner);
            }
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, texture.Frame(),
          lightColor, Projectile.rotation, texture.Size() * 0.5f, Projectile.scale,
          Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally);
            return false;
        }
    }

    public class ScourgeOfTheCorruptor_Minion : ModProjectile
    {
        public override string Texture => $"Terraria/Images/Projectile_{ProjectileID.TinyEater}";

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.minionSlots = ScourgeOfTheCorruptor.EaterMinionSlotCount;
            Projectile.minion = true;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.tileCollide = false;
            Projectile.MaxUpdates = 4;
            Projectile.ArmorPenetration = 100;
            for (int i = 0; i < Projectile.localNPCImmunity.Length; i++)
            {
                Projectile.localNPCImmunity[i] = 60;
            }

            Projectile.localNPCHitCooldown = ScourgeOfTheCorruptor.EaterHitrate * Projectile.MaxUpdates;
            Projectile.timeLeft = 600 * Projectile.MaxUpdates;
        }

        public NPC attachTarget
        {
            get
            {
                if (!Main.npc.IndexInRange((int)Projectile.ai[0] - 1))
                    return null;
                var npc = Main.npc[(int)Projectile.ai[0] - 1];
                if (!npc.active || (!npc.CanBeChasedBy(Projectile) && npc.type != NPCID.DukeFishron))
                {
                    Projectile.ai[0] = 0;
                    return null;
                }
                return npc;
            }

            set => Projectile.ai[0] = value.whoAmI + 1;
        }

        public override void AI()
        {
            Projectile.rotation = -Projectile.velocity.ToRotation() - MathHelper.PiOver2;

            if (Projectile.frameCounter++ > 4)
            {
                Projectile.frameCounter = 0;
                Projectile.frame = Projectile.frame++ % 2;
            }

            if (attachTarget is not null)
            {
                Projectile.timeLeft++;
                Projectile.Center = attachTarget.Center;
                return;
            }

            Dust.NewDustPerfect(Projectile.Center, DustID.ScourgeOfTheCorruptor, Vector2.Zero, Scale: 0.5f);
            int attackTarget = -1;
            Projectile.Minion_FindTargetInRange(1200, ref attackTarget, true, (enty, _) => enty is NPC nPC && Projectile.localNPCImmunity[enty.whoAmI] == 0 && nPC.CanBeChasedBy(Projectile));
            if (attackTarget >= 0)
            {
                var target = Main.npc[attackTarget];
                Projectile.velocity += Projectile.DirectionTo(target.Center) * 0.25f;
                Projectile.velocity *= 0.95f;
            }
            else
            {
                Projectile.timeLeft -= 3;
            }

        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Projectile.netUpdate = true;
            attachTarget ??= target;
            Projectile.timeLeft = (int)MathHelper.Max(Projectile.timeLeft, ScourgeOfTheCorruptor.EaterHitrate * Projectile.MaxUpdates * 2);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = TextureAssets.Projectile[Type].Value;
            var frame = texture.Frame(1, 2, 0, Projectile.frame);
            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, frame,
              lightColor, Projectile.rotation, frame.Size() * 0.5f, Projectile.scale,
              Projectile.spriteDirection == 1 ? SpriteEffects.None : SpriteEffects.FlipHorizontally);
            return false;
        }
    }
}
