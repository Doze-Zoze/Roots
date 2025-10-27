﻿using Microsoft.Xna.Framework;
using Roots.Projectiles;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Roots.Players
{
    public class RootsPlayer : ModPlayer
    {
        #region Fields/Variables
        public float shootSpeedMult = 1;

        #region Accessories
        public float manaFlowerReduction = 1f;
        public int TimeSinceManaStarPickup = 0;
        public int TimeSinceManaCloakStarAttack = 0;
        public bool forceAutoswing = false;
        #endregion

        #endregion

        public List<Func<Player, Projectile, NPC, NPC.HitModifiers, NPC.HitModifiers>> ModifyHitNPCWithProjectileFuncs = new();
        public List<Func<Player, NPC, NPC.HitModifiers, NPC.HitModifiers>> ModifyHitNPCFuncs = new();
        public List<Action<Player, Projectile, NPC, NPC.HitInfo, int>> OnHitNPCWithProjectileFuncs = new();
        public List<Action<Player, NPC, NPC.HitInfo, int>> OnHitNPCFuncs = new();

        public float AdditiveDamageMultipliersToApplyOnHit = 1;

        public override void ResetEffects()
        {
            if (false && Configs.instance.RemoveBaseCrit)
                Player.GetCritChance(DamageClass.Generic) -= 4;
            //config needed
            if (!Player.shinyStone)
                Player.lifeRegenTime--; //keep natural life regen from happening without things to boost it
            shootSpeedMult = 1;
            AdditiveDamageMultipliersToApplyOnHit = 1;

            #region Accessories And Gear
            manaFlowerReduction = 1f;
            TimeSinceManaStarPickup++;
            TimeSinceManaCloakStarAttack++;
            forceAutoswing = false;


            #endregion
            ModifyHitNPCWithProjectileFuncs = new();
            ModifyHitNPCFuncs = new();
            OnHitNPCFuncs = new();
            OnHitNPCWithProjectileFuncs = new();
            Player.maxMinions--;

        }
        public override void ModifyHitNPCWithProj(Projectile proj, NPC target, ref NPC.HitModifiers modifiers)
        {
            if (manaFlowerReduction != 1f && proj.GetGlobalProjectile<RootsGlobalProjectile>().isManaProjectile)
                modifiers.SourceDamage *= manaFlowerReduction;
            foreach (var info in ModifyHitNPCWithProjectileFuncs)
            {
                modifiers = info(Player, proj, target, modifiers);
            }
            if (AdditiveDamageMultipliersToApplyOnHit != 1)
                modifiers.FinalDamage += ((AdditiveDamageMultipliersToApplyOnHit - 1) / Player.GetTotalDamage(modifiers.DamageType).Additive);
            AdditiveDamageMultipliersToApplyOnHit = 1;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            foreach (var info in ModifyHitNPCFuncs)
            {
                modifiers = info(Player, target, modifiers);
            }
            if (AdditiveDamageMultipliersToApplyOnHit != 1)
                modifiers.FinalDamage += ((AdditiveDamageMultipliersToApplyOnHit - 1) / Player.GetTotalDamage(modifiers.DamageType).Additive);
            AdditiveDamageMultipliersToApplyOnHit = 1;
        }
        public override void UpdateEquips()
        {
            if (Configs.instance.RemoveClasses && Player.kbGlove)
                Player.GetKnockback(DamageClass.Generic) *= 2f;

            int ManaPerMinion = 40;
            float LostMana = ((int)((Player.slotsMinions - Player.maxMinions) * ManaPerMinion));
            Player.maxMinions += (int)(Player.statManaMax2 / ManaPerMinion);
            if (LostMana > 0)
                Player.statManaMax2 -= (int)LostMana;
        }
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            foreach (var func in OnHitNPCFuncs)
            {
                func(Player, target, hit, damageDone);
            }
            if (Configs.instance.RemoveClasses && Player.magmaStone)
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
            velocity *= shootSpeedMult;
        }
        public override bool? CanAutoReuseItem(Item item)
        {
            return forceAutoswing ? true : null;
        }
    }
}
