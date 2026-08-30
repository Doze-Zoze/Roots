using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using RootsCore;

namespace RootsBeta.Items.Weapons
{
    public class IceBlade : GlobalItem
    {
        #region Parameters
        public static int ManaCost => 10;
        #endregion

        public override bool IsLoadingEnabled(Mod mod) => Configs.Instance.ManaChanges;
        public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.IceBlade;

        public override void SetStaticDefaults()
        {
            ItemSets.DontConsumeManaOnSwing[ItemID.IceBlade] = true;
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
