using Roots.Config;
using RootsBeta.Utilities;
using RootsCore;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RootsBeta.Items.Accessories.Melee
{
    public class FeralClaws : ConfigurableItemRework
    {
        #region Parameters
        public static float ShootSpeedMultiplier => 1.1f;
        #endregion

        public override int[] ItemIds => [ItemID.FeralClaws];
        public override void SetDefaults(Item entity) => ItemSets.DontUseVanillaEquipEffects[ItemID.FeralClaws] = true;
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips) => 
            tooltips.ReplaceTooltipWith("Accessories.FeralClaws.Tooltip");

        public override void UpdateEquip(Item item, Player player)
        {
            player.Roots().ShootSpeedMult *= ShootSpeedMultiplier;
            player.Roots().ForceAutoSwing = true;
        }
    }
}
