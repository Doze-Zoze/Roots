using RootsBeta.Utilities;
using RootsCore;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RootsBeta.Items.Accessories.Melee
{
    public class FeralClaws : GlobalItem
    {
        #region Parameters
        public static float ShootSpeedMultiplier => 1.1f;
        #endregion

        public override bool IsLoadingEnabled(Mod mod) => Configs.Instance.RemoveClasses;
        public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.FeralClaws;
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
