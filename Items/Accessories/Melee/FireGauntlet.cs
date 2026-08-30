using RootsBeta.Utilities;
using RootsCore;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RootsBeta.Items.Accessories.Melee
{
    public class FireGauntlet : GlobalItem
    {
        #region Parameters
        public static float ShootSpeedMultiplier => MechanicalGlove.ShootSpeedMultiplier;
        public static float DamageBonus => MechanicalGlove.DamageBonus;
        #endregion

        public override bool IsLoadingEnabled(Mod mod) => Configs.Instance.RemoveClasses;
        public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.FireGauntlet;
        public override void SetDefaults(Item entity) => ItemSets.DontUseVanillaEquipEffects[ItemID.FireGauntlet] = true;
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips) =>
            tooltips.ReplaceTooltipWith("Accessories.FireGauntlet.Tooltip");

        public override void UpdateEquip(Item item, Player player)
        {
            player.Roots().ShootSpeedMult *= ShootSpeedMultiplier;
            player.Roots().ForceAutoSwing = true;
            player.magmaStone = true;
            player.kbGlove = true;
            player.meleeScaleGlove = true;
            player.GetDamage<GenericDamageClass>() += DamageBonus;
        }
    }
}
