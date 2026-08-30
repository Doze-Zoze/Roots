using RootsBeta.Utilities;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RootsBeta.Items.ArmorSets
{
    public class SpookyArmor : BaseArmorSet
    {
        #region Parameters
        public static float DamageBonusHead => 0.11f;
        public static float DamageBonusChest => 0.11f;
        public static float DamageBonusLegs => 0.11f;
        public static int MinionSlotsHead => 1;
        public static int MinionSlotsChest => 2;
        public static int MinionSlotsLegs => 1;
        public static float MoveSpeedBonusLegs => 0.2f;
        public static float SummonDamageBonusSet => 0.25f;
        #endregion

        public override string SetID => "Spooky";
        public override List<int> HeadsToApplyTo => [ItemID.SpookyHelmet];
        public override List<int> ChestsToApplyTo => [ItemID.SpookyBreastplate];
        public override List<int> LegsToApplyTo => [ItemID.SpookyLeggings];

        public override void HeadEquips(Item item, Player player)
        {
            player.GetDamage<GenericDamageClass>() += DamageBonusHead;
            player.slotsMinions += MinionSlotsHead;
        }

        public override void ChestEquips(Item item, Player player)
        {
            player.GetDamage<GenericDamageClass>() += DamageBonusChest;
            player.slotsMinions += MinionSlotsChest;
        }

        public override void LegsEquips(Item item, Player player)
        {
            player.GetDamage<GenericDamageClass>() += DamageBonusLegs;
            player.slotsMinions += MinionSlotsLegs;
            player.moveSpeed += MoveSpeedBonusLegs;
        }
        public override void SetBonusEffect(Player player)
        {
            player.Roots().ModifyHitNPCWithProjectileFuncs.Add((plr, proj, _, modifiers) =>
            {
                if (proj.IsMinionOrSentryRelated)
                    plr.Roots().AdditiveDamageMultipliersToApplyOnHit += SummonDamageBonusSet;
                return modifiers;
            });
        }
    }
}
