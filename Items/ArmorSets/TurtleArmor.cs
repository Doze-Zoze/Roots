using RootsBeta.Utilities;
using System.Collections.Generic;
using RootsBeta.Players;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RootsBeta.Items.ArmorSets
{
    public class TurtleArmor : BaseArmorSet
    {
        #region Parameters
        public static float CloseRangedDamageBonusHead => 0.06f;
        public static float CloseRangedDamageBonusChest => 0.08f;
        public static int CritChanceBonusChest => 8;
        public static int CritChanceBonusLegs => 4;
        public static float DamageReduction => 0.15f;
        #endregion

        public override string SetID => "Turtle";
        public override List<int> HeadsToApplyTo => [ItemID.TurtleHelmet];
        public override List<int> ChestsToApplyTo => [ItemID.TurtleScaleMail];
        public override List<int> LegsToApplyTo => [ItemID.TurtleLeggings];

        private static void AdditiveDamageAtCloseRange(Player player, float amount)
        {
            player.Roots().ModifyHitNPCFuncs.Add((plr, npc, modifiers) => {
                if (plr.Distance(npc.Center) <= RootsPlayer.CloseRangeDistance)
                    plr.Roots().AdditiveDamageMultipliersToApplyOnHit += amount;
                return modifiers;
            });
        }

        public override void HeadEquips(Item item, Player player)
        {
            AdditiveDamageAtCloseRange(player, CloseRangedDamageBonusHead);
        }

        public override void ChestEquips(Item item, Player player)
        {
            AdditiveDamageAtCloseRange(player, CloseRangedDamageBonusChest);
            player.GetCritChance<GenericDamageClass>() += CritChanceBonusChest;
        }

        public override void LegsEquips(Item item, Player player)
        {
            player.GetCritChance<GenericDamageClass>() += CritChanceBonusLegs;
        }

        public override void SetBonusEffect(Player player)
        {
            player.turtleArmor = true;
            player.turtleThorns = true;
            player.endurance += DamageReduction;
        }
    }
}
