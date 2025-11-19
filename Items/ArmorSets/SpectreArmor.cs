using Roots.Utilities;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Roots.Items.ArmorSets
{
    public class SpectreArmor : BaseArmorSet
    {
        public override string SetID => "Spectre";
        public override List<int> HeadsToApplyTo => [ItemID.SpectreHood, ItemID.SpectreMask];
        public override List<int> ChestsToApplyTo => [ItemID.SpectreRobe];
        public override List<int> LegsToApplyTo => [ItemID.SpectrePants];

        public override void HeadEquips(Item item, Player player)
        {
            if (item.type != ItemID.SpectreHood)
            {
                player.statManaMax2 += 60;
                player.manaCost -= 0.13f;
                player.GetDamage<GenericDamageClass>() += 0.1f;
                player.GetCritChance<GenericDamageClass>() += 10f;
            }
        }

        public override void ChestEquips(Item item, Player player)
        {
            player.GetDamage<GenericDamageClass>() += 0.07f;
            player.GetCritChance<GenericDamageClass>() += 7f;
        }

        public override void LegsEquips(Item item, Player player)
        {
            player.GetDamage<GenericDamageClass>() += 0.08f;
            player.moveSpeed += 0.08f;
        }


        public override string IsArmorSet(Item head, Item body, Item legs)
        {
            if ((head.type == ItemID.SpectreHood) && (ChestsToApplyTo.Count == 0 || ChestsToApplyTo.Contains(body.type)) && (LegsToApplyTo.Count == 0 || LegsToApplyTo.Contains(legs.type)))
                return SetID + "SetHood";
            if ((head.type == ItemID.SpectreMask) && (ChestsToApplyTo.Count == 0 || ChestsToApplyTo.Contains(body.type)) && (LegsToApplyTo.Count == 0 || LegsToApplyTo.Contains(legs.type)))
                return SetID + "SetMask";
            return string.Empty;
        }

        public override void UpdateArmorSet(Player player, string set)
        {
            if (set == SetID + "SetHood")
            {
                player.setBonus = RootsUtils.GetLocalizedTextValue($"Armor.{SetID}.SetBonusHood");
                player.Roots().AdditiveManaDamage -= 0.4f;
                player.ghostHeal = true;
            }
            else if (set == SetID + "SetMask")
            {
                player.setBonus = RootsUtils.GetLocalizedTextValue($"Armor.{SetID}.SetBonusMask");
                player.ghostHurt = true;
            }
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (item.type == ItemID.SpectreHood)
                tooltips.ReplaceTooltipWith($"Armor.{SetID}.HoodTooltip");
            else if (item.type == ItemID.SpectreMask)
                tooltips.ReplaceTooltipWith($"Armor.{SetID}.MaskTooltip");
            else if (ChestsToApplyTo.Contains(item.type))
                tooltips.ReplaceTooltipWith($"Armor.{SetID}.ChestplateTooltip");
            else if (LegsToApplyTo.Contains(item.type))
                tooltips.ReplaceTooltipWith($"Armor.{SetID}.LeggingsTooltip");
        }
    }
}
