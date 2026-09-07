using Roots.Config;
using RootsBeta.Utilities;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RootsBeta.Items.ArmorSets
{
    public class BeetleArmor : BaseArmorSet<BeetleArmor>, IConfigurableContent<BeetleArmor>
    {
        public static ConfigGroup ConfigGroups => ConfigGroup.ArmorReworks;

        #region Parameters
        public static float DamageBonusHead => 0.05f;
        public static float DamageBonusScale => 0.08f;
        public static float DamageBonusShell => 0.05f;
        public static int CritChanceBonusScale => 8;
        public static int CritChanceBonusShell => 5;
        public static float MoveSpeedBonusScale => 0.06f;
        public static float MoveSpeedBonusLegs => 0.06f;
        public static int AggroBonusHead => 250;
        public static int AggroBonusShell  => 400;
        public static float BeetleMightDamageModifier => 0.1f;
        #endregion

        public override string SetID => "Beetle";
        public override List<int> HeadsToApplyTo => [ItemID.BeetleHelmet];
        public override List<int> ChestsToApplyTo => [ItemID.BeetleShell, ItemID.BeetleScaleMail];
        public override List<int> LegsToApplyTo => [ItemID.BeetleLeggings];

        public override void HeadEquips(Item item, Player player)
        {
            player.GetDamage<GenericDamageClass>() += DamageBonusHead;
            player.aggro += AggroBonusHead;
        }

        public override void ChestEquips(Item item, Player player)
        {
            if (item.type == ItemID.BeetleScaleMail)
            {
                player.GetDamage<GenericDamageClass>() += DamageBonusScale;
                player.GetCritChance<GenericDamageClass>() += CritChanceBonusScale;
                player.moveSpeed += MoveSpeedBonusScale;
                //6% melee speed
            } else
            {
                player.GetDamage<GenericDamageClass>() += DamageBonusShell;
                player.GetCritChance<GenericDamageClass>() += CritChanceBonusShell;
                player.aggro += AggroBonusShell;
            }
        }

        public override void LegsEquips(Item item, Player player)
        {
            //6% melee speed
            player.moveSpeed += MoveSpeedBonusLegs;
        }

        public override string IsArmorSet(Item head, Item body, Item legs)
        {
            if ((ChestsToApplyTo.Count == 0 || ChestsToApplyTo.Contains(body.type)) &&
                (LegsToApplyTo.Count == 0 || LegsToApplyTo.Contains(legs.type)) &&
                (HeadsToApplyTo.Count == 0 || HeadsToApplyTo.Contains(head.type)))
            {
                return SetID + (body.type == ItemID.BeetleScaleMail ? "SetScaleMail" : "SetShell");
            }
            return string.Empty;
        }

        public override void UpdateArmorSet(Player player, string set)
        {
            if (set == SetID + "SetScaleMail")
            {
                player.setBonus = RootsUtils.GetLocalizedTextValue($"Armor.{SetID}.SetBonusScaleMail");
                player.beetleOffense = true;
                player.Roots().OnHitNPCWithProjectileFuncs.Add((plr, _, _, _, dmg) =>
                {
                    if (!plr.beetleOffense) return;
                    plr.beetleCounter += plr.beetleOrbs switch
                    {
                        0 => dmg * 3,
                        1 => dmg * 2,
                        _ => dmg
                    };
                    plr.beetleCountdown = 0;
                });
            }
            else if (set == SetID + "SetShell")
            {
                player.setBonus = RootsUtils.GetLocalizedTextValue($"Armor.{SetID}.SetBonusShell");
                player.beetleDefense = true;
            }
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (HeadsToApplyTo.Contains(item.type))
                tooltips.ReplaceTooltipWith($"Armor.{SetID}.HelmetTooltip");
            else if (item.type == ItemID.BeetleScaleMail)
                tooltips.ReplaceTooltipWith($"Armor.{SetID}.ScaleMailTooltip");
            else if (item.type == ItemID.BeetleShell)
                tooltips.ReplaceTooltipWith($"Armor.{SetID}.ShellTooltip");
            else if (LegsToApplyTo.Contains(item.type))
                tooltips.ReplaceTooltipWith($"Armor.{SetID}.LeggingsTooltip");
        }
    }
}
