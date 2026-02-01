using RootsBeta.Utilities;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RootsBeta.Items.ArmorSets
{
    public class ShroomiteArmor : BaseArmorSet
    {
        public override string SetID => "Shroomite";
        public override List<int> HeadsToApplyTo => [ItemID.ShroomiteHeadgear, ItemID.ShroomiteMask,ItemID.ShroomiteHelmet];
        public override List<int> ChestsToApplyTo => [ItemID.ShroomiteBreastplate];
        public override List<int> LegsToApplyTo => [ItemID.ShroomiteLeggings];

        public override void HeadEquips(Item item, Player player)
        {
            if (item.type == ItemID.ShroomiteHeadgear) //Arrow
                player.arrowDamage *= 0.15f;
            if (item.type == ItemID.ShroomiteMask) //Bullet
                player.bulletDamage *= 0.15f;
            if (item.type == ItemID.ShroomiteHeadgear) //Specialist
                player.specialistDamage *= 0.15f;
            player.GetCritChance<GenericDamageClass>() += 5f;
        }

        public override void ChestEquips(Item item, Player player)
        {
            player.GetDamage<GenericDamageClass>() += 0.13f;
            player.GetCritChance<GenericDamageClass>() += 13f;
            player.ammoCost80 = true;
        }

        public override void LegsEquips(Item item, Player player)
        {
            player.GetCritChance<GenericDamageClass>() += 7f;
            player.moveSpeed += 0.12f;
        }

        public override void SetBonusEffect(Player player)
        {
            player.shroomiteStealth = true;
            player.GetDamage<GenericDamageClass>() += (1f - player.stealth) * 0.6f;
            player.GetCritChance<GenericDamageClass>() += (int)((1f - player.stealth) * 10f);
            player.GetKnockback<GenericDamageClass>() *= 1f + (1f - player.stealth) * 0.5f;

            //Cancel vanilla buffs
            player.GetDamage<RangedDamageClass>() -= (1f - player.stealth) * 0.6f;
            player.GetCritChance<RangedDamageClass>() -= (int)((1f - player.stealth) * 10f);
            player.GetKnockback<RangedDamageClass>() /= 1f + (1f - player.stealth) * 0.5f;
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (item.type == ItemID.ShroomiteHeadgear)
                tooltips.ReplaceTooltipWith($"Armor.{SetID}.HeadgearTooltip");
            else if (item.type == ItemID.ShroomiteMask)
                tooltips.ReplaceTooltipWith($"Armor.{SetID}.MaskTooltip");
            else if (item.type == ItemID.ShroomiteHelmet)
                tooltips.ReplaceTooltipWith($"Armor.{SetID}.HelmetTooltip");
            else if (ChestsToApplyTo.Contains(item.type))
                tooltips.ReplaceTooltipWith($"Armor.{SetID}.ChestplateTooltip");
            else if (LegsToApplyTo.Contains(item.type))
                tooltips.ReplaceTooltipWith($"Armor.{SetID}.LeggingsTooltip");
        }
    }
}
