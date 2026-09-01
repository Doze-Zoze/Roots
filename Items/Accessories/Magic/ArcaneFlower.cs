using Microsoft.Xna.Framework;
using Roots.Config;
using RootsBeta.Utilities;
using RootsCore;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RootsBeta.Items.Accessories.Magic
{
    public class ArcaneFlower : ConfigurableItemRework
    {
        #region Parameters
        public static float MinimumManaMagicDamageModifier => 0.5f;
        public static float MaximumManaMagicDamageModifier => 1.0f;
        public static int MaximumManaRegen => 120;
        #endregion
        
        public override int[] ItemIds => [ItemID.ArcaneFlower];
        public override void SetStaticDefaults() => ItemSets.DontUseVanillaEquipEffects[ItemID.ArcaneFlower] = true;
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips) => 
            tooltips.ReplaceTooltipWith("Accessories.ArcaneFlower.Tooltip");

        public override void UpdateEquip(Item item, Player player)
        {
            float playerManaRatio = player.statMana / (float)player.statManaMax2;
            player.Roots().ManaFlowerReduction *= MathHelper.Lerp(MinimumManaMagicDamageModifier, MaximumManaMagicDamageModifier, playerManaRatio);
            player.manaRegenCount += (int)(MaximumManaRegen * (1 - playerManaRatio));
        }
    }
}
