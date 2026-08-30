using RootsBeta.Utilities;
using System.Collections.Generic;
using RootsBeta.Players;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RootsBeta.Items.ArmorSets
{
    public class HuntressArmor : BaseArmorSet
    {
        #region Parameters
        public static int SentrySlotsHead => 1;
        public static int SentrySlotsLegs => 1;
        public static int CritChanceBonusHead => 10;
        public static float SummonOrLongRangeDamageBonusChest => 0.2f;
        public static float MoveSpeedBonusLegs => 0.2f;
        #endregion

        public override string SetID => "Huntress";
        public override List<int> HeadsToApplyTo => [ItemID.HuntressWig];
        public override List<int> ChestsToApplyTo => [ItemID.HuntressJerkin];
        public override List<int> LegsToApplyTo => [ItemID.HuntressPants];

        public override void HeadEquips(Item item, Player player)
        {
            player.maxTurrets += SentrySlotsHead;
            player.GetCritChance<GenericDamageClass>() += CritChanceBonusHead;
        }

        public override void ChestEquips(Item item, Player player)
        {
            player.Roots().ModifyHitNPCWithProjectileFuncs.Add((plr, proj, npc, modifiers) =>
            {
                if (proj.IsMinionOrSentryRelated || (plr.Distance(npc.Center) > RootsPlayer.CloseRangeDistance))
                    plr.Roots().AdditiveDamageMultipliersToApplyOnHit += SummonOrLongRangeDamageBonusChest;
                return modifiers;
            });
        }

        public override void LegsEquips(Item item, Player player)
        {
            player.moveSpeed += MoveSpeedBonusLegs;
            player.huntressAmmoCost90 = true;
        }

        public override void SetBonusEffect(Player player)
        {
            player.maxTurrets += SentrySlotsLegs;
            player.setHuntressT2 = true;
        }
    }
}
