using Roots.Config;
using RootsBeta.Utilities;
using RootsCore;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RootsBeta.Items.Accessories.Magic
{
    public class MagnetFlower : ConfigurableItemRework<MagnetFlower>, IConfigurableContent<MagnetFlower>
    {
        public static ConfigGroup ConfigGroups => ConfigGroup.AccessoryReworks;

        #region Parameters
        public static int ManaStarPickupGraceFrames => 300;
            public static float MagicDamageReduction => ManaFlower.MagicDamageReduction;
            public static int ManaRegen => ManaFlower.ManaRegen;
        #endregion

        public override int[] ItemIds => [ItemID.MagnetFlower];
        public override void SetStaticDefaults() => ItemSets.DontUseVanillaEquipEffects[ItemID.MagnetFlower] = true;
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips) =>
            tooltips.ReplaceTooltipWith("Accessories.MagnetFlower.Tooltip");

        public override void UpdateAccessory(Item item, Player player, bool hideVisual)
        {
            if (player.Roots().TimeSinceManaStarPickup >= ManaStarPickupGraceFrames)
                player.Roots().ManaFlowerReduction *= MagicDamageReduction;
            player.manaRegenCount += ManaRegen;
        }
    }
}
