using Roots.Config;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.Config.UI;

namespace RootsBeta.Items.Consumables
{
    public class HealingPotions : ConfigurableItemRework<HealingPotions>, IConfigurableContent<HealingPotions>
    {
        public static string ConfigGroupName => "Regen & Healing Potions";
        #region Parameters
        public static float HealMultiplier => 2f;
        #endregion
        public override int[] ItemIds =>
        [
            ItemID.LesserHealingPotion,
            ItemID.GreaterHealingPotion,
            ItemID.HealingPotion,
            ItemID.SuperHealingPotion,
            ItemID.RestorationPotion,
            ItemID.LesserRestorationPotion,
            ItemID.BottledHoney,
            ItemID.BottledWater,
            ItemID.Honeyfin,
            ItemID.Eggnog
        ];
        public override void SetDefaults(Item entity)
        {
            entity.healLife = (int)(entity.healLife * HealMultiplier);
        }
    }
}
