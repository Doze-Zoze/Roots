using Roots.Config;
using RootsBeta.Utilities;
using RootsCore;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RootsBeta.Items.Accessories.Melee
{
    public class BerserkerGlove : ConfigurableItemRework
    {
        #region Parameters
        public static float ShootSpeedMultiplier => PowerGlove.ShootSpeedMultiplier;
        #endregion

        public override int[] ItemIds => [ItemID.BerserkerGlove];
        public override void SetDefaults(Item entity) => ItemSets.DontUseVanillaEquipEffects[ItemIds[0]] = true;
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips) =>
            tooltips.ReplaceTooltipWith("Accessories.BerserkerGlove.Tooltip");

        public override void UpdateEquip(Item item, Player player)
        {
            player.Roots().ForceAutoSwing = true;
            player.Roots().ShootSpeedMult *= ShootSpeedMultiplier;
            player.kbGlove = true;
            player.meleeScaleGlove = true;
        }
    }
}
