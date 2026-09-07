using Roots.Config;
using RootsBeta.Utilities;
using RootsCore;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RootsBeta.Items.Accessories.Melee
{
    public class MechanicalGlove : ConfigurableItemRework<MechanicalGlove>, IConfigurableContent<MechanicalGlove>
    {
        public static ConfigGroup ConfigGroups => ConfigGroup.AccessoryReworks;

        #region Parameters
        public static float ShootSpeedMultiplier => PowerGlove.ShootSpeedMultiplier;
        public static float DamageBonus => 0.1f;
        #endregion

        public override int[] ItemIds => [ItemID.MechanicalGlove];
        public override void SetDefaults(Item entity) => ItemSets.DontUseVanillaEquipEffects[ItemID.MechanicalGlove] = true;
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips) =>
            tooltips.ReplaceTooltipWith("Accessories.MechanicalGlove.Tooltip");

        public override void UpdateAccessory(Item item, Player player, bool hideVisual)
        {
            player.Roots().ShootSpeedMult *= ShootSpeedMultiplier;
            player.Roots().ForceAutoSwing = true;
            player.kbGlove = true;
            player.meleeScaleGlove = true;
            player.GetDamage<GenericDamageClass>() += DamageBonus;
        }
    }
}
