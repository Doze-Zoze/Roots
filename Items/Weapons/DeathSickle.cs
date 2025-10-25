using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Roots.Items.Weapons
{
    public class DeathSickle : GlobalItem
    {
        public override bool IsLoadingEnabled(Mod mod) => Configs.instance.ManaChanges;
        public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.DeathSickle;
        public override void SetStaticDefaults()
        {
            ItemSets.DontConsumeManaOnSwing[ItemID.DeathSickle] = true;
        }

        public override void SetDefaults(Item item)
        {
            item.mana = 12;
        }
        public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.CheckMana((int)Math.Max(0, item.mana * player.manaCost), true))
            {
                return true;
            }
            return false;
        }
    }
}
