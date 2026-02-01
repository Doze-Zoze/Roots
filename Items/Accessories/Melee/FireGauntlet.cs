using RootsBeta.Utilities;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RootsBeta.Items.Accessories.Melee
{
    public class FireGauntlet : GlobalItem
    {
        public override bool IsLoadingEnabled(Mod mod) => Configs.instance.RemoveClasses;
        public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.FireGauntlet;

        public override void SetDefaults(Item entity)
        {
            ItemSets.DontUseVanillaEquipEffects[ItemID.FireGauntlet] = true;
        }
        public override void UpdateEquip(Item item, Player player)
        {
            player.Roots().shootSpeedMult *= 1.1f;
            player.Roots().forceAutoswing = true;
            player.magmaStone = true;
            player.kbGlove = true;
            player.meleeScaleGlove = true;
            player.GetDamage<GenericDamageClass>() += 0.1f;
        }
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            tooltips.ReplaceTooltipWith("Accessories.FireGauntlet.Tooltip");
        }
    }
}
