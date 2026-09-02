using Roots.Config;
using RootsBeta.Utilities;
using RootsCore;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RootsBeta.Items.Accessories.Melee
{
    public class FireGauntlet : ConfigurableItemRework<FireGauntlet>, IConfigurableContent<FireGauntlet>
    {
        public static ConfigGroup ConfigGroups => ConfigGroup.AccessoryReworks;

        #region Parameters
        public static float ShootSpeedMultiplier => MechanicalGlove.ShootSpeedMultiplier;
        public static float DamageBonus => MechanicalGlove.DamageBonus;
        #endregion

        public override int[] ItemIds => [ItemID.FireGauntlet];
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
