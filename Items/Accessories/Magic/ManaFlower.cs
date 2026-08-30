using RootsBeta.Utilities;
using RootsCore;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RootsBeta.Items.Accessories.Magic
{
    public class ManaFlower : GlobalItem
    {
        #region Parameters
        public static float MagicDamageReduction => 0.75f;
        public static int ManaRegen => 40;
        #endregion

        public override bool IsLoadingEnabled(Mod mod) => Configs.Instance.ManaChanges;
        public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.ManaFlower;
        public override void SetStaticDefaults() => ItemSets.DontUseVanillaEquipEffects[ItemID.ManaFlower] = true;
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips) =>
            tooltips.ReplaceTooltipWith("Accessories.ManaFlower.Tooltip");
        
        public override void UpdateEquip(Item item, Player player)
        {
            player.Roots().ManaFlowerReduction *= MagicDamageReduction;
            player.manaRegenCount += ManaRegen;
        }
    }
}
