using Roots.Config;
using RootsBeta.Utilities;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RootsBeta.Items.ArmorSets
{
    public class ShinobiInfiltratorArmor : BaseArmorSet<ShinobiInfiltratorArmor>, IConfigurableContent<ShinobiInfiltratorArmor>
    {
        public static ConfigGroup ConfigGroups => ConfigGroup.ArmorReworks;

        #region Parameters
        public static int SentrySlotsHead => 2;
        public static int SentrySlotsSet => 1;
        public static float DamageBonusHead => 0.2f;
        public static float SummonDamageBonusChest => 0.2f;
        public static float SummonDamageBonusLegs => 0.2f;
        public static float AttackSpeedBonusChest => 0.2f;
        public static int CritChanceBonusChest => 5;
        public static int CritChanceBonusLegs => 20;
        public static float MoveSpeedBonusLegs => 0.3f;
        #endregion

        public override string SetID => "ShinobiInfiltrator";
        public override List<int> HeadsToApplyTo => [ItemID.MonkAltHead];
        public override List<int> ChestsToApplyTo => [ItemID.MonkAltShirt];
        public override List<int> LegsToApplyTo => [ItemID.MonkAltPants];

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
                return modifiers;
            });
            player.GetAttackSpeed<GenericDamageClass>() += AttackSpeedBonusChest;
            player.GetCritChance<GenericDamageClass>() += CritChanceBonusChest;
        }

        public override void LegsEquips(Item item, Player player)
        {
            player.Roots().ModifyHitNPCWithProjectileFuncs.Add((plr, proj, _, modifiers) =>
            {
                if (proj.IsMinionOrSentryRelated)
                    plr.Roots().AdditiveDamageMultipliersToApplyOnHit += SummonDamageBonusLegs;
                return modifiers;
            });
            player.GetCritChance<GenericDamageClass>() += CritChanceBonusLegs;
            player.moveSpeed += MoveSpeedBonusLegs;
        }

        public override void SetBonusEffect(Player player)
        {
            player.maxTurrets += SentrySlotsSet;
            player.setMonkT2 = true;
            player.setMonkT3 = true;
        }
    }
}
