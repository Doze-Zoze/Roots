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
    public class FlyingDragon : ConfigurableItemRework<FlyingDragon>, IConfigurableContent<FlyingDragon>
    {
        public static ConfigGroup ConfigGroups => ConfigGroup.WeaponReworks;

        #region Parameters
        public static int LifeCost => 5;
        public static int ManaCost => 12;
        #endregion

        public override int[] ItemIds => [ItemID.DD2SquireBetsySword];
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips) =>
            tooltips.AppendTooltipWith("Weapons.FlyingDragon.Tooltip");

        public override void SetStaticDefaults()
        {
            ItemSets.DontConsumeManaOnSwing[ItemID.DD2SquireBetsySword] = true;
            ItemSets.ShouldResetManaRegen[ItemID.DD2SquireBetsySword] = 
                x => x.Item1.statMana >= (int)(x.Item2.mana * x.Item1.manaCost) || x.Item1.statLife > LifeCost;
        }

        public override void SetDefaults(Item item)
        {
            item.mana = ManaCost;
        }

        public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.CheckMana((int)Math.Max(0, item.mana * player.manaCost), true))
            {
                return true;
            }
            if (player.statLife <= LifeCost) return true;
            player.lifeRegenCount -= 120 * LifeCost;
            player.CheckMana(0, true);
            return true;
        }
    }
}
