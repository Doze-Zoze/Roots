using Roots.Config;
using RootsBeta.Utilities;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RootsBeta.Items.ArmorSets
{
    public class ShroomiteArmor : BaseArmorSet<ShroomiteArmor>, IConfigurableContent<ShroomiteArmor>
    {
        public static ConfigGroup ConfigGroups => ConfigGroup.ArmorReworks;

        #region Parameters
        public static float DamageMultiplierHead => 1.15f;
        public static float DamageBonusChest => 0.13f;
        public static int CritChanceBonusHead => 5;
        public static int CritChanceBonusChest => 13;
        public static int CritChanceBonusLegs => 7;
        public static float MoveSpeedBonusLegs => 0.12f;
        public static float StealthDamageMax => 0.6f;
        public static float StealthCritMax => 10f;
        public static float StealthKnockbackMax => 0.5f;
        #endregion

        private const float StealthDamageMaxVanilla = 0.6f;
        private const float StealthCritMaxVanilla = 10f;
        private const float StealthKnockbackMaxVanilla = 0.5f;

        public override string SetID => "Shroomite";
        public override List<int> HeadsToApplyTo => [ItemID.ShroomiteHeadgear, ItemID.ShroomiteMask,ItemID.ShroomiteHelmet];
        public override List<int> ChestsToApplyTo => [ItemID.ShroomiteBreastplate];
        public override List<int> LegsToApplyTo => [ItemID.ShroomiteLeggings];

        public override void HeadEquips(Item item, Player player)
        {
            switch (item.type)
            {
                //Arrow
                case ItemID.ShroomiteHeadgear:
                    player.arrowDamage *= DamageMultiplierHead;
                    break;
                //Bullet
                case ItemID.ShroomiteMask:
                    player.bulletDamage *= DamageMultiplierHead;
                    break;
                //Specialist
                case ItemID.ShroomiteHelmet:
                    player.specialistDamage *= DamageMultiplierHead;
                    break;
            }
            player.GetCritChance<GenericDamageClass>() += CritChanceBonusHead;
        }

        public override void ChestEquips(Item item, Player player)
        {
            player.GetDamage<GenericDamageClass>() += DamageBonusChest;
            player.GetCritChance<GenericDamageClass>() += CritChanceBonusChest;
            player.ammoCost80 = true;
        }

        public override void LegsEquips(Item item, Player player)
        {
            player.GetCritChance<GenericDamageClass>() += CritChanceBonusLegs;
            player.moveSpeed += MoveSpeedBonusLegs;
        }

        public override void SetBonusEffect(Player player)
        {
            player.shroomiteStealth = true;
            player.GetDamage<GenericDamageClass>() += (1f - player.stealth) * StealthDamageMax;
            player.GetCritChance<GenericDamageClass>() += (int)((1f - player.stealth) * StealthCritMax);
            player.GetKnockback<GenericDamageClass>() *= 1f + (1f - player.stealth) * StealthKnockbackMax;

            //Cancel vanilla buffs
            player.GetDamage<RangedDamageClass>() -= (1f - player.stealth) * StealthDamageMaxVanilla;
            player.GetCritChance<RangedDamageClass>() -= (int)((1f - player.stealth) * StealthCritMaxVanilla);
            player.GetKnockback<RangedDamageClass>() /= 1f + (1f - player.stealth) * StealthKnockbackMaxVanilla;
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
