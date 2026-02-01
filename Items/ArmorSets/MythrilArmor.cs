using Microsoft.CodeAnalysis.Differencing;
using Microsoft.Xna.Framework;
using RootsBeta;
using RootsBeta.Utilities;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace RootsBeta.Items.ArmorSets
{
    public class MythrilHelmets : GlobalItem
    {
        List<int> ItemsToApplyTo =
        [
            ItemID.MythrilHelmet,
            ItemID.MythrilHat,
            ItemID.MythrilHood
        ];
        public override bool IsLoadingEnabled(Mod mod) => Configs.instance.RemoveClasses;

        public override bool AppliesToEntity(Item item, bool lateInstantiation) => ItemsToApplyTo.Contains(item.type);

        public override bool InstancePerEntity => true;

        public override void SetStaticDefaults()
        {
            foreach (var item in ItemsToApplyTo)
            {
                ItemSets.DontUseVanillaEquipEffects[item] = true;
                ItemSets.DontUseVanillaSetBonus[item] = true;
            }
        }

        public override void SetDefaults(Item item)
        {
            item.defense = 3;
        }

        public override void UpdateEquip(Item item, Player player)
        {
            player.GetDamage<GenericDamageClass>() += 0.14f;
            player.statManaMax2 += 80;
            player.ammoCost80 = true;
        }

        public override string IsArmorSet(Item head, Item body, Item legs)
        {
            if (ItemsToApplyTo.Contains(head.type) && body.type == ItemID.MythrilChainmail && legs.type == ItemID.MythrilGreaves)
                return "MythrilSet";
            return string.Empty;
        }

        public override void UpdateArmorSet(Player player, string set)
        {
            if (set == "MythrilSet")
            {
                player.setBonus = RootsUtils.GetLocalizedTextValue("Armor.Mythril.SetBonus");
                player.manaCost *= 0.8f;
                player.Roots().ModifyHitNPCFuncs.Add((player, npc, modifiers) =>
                {
                    modifiers.CritDamage += 0.15f;
                    return modifiers;
                });
            }
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            tooltips.ReplaceTooltipWith("Armor.Mythril.HelmetTooltip");
        }

    }
}
