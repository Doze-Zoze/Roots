using Roots.Config;
using RootsBeta.Utilities;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RootsBeta.Items.Accessories.Magic
{
    public class MagicCuffs : ConfigurableItemRework
    {
        #region Parameters
        public static int Defense => 1;
        public static float ManaRegenDelayBonus => 1f;
        public static int ManaRegen => 25;
        #endregion

        public override int[] ItemIds => [ItemID.MagicCuffs,ItemID.CelestialCuffs];
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips) =>
            tooltips.AppendTooltipWith("Accessories.MagicCuffs.Tooltip");

        public override void SetDefaults(Item entity)
        {
            entity.defense = Defense;
        }

        public override void UpdateEquip(Item item, Player player)
        {
            player.manaRegenDelayBonus += ManaRegenDelayBonus;
            player.manaRegenBonus += ManaRegen;
        }
    }
}
