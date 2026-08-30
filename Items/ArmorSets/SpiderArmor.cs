using RootsBeta.Utilities;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RootsBeta.Items.ArmorSets
{
    public class SpiderArmor : BaseArmorSet
    {
        #region Parameters
        public static float DamageBonusHead => 0.05f;
        public static float DamageBonusChest => 0.05f;
        public static float DamageBonusLegs => 0.05f;
        public static int MinionSlotsHead => 1;
        public static int MinionSlotsChest => 1;
        public static int MinionSlotsLegs => 1;
        public static float SummonDamageBonusSet => 0.12f;
        #endregion

        public override string SetID => "Spider";
        public override List<int> HeadsToApplyTo => [ItemID.SpiderMask];
        public override List<int> ChestsToApplyTo => [ItemID.SpiderBreastplate];
        public override List<int> LegsToApplyTo => [ItemID.SpiderGreaves];

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
