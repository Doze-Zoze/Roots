using Microsoft.Xna.Framework;
using Roots.Config;
using RootsBeta.Items.Consumables;
using RootsBeta.Projectiles;
using RootsBeta.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RootsBeta.Players
{
    public class RootsPlayer : ModPlayer
    {
        #region Fields/Variables
        public float ShootSpeedMult = 1;

        #region Accessories
        public float ManaFlowerReduction = 1f;
        public int TimeSinceManaStarPickup;
        public int TimeSinceManaCloakStarAttack;
        public bool ForceAutoSwing;
        public float AdditiveManaDamage;
        #endregion

        #endregion
        
        #region Balancing Properties
        public static float CloseRangeDistance => 16 * 25; // 16 pixels per tile
        public static int ManaSickFrames => 900;
        public static int ManaRegenDelayMax => 30;
        public static int ManaPerMinion => 40;
        #endregion

        public List<Func<Player, Projectile, NPC, NPC.HitModifiers, NPC.HitModifiers>> ModifyHitNPCWithProjectileFuncs = [];
        public List<Func<Player, NPC, NPC.HitModifiers, NPC.HitModifiers>> ModifyHitNPCFuncs = [];
        public List<Func<Player, Item, NPC, NPC.HitModifiers, NPC.HitModifiers>> ModifyHitNPCWithItemFuncs = [];
        public List<Action<Player, Projectile, NPC, NPC.HitInfo, int>> OnHitNPCWithProjectileFuncs = [];
        public List<Action<Player, NPC, NPC.HitInfo, int>> OnHitNPCFuncs =[];
        
        public List<Func<Player, NPC, NPC.HitModifiers, NPC.HitModifiers>> PhysicalModifyHitNPCFuncs = [];
        public List<Func<Player, NPC, NPC.HitModifiers, NPC.HitModifiers>> MagicalModifyHitNPCFuncs = [];
        public List<Action<Player, NPC, NPC.HitInfo, int>> PhysicalOnHitNPCFuncs = [];
        public List<Action<Player, NPC, NPC.HitInfo, int>> MagicalOnHitNPCFuncs = [];

        public float AdditiveDamageMultipliersToApplyOnHit = 1;

        public override void ResetEffects()
        {
            Player.manaSickTimeMax = ManaSickFrames;
            Player.manaSickTime = ManaSickFrames;
            Player.manaSickLessDmg = 0;

            Player.manaRegenDelay = Math.Min(Player.manaRegenDelay, ManaRegenDelayMax);

            //rework needed
            if (ConfigHelpers.ConfigEnabled<HealingPotions>() && !Player.shinyStone)
                Player.lifeRegenTime--; //keep natural life regen from happening without things to boost it
            ShootSpeedMult = 1;
            AdditiveDamageMultipliersToApplyOnHit = 1;

            #region Accessories And Gear
            ManaFlowerReduction = 1f;
            TimeSinceManaStarPickup++;
            TimeSinceManaCloakStarAttack++;
            ForceAutoSwing = false;
            AdditiveManaDamage = 0;
            #endregion

            ModifyHitNPCWithProjectileFuncs = [];
            ModifyHitNPCFuncs = [];
            ModifyHitNPCWithItemFuncs = [];
            OnHitNPCFuncs = [];
            OnHitNPCWithProjectileFuncs = [];
            PhysicalModifyHitNPCFuncs = [];
            MagicalModifyHitNPCFuncs = [];
            PhysicalOnHitNPCFuncs = [];
            MagicalOnHitNPCFuncs = [];
            Player.maxMinions--;

            Player.setBonus = "";
        }
        public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref NPC.HitModifiers modifiers)
        {
            if (ManaFlowerReduction != 1f && proj.GetGlobalProjectile<RootsGlobalProjectile>().IsManaProjectile)
                modifiers.SourceDamage *= ManaFlowerReduction;
            modifiers = ModifyHitNPCWithProjectileFuncs.Aggregate(modifiers, (current, info) => info(Player, proj, target, current));
            if (proj.Roots().IsManaProjectile)
            {
                AdditiveDamageMultipliersToApplyOnHit += AdditiveManaDamage;
                modifiers = MagicalModifyHitNPCFuncs.Aggregate(modifiers, (current, info) => info(Player, target, current));
            }
            else if (!proj.IsMinionOrSentryRelated)
            {
                modifiers = PhysicalModifyHitNPCFuncs.Aggregate(modifiers, (current, info) => info(Player, target, current));
            }
            if (AdditiveDamageMultipliersToApplyOnHit != 1)
                modifiers.FinalDamage += ((AdditiveDamageMultipliersToApplyOnHit - 1) / Player.GetTotalDamage(modifiers.DamageType).Additive);
            AdditiveDamageMultipliersToApplyOnHit = 1;

        }

        public override void ModifyHitNPCWithItem(Item item, NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers = PhysicalModifyHitNPCFuncs.Aggregate(modifiers, (current, info) => info(Player, target, current));
            modifiers = ModifyHitNPCWithItemFuncs.Aggregate(modifiers, (current, info) => info(Player, item, target, current));
            if (AdditiveDamageMultipliersToApplyOnHit != 1)
                modifiers.FinalDamage += ((AdditiveDamageMultipliersToApplyOnHit - 1) / Player.GetTotalDamage(modifiers.DamageType).Additive);
            AdditiveDamageMultipliersToApplyOnHit = 1;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers = ModifyHitNPCFuncs.Aggregate(modifiers, (current, info) => info(Player, target, current));
            if (AdditiveDamageMultipliersToApplyOnHit != 1)
                modifiers.FinalDamage += ((AdditiveDamageMultipliersToApplyOnHit - 1) / Player.GetTotalDamage(modifiers.DamageType).Additive);
            AdditiveDamageMultipliersToApplyOnHit = 1;

            modifiers.DamageVariationScale *= 0;
        }

        public override void UpdateEquips()
        {
            if (RootsModConfig.Instance.RemoveClasses && Player.kbGlove)
                Player.GetKnockback(DamageClass.Generic) *= 2f;

            //TODO - High Priority - Make configurable, make it work properly
            int lostMana = (int)((Player.slotsMinions - Player.maxMinions) * ManaPerMinion);
            Player.maxMinions += Player.statManaMax2 / ManaPerMinion;
            if (lostMana > 0)
                Player.statManaMax2 -= lostMana;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            foreach (var func in OnHitNPCFuncs)
            {
                func(Player, target, hit, damageDone);
            }
            if (RootsModConfig.Instance.RemoveClasses && Player.magmaStone)
                target.AddBuff(BuffID.OnFire3, 120);
        }

        public override void OnHitNPCWithProj(Projectile proj, NPC target, NPC.HitInfo hit, int damageDone)
        {
            foreach (var func in OnHitNPCWithProjectileFuncs)
            {
                func(Player, proj, target, hit, damageDone);
            }
        }

        public override void ModifyShootStats(Item item, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            velocity *= ShootSpeedMult;
        }

        public override bool? CanAutoReuseItem(Item item)
        {
            return ForceAutoSwing ? true : null;
        }
    }
}
