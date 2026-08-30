using RootsBeta.Utilities;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RootsBeta.Items.Consumables
{
    public class Mushroom : GlobalItem
    {
        #region Parameters
        public static int Healing => 20;
        public static int HealingCooldownSeconds => 15;
        #endregion

        public override bool IsLoadingEnabled(Mod mod) => Configs.Instance.LifeChanges;
        public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.Mushroom;
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips) =>
            tooltips.ReplaceTooltipWith("Consumables.Mushroom.Tooltip");

        public override void SetDefaults(Item entity)
        {
            entity.healLife = Healing;
        }

        public override void ModifyPotionDelay(Item item, Player player, ref int baseDelay)
        {
            baseDelay = HealingCooldownSeconds * 60;
        }
    }
}
