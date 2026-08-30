using RootsBeta.Utilities;
using RootsCore;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RootsBeta.Items.Accessories.Magic
{
    public class MagnetFlower : GlobalItem
    {
        #region Parameters
            public static int ManaStarPickupGraceFrames => 300;
            public static float MagicDamageReduction => ManaFlower.MagicDamageReduction;
            public static int ManaRegen => ManaFlower.ManaRegen;
        #endregion

        public override bool IsLoadingEnabled(Mod mod) => Configs.Instance.ManaChanges;
        public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.MagnetFlower;
        public override void SetStaticDefaults() => ItemSets.DontUseVanillaEquipEffects[ItemID.MagnetFlower] = true;
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips) =>
            tooltips.ReplaceTooltipWith("Accessories.MagnetFlower.Tooltip");

        public override void UpdateEquip(Item item, Player player)
        {
            if (player.Roots().TimeSinceManaStarPickup >= ManaStarPickupGraceFrames)
                player.Roots().ManaFlowerReduction *= MagicDamageReduction;
            player.manaRegenCount += ManaRegen;
        }
    }
}
