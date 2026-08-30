using RootsBeta.Utilities;
using RootsCore;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RootsBeta.Items.Accessories.Melee
{
    public class MechanicalGlove : GlobalItem
    {
        #region Parameters
        public static float ShootSpeedMultiplier => PowerGlove.ShootSpeedMultiplier;
        public static float DamageBonus => 0.1f;
        #endregion

        public override bool IsLoadingEnabled(Mod mod) => Configs.Instance.RemoveClasses;
        public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.MechanicalGlove;
        public override void SetDefaults(Item entity) => ItemSets.DontUseVanillaEquipEffects[ItemID.MechanicalGlove] = true;
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips) =>
            tooltips.ReplaceTooltipWith("Accessories.MechanicalGlove.Tooltip");

        public override void UpdateEquip(Item item, Player player)
        {
            player.Roots().ShootSpeedMult *= ShootSpeedMultiplier;
            player.Roots().ForceAutoSwing = true;
            player.kbGlove = true;
            player.meleeScaleGlove = true;
            player.GetDamage<GenericDamageClass>() += DamageBonus;
        }
    }
}
