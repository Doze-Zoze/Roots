using Roots.Utilities;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Roots.Items.Accessories.Melee
{
    public class BerserkerGlove : GlobalItem
    {
        public override bool IsLoadingEnabled(Mod mod) => Configs.instance.RemoveClasses;
        public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.BerserkerGlove;

        public override void SetDefaults(Item entity)
        {
            ItemSets.DontUseVanillaEquipEffects[ItemID.BerserkerGlove] = true;
        }
        public override void UpdateEquip(Item item, Player player)
        {
            player.Roots().forceAutoswing = true;
            player.magmaStone = true;
            player.kbGlove = true;
            player.meleeScaleGlove = true;
        }
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            tooltips.ReplaceTooltipWith("Accessories.BerserkerGlove.Tooltip");
        }
    }
}
