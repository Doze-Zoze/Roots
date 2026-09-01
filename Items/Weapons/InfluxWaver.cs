using Microsoft.Xna.Framework;
using RootsBeta.Utilities;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using RootsCore;
using Roots.Config;

namespace RootsBeta.Items.Weapons
{
    public class InfluxWaver : ConfigurableItemRework
    {
        #region Parameters

        public static int ElectrifiedFrames => 60;
        public static int ManaCost => 20;
        private static float ElectrifiedMultiplier => 2f;
        #endregion

        public override int[] ItemIds => [ItemID.InfluxWaver];
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips) =>
            tooltips.AppendTooltipWith("Weapons.InfluxWaver.Tooltip");

        public override void SetStaticDefaults()
        {
            ItemSets.DontConsumeManaOnSwing[ItemID.InfluxWaver] = true;
            ItemSets.ShouldResetManaRegen[ItemID.InfluxWaver] = 
                x => !x.Item1.electrified && x.Item1.statMana >= (int)(x.Item2.mana * x.Item1.manaCost);
        }

        public override void HoldItem(Item item, Player player)
        {
            if (!player.controlUseTile) return;
            player.AddBuff(BuffID.Electrified, ElectrifiedFrames);
            player.manaRegenDelay = 0;
        }

        public override void SetDefaults(Item item)
        {
            item.mana = ManaCost;
        }

        public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (!player.electrified) return player.CheckMana((int)Math.Max(0, item.mana * player.manaCost), true);
            Projectile.NewProjectile(source, position, velocity, type, (int)(damage * ElectrifiedMultiplier), knockback, player.whoAmI);
            return false;
        }
    }
}
