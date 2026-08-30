using RootsBeta.Utilities;
using RootsCore;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RootsBeta.Items.ArmorSets
{
    public class FlinxFurCoat : GlobalItem
    {
        #region Parameters
        public static float SummonDamageBonus => 0.1f;
        public static int MinionSlots => 1;
        #endregion
        
        public override bool IsLoadingEnabled(Mod mod) => Configs.Instance.RemoveClasses;
        public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.FlinxFurCoat;
        public override bool InstancePerEntity => true;
        public override void SetStaticDefaults() => ItemSets.DontUseVanillaEquipEffects[ItemID.FlinxFurCoat] = true;
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips) =>
            tooltips.ReplaceTooltipWith("Armor.FlinxFurCoat.Tooltip");

        public override void UpdateEquip(Item item, Player player)
        {
            player.Roots().ModifyHitNPCWithProjectileFuncs.Add((plr, proj, _, modifiers) =>
            {
                if (proj.IsMinionOrSentryRelated)
                    plr.Roots().AdditiveDamageMultipliersToApplyOnHit += SummonDamageBonus;
                return modifiers;
            });
            player.maxMinions += MinionSlots;
        }
    }
}
