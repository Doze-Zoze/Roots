using Microsoft.CodeAnalysis.Differencing;
using Microsoft.Xna.Framework;
using Roots;
using Roots.Utilities;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Roots.Items.ArmorSets
{
    public class FlinxFurCoat : GlobalItem
    {
        public override bool IsLoadingEnabled(Mod mod) => Configs.instance.RemoveClasses;

        public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.FlinxFurCoat;

        public override bool InstancePerEntity => true;

        public override void SetStaticDefaults()
        {
            ItemSets.DontUseVanillaEquipEffects[ItemID.FlinxFurCoat] = true;
        }

        public override void UpdateEquip(Item item, Player player)
        {
            player.Roots().ModifyHitNPCWithProjectileFuncs.Add((player, proj, npc, mod) =>
            {
                if (proj.IsMinionOrSentryRelated)
                    player.Roots().AdditiveDamageMultipliersToApplyOnHit += 0.1f;
                return mod;
            });
            player.maxMinions++;
        }
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            tooltips.ReplaceTooltipWith("Armor.FlinxFurCoat.Tooltip");
        }

    }
}
