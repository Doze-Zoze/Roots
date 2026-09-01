using RootsCore;
using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Roots.Config;

namespace RootsBeta.Items.Weapons
{
    public class DeathSickle : ConfigurableItemRework
    {
        #region Parameters
        public static int ManaCost => 12;
        #endregion

        public override int[] ItemIds => [ItemID.DeathSickle];

        public override void SetStaticDefaults()
        {
            ItemSets.DontConsumeManaOnSwing[ItemID.DeathSickle] = true;
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
