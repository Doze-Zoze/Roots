using RootsBeta.Utilities;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RootsBeta.Items.ArmorSets
{
    public class SpectreArmor : BaseArmorSet
    {
        #region Parameters
        public static int ManaMaxBonusMask => 60;
        public static float ManaCostReductionMask => 0.13f;
        public static float DamageBonusMask => 0.1f;
        public static int CritChanceBonusMask => 10;
        public static float DamageBonusChest => 0.07f;
        public static int CritChanceBonusChest => 7;
        public static float DamageBonusLegs => 0.08f;
        public static float MoveSpeedBonusLegs => 0.08f;
        public static float ManaDamageReductionHood => 0.4f;
        #endregion

        public override string SetID => "Spectre";
        public override List<int> HeadsToApplyTo => [ItemID.SpectreHood, ItemID.SpectreMask];
        public override List<int> ChestsToApplyTo => [ItemID.SpectreRobe];
        public override List<int> LegsToApplyTo => [ItemID.SpectrePants];

        public override void HeadEquips(Item item, Player player)
        {
            if (item.type == ItemID.SpectreHood) return;
            player.statManaMax2 += ManaMaxBonusMask;
            player.manaCost -= ManaCostReductionMask;
            player.GetDamage<GenericDamageClass>() += DamageBonusMask;
            player.GetCritChance<GenericDamageClass>() += CritChanceBonusMask;
        }

        public override void ChestEquips(Item item, Player player)
        {
            player.GetDamage<GenericDamageClass>() += DamageBonusChest;
            player.GetCritChance<GenericDamageClass>() += CritChanceBonusChest;
        }

        public override void LegsEquips(Item item, Player player)
        {
            player.GetDamage<GenericDamageClass>() += DamageBonusLegs;
            player.moveSpeed += MoveSpeedBonusLegs;
        }

        public override string IsArmorSet(Item head, Item body, Item legs)
        {
            return head.type switch
            {
                ItemID.SpectreHood when (ChestsToApplyTo.Count == 0 || ChestsToApplyTo.Contains(body.type)) &&
                    (LegsToApplyTo.Count == 0 || LegsToApplyTo.Contains(legs.type)) 
                    => SetID + "SetHood",
                ItemID.SpectreMask when (ChestsToApplyTo.Count == 0 || ChestsToApplyTo.Contains(body.type)) &&
                    (LegsToApplyTo.Count == 0 || LegsToApplyTo.Contains(legs.type)) 
                    => SetID + "SetMask",
                _ => string.Empty
            };
        }

        public override void UpdateArmorSet(Player player, string set)
        {
            if (set == SetID + "SetHood")
            {
                player.setBonus = RootsUtils.GetLocalizedTextValue($"Armor.{SetID}.SetBonusHood");
                player.Roots().AdditiveManaDamage -= ManaDamageReductionHood;
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
