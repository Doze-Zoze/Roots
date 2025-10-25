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
    public class InfluxWaver : GlobalItem
    {
        public override bool IsLoadingEnabled(Mod mod) => Configs.instance.ManaChanges;
        public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.InfluxWaver;
        public override void SetStaticDefaults()
        {
            ItemSets.DontConsumeManaOnSwing[ItemID.InfluxWaver] = true;
            ItemSets.ShouldResetManaRegen[ItemID.InfluxWaver] = x => !x.Item1.electrified && x.Item1.statMana >= (int)(x.Item2.mana * x.Item1.manaCost);
        }

        public override void SetDefaults(Item item)
        {
            item.mana = 10;
        }
        public override bool Shoot(Item item, Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            if (player.electrified)
                return true;
            if (player.CheckMana((int)Math.Max(0, item.mana * player.manaCost), true))
            {
                return true;
            }
            return false;
        }
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips) =>
            tooltips.AppendTooltipWith("Weapons.InfluxWaver.Tooltip");
    }
}
