using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using RootsCore;
using Roots.Config;

namespace RootsBeta.Items.Weapons
{
    public class ChlorophyteSaber : ConfigurableItemRework<ChlorophyteSaber>, IConfigurableContent<ChlorophyteSaber>
    {
        public static ConfigGroup ConfigGroups => ConfigGroup.WeaponReworks;

        #region Parameters
        public static int ManaCost => 8;
        #endregion

        public override int[] ItemIds => [ItemID.ChlorophyteSaber];
        public override void SetStaticDefaults()
        {
            ItemSets.DontConsumeManaOnSwing[ItemID.ChlorophyteSaber] = true;
        }

        public override void SetDefaults(Item item)
        {
            item.mana = ManaCost;
        }

        public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            return player.CheckMana((int)Math.Max(0, item.mana * player.manaCost), true);
        }
    }
}
