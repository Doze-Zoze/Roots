using RootsBeta.Utilities;
using System.Collections.Generic;
using RootsBeta.Players;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RootsBeta.Items.ArmorSets
{
    public class RedRidingArmor : BaseArmorSet
    {
        #region Parameters
        public static int SentrySlotsHead => 2;
        public static int SentrySlotsSet => 1;
        public static float CritChanceBonusHead => 20;
        public static float SummonOrLongRangeDamageBonusChest => 0.25f;
        public static float MoveSpeedBonus => 0.2f;
        public static float SummonDamageBonusLegs => 0.35f;
        #endregion

        public override string SetID => "RedRiding";
        public override List<int> HeadsToApplyTo => [ItemID.HuntressAltHead];
        public override List<int> ChestsToApplyTo => [ItemID.HuntressAltShirt];
        public override List<int> LegsToApplyTo => [ItemID.HuntressAltPants];

        public override void HeadEquips(Item item, Player player)
        {
            player.maxTurrets += SentrySlotsHead;
            player.GetCritChance<GenericDamageClass>() += CritChanceBonusHead;
        }

        public override void ChestEquips(Item item, Player player)
        {
            player.Roots().ModifyHitNPCWithProjectileFuncs.Add((_, proj, npc, modifiers) =>
            {
                if (proj.IsMinionOrSentryRelated || (player.Distance(npc.Center) > RootsPlayer.CloseRangeDistance))
                    player.Roots().AdditiveDamageMultipliersToApplyOnHit += SummonOrLongRangeDamageBonusChest;
                return modifiers;
            });
        }

        public override void LegsEquips(Item item, Player player)
        {
            player.moveSpeed += MoveSpeedBonus;
            player.ammoCost80 = true;
            player.Roots().ModifyHitNPCWithProjectileFuncs.Add((_, proj, _, modifiers) =>
            {
                if (proj.IsMinionOrSentryRelated)
                    player.Roots().AdditiveDamageMultipliersToApplyOnHit += SummonDamageBonusLegs;
                return modifiers;
            });
        }

        public override void SetBonusEffect(Player player)
        {
            player.maxTurrets += SentrySlotsSet;
            player.setHuntressT2 = true;
            player.setHuntressT3 = true;
        }
    }
}
