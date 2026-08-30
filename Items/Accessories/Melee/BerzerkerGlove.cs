using RootsBeta.Utilities;
using RootsCore;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RootsBeta.Items.Accessories.Melee
{
    public class BerserkerGlove : GlobalItem
    {
        #region Parameters
        public static float ShootSpeedMultiplier => PowerGlove.ShootSpeedMultiplier;
        #endregion

        public override bool IsLoadingEnabled(Mod mod) => Configs.Instance.RemoveClasses;
        public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.BerserkerGlove;
        public override void SetDefaults(Item entity) => ItemSets.DontUseVanillaEquipEffects[ItemID.BerserkerGlove] = true;
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
