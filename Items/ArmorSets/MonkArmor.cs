using Roots.Config;
using RootsBeta.Utilities;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RootsBeta.Items.ArmorSets
{
    public class MonkArmor : BaseArmorSet<MonkArmor>, IConfigurableContent<MonkArmor>
    {
        public static ConfigGroup ConfigGroups => ConfigGroup.ArmorReworks;

        #region Parameters
        public static int SentrySlotsHead => 1;
        public static int SentrySlotsSet => 1;
        public static float AttackSpeedBonusHead => 0.2f;
        public static float SummonDamageBonusChest => 0.2f;
        public static float CritChanceBonusChest => 0.15f;
        public static float MoveSpeedBonusLegs => 0.2f;
        public static float DamageBonusLegs => 0.1f;
        #endregion

        public override string SetID => "Monk";
        public override List<int> HeadsToApplyTo => [ItemID.MonkBrows];
        public override List<int> ChestsToApplyTo => [ItemID.MonkShirt];
        public override List<int> LegsToApplyTo => [ItemID.MonkPants];

        public override void HeadEquips(Item item, Player player)
        {
            player.maxTurrets += SentrySlotsHead;
            player.GetAttackSpeed<GenericDamageClass>() += AttackSpeedBonusHead;
        }

        public override void ChestEquips(Item item, Player player)
        {
            player.Roots().ModifyHitNPCWithProjectileFuncs.Add((plr, proj, _, modifiers) =>
            {
                if (proj.IsMinionOrSentryRelated)
                    plr.Roots().AdditiveDamageMultipliersToApplyOnHit += SummonDamageBonusChest;
                return modifiers;
            });
            player.GetCritChance<GenericDamageClass>() += CritChanceBonusChest;
        }

        public override void LegsEquips(Item item, Player player)
        {
            player.moveSpeed += MoveSpeedBonusLegs;
            player.GetDamage<GenericDamageClass>() += DamageBonusLegs;
        }

        public override void SetBonusEffect(Player player)
        {
            player.maxTurrets += SentrySlotsSet;
            player.setMonkT2 = true;
        }
    }
}
