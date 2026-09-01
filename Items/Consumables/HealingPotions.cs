using Roots.Config;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RootsBeta.Items.Consumables
{
    public class HealingPotions : GlobalItem, IConfigurableContent
    {
        #region Parameters

        public static float HealMultiplier => 2f;
        #endregion

        private static List<int> ItemsToCount =>
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
        public override bool IsLoadingEnabled(Mod mod) => this.ConfigEnabled;
        public override bool AppliesToEntity(Item item, bool lateInstantiation) => ItemsToCount.Contains(item.type);

        public override void SetDefaults(Item entity)
        {
            entity.healLife = (int)(entity.healLife * HealMultiplier);
        }
    }
}
