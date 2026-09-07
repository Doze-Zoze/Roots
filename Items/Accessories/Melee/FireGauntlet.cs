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

        public override void UpdateAccessory(Item item, Player player, bool hideVisual)
        {
            player.Roots().ShootSpeedMult *= ShootSpeedMultiplier;
            player.Roots().ForceAutoSwing = true;
            if (!ConfigHelpers.ConfigEnabled<MagmaStone>())
                player.magmaStone = true;
            else
                player.Roots().PhysicalOnHitNPCFuncs.Add(MagmaStone.OnHit);
            player.kbGlove = true;
            player.meleeScaleGlove = true;
            player.GetDamage<GenericDamageClass>() += DamageBonus;
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            tooltips.ReplaceTooltipWith("Accessories.FireGauntlet.Tooltip");
            if (!ConfigHelpers.ConfigEnabled<MagmaStone>())
                tooltips.AppendTooltipWith("ItemTooltip.MagmaStone", 0, false);
            else
                tooltips.AppendTooltipWith("Accessories.MagmaStone.Tooltip");
        }
    }
}
