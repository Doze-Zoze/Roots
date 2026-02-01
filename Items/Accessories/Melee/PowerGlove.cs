using RootsBeta.Utilities;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RootsBeta.Items.Accessories.Melee
{
    public class PowerGlove : GlobalItem
    {
        public override bool IsLoadingEnabled(Mod mod) => Configs.instance.RemoveClasses;
        public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.PowerGlove;

        public override void SetDefaults(Item entity)
        {
            ItemSets.DontUseVanillaEquipEffects[ItemID.PowerGlove] = true;
        }
        public override void UpdateEquip(Item item, Player player)
        {
            player.Roots().shootSpeedMult *= 1.1f;
            player.Roots().forceAutoswing = true;
            player.kbGlove = true;
            player.meleeScaleGlove = true;
        }
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            tooltips.ReplaceTooltipWith("Accessories.PowerGlove.Tooltip");
        }
    }
}
