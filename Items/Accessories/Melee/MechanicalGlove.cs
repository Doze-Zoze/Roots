using Roots.Utilities;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Roots.Items.Accessories.Melee
{
    public class MechanicalGlove : GlobalItem
    {
        public override bool IsLoadingEnabled(Mod mod) => Configs.instance.RemoveClasses;
        public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.MechanicalGlove;

        public override void SetDefaults(Item entity)
        {
            ItemSets.DontUseVanillaEquipEffects[ItemID.MechanicalGlove] = true;
        }
        public override void UpdateEquip(Item item, Player player)
        {
            player.Roots().shootSpeedMult *= 1.1f;
            player.Roots().forceAutoswing = true;
            player.kbGlove = true;
            player.meleeScaleGlove = true;
            player.GetDamage<GenericDamageClass>() += 0.1f;
        }
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            tooltips.ReplaceTooltipWith("Accessories.MechanicalGlove.Tooltip");
        }
    }
}
