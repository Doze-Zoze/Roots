using RootsBeta.Utilities;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RootsBeta.Items.ArmorSets
{
    public class DarkArtistArmor : BaseArmorSet
    {
        #region Parameters
        public static float DamageBonusHead => 0.15f;
        public static float SummonDamageBonusChest => 0.25f;
        public static float ManaDamageBonusChest => 0.1f;
        public static float SummonDamageBonusLegs => 0.2f;
        public static int SentrySlotsHead => 2;
        public static int SentrySlotsSet => 1;
        public static int CritChanceBonus => 25;
        public static float MoveSpeedBonus => 0.2f;
        public static float ManaCostReduction => 0.1f;
        #endregion

        public override string SetID => "DarkArtist";
        public override List<int> HeadsToApplyTo => [ItemID.ApprenticeAltHead];
        public override List<int> ChestsToApplyTo => [ItemID.ApprenticeAltShirt];
        public override List<int> LegsToApplyTo => [ItemID.ApprenticeAltPants];

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
                    plr.Roots().AdditiveDamageMultipliersToApplyOnHit += ManaDamageBonusChest;
                return modifiers;
            });
        }

        public override void LegsEquips(Item item, Player player)
        {
            player.Roots().ModifyHitNPCWithProjectileFuncs.Add((plr, proj, _, modifiers) =>
            {
                if (proj.IsMinionOrSentryRelated)
                    plr.Roots().AdditiveDamageMultipliersToApplyOnHit += SummonDamageBonusLegs;
                if (proj.Roots().IsManaProjectile)
                    proj.CritChance += CritChanceBonus;
                return modifiers;
            });
            player.moveSpeed += MoveSpeedBonus;
            player.manaCost -= ManaCostReduction;
        }

        public override void SetBonusEffect(Player player)
        {
            player.maxTurrets += SentrySlotsSet;
            player.setApprenticeT2 = true;
            player.setApprenticeT3 = true;
        }
    }
}
