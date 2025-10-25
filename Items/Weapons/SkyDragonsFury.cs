using Microsoft.Xna.Framework;
using Roots.Utilities;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Roots.Items.Weapons
{
    public class SkyDragonsFury : GlobalItem
    {
        public override bool IsLoadingEnabled(Mod mod) => Configs.instance.ManaChanges;
        public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.MonkStaffT3;
        public override void SetStaticDefaults()
        {
            ItemSets.DontConsumeManaOnSwing[ItemID.MonkStaffT3] = true;
            ItemSets.ShouldResetManaRegen[ItemID.MonkStaffT3] = (x => x.Item1.altFunctionUse == 2 && x.Item1.statMana >= (int)(x.Item2.mana * x.Item1.manaCost));
        }

        public override void SetDefaults(Item item)
        {
            item.mana = 30;
        }
        public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.altFunctionUse != 2)
                return true;
            if (player.CheckMana((int)Math.Max(0, item.mana * player.manaCost), true))
            {
                return true;
            }
            return false;
        }
    }
}
