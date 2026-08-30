using RootsBeta.Utilities;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RootsBeta.Items.ArmorSets
{
    public class ApprenticeArmor : BaseArmorSet
    {
        #region Parameters
        public static float DamageBonusHead => 0.1f;
        public static int SentrySlotsHead => 1;
        public static float SummonDamageBonusChest => 0.2f;
        public static int ManaCritChanceBonusChest => 20;
        public static float MoveSpeedBonusLegs => 0.2f;
        public static float ManaCostReductionLegs => 0.1f;
        public static int SentrySlotsSet => 1;
        #endregion

        public override string SetID => "Apprentice";
        public override List<int> HeadsToApplyTo => [ItemID.ApprenticeHat];
        public override List<int> ChestsToApplyTo => [ItemID.ApprenticeRobe];
        public override List<int> LegsToApplyTo => [ItemID.ApprenticeTrousers];

        public override void HeadEquips(Item item, Player player)
        {
            player.maxTurrets += SentrySlotsHead;
            player.GetDamage<GenericDamageClass>() += DamageBonusHead;
        }

        public override void ChestEquips(Item item, Player player)
        {
            player.Roots().ModifyHitNPCWithProjectileFuncs.Add((plr, proj, _, modifiers) =>
            {
                if (proj.IsMinionOrSentryRelated)
                    plr.Roots().AdditiveDamageMultipliersToApplyOnHit += SummonDamageBonusChest;
                if (proj.Roots().IsManaProjectile)
                    proj.CritChance += ManaCritChanceBonusChest;
                return modifiers;
            });
        }

        public override void LegsEquips(Item item, Player player)
        {
            player.moveSpeed += MoveSpeedBonusLegs;
            player.manaCost -= ManaCostReductionLegs;
        }

        public override void SetBonusEffect(Player player)
        {
            player.maxTurrets += SentrySlotsSet;
            player.setApprenticeT2 = true;
        }
    }
}
