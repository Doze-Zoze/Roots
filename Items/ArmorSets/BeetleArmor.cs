using RootsBeta.Utilities;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RootsBeta.Items.ArmorSets
{
    public class BeetlerArmor : BaseArmorSet
    {
        public override string SetID => "Beetle";
        public override List<int> HeadsToApplyTo => [ItemID.BeetleHelmet];
        public override List<int> ChestsToApplyTo => [ItemID.BeetleShell, ItemID.BeetleScaleMail];
        public override List<int> LegsToApplyTo => [ItemID.BeetleLeggings];

        public override void HeadEquips(Item item, Player player)
        {
            player.GetDamage<GenericDamageClass>() += 0.05f;
            player.aggro += 250;
        }

        public override void ChestEquips(Item item, Player player)
        {
            if (item.type == ItemID.BeetleScaleMail)
            {

                player.GetDamage<GenericDamageClass>() += 0.08f;
                player.GetCritChance<GenericDamageClass>() += 8f;
                player.moveSpeed += 0.06f;
                //6% melee speed
            } else
            {

                player.GetDamage<GenericDamageClass>() += 0.05f;
                player.GetCritChance<GenericDamageClass>() += 5f;
                player.aggro += 400;
            }
        }

        public override void LegsEquips(Item item, Player player)
        {
            //6% melee speed
            player.moveSpeed += 0.06f;
        }


        public override string IsArmorSet(Item head, Item body, Item legs)
        {
            if ((ChestsToApplyTo.Count == 0 || ChestsToApplyTo.Contains(head.type)) && (body.type == ItemID.BeetleScaleMail) && (LegsToApplyTo.Count == 0 || LegsToApplyTo.Contains(legs.type)))
                return SetID + "SetScaleMail";
            if ((ChestsToApplyTo.Count == 0 || ChestsToApplyTo.Contains(head.type)) && (body.type == ItemID.BeetleShell) && (LegsToApplyTo.Count == 0 || LegsToApplyTo.Contains(legs.type)))
                return SetID + "SetShell";
            return string.Empty;
        }

        public override void UpdateArmorSet(Player player, string set)
        {
            if (set == SetID + "SetScaleMail")
            {
                player.setBonus = RootsUtils.GetLocalizedTextValue($"Armor.{SetID}.SetBonusScaleMail");
                player.beetleOffense = true;
                player.Roots().OnHitNPCWithProjectileFuncs.Add((player, projectile, npc, hitinfo, dmg) =>
                {
                    if (player.beetleOffense)
                    {
                        if (player.beetleOrbs == 0)
                        {
                            player.beetleCounter += dmg * 3;
                        }
                        else if (player.beetleOrbs == 1)
                        {
                            player.beetleCounter += dmg * 2;
                        }
                        else
                        {
                            player.beetleCounter += dmg;
                        }
                        player.beetleCountdown = 0;
                    }
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
