using Roots.Config;
using RootsBeta.Utilities;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RootsBeta.Items.ArmorSets
{
    public class BeeArmor : BaseArmorSet<BeeArmor>, IConfigurableContent<BeeArmor>
    {
        public static ConfigGroup ConfigGroups => ConfigGroup.ArmorReworks;

        #region Parameters
        public static float DamageBonusHead => 0.04f;
        public static float DamageBonusChest => 0.04f;
        public static float DamageBonusLegs => 0.05f;
        public static int MinionSlotsHead => 1;
        public static int MinionSlotsChest => 1;
        public static float SummonDamageBonusSet => 0.1f;
        #endregion

        public override string SetID => "Bee";
        public override List<int> HeadsToApplyTo => [ItemID.BeeHeadgear];
        public override List<int> ChestsToApplyTo => [ItemID.BeeBreastplate];
        public override List<int> LegsToApplyTo => [ItemID.BeeGreaves];

        public override void HeadEquips(Item item, Player player)
        {
            player.GetDamage<GenericDamageClass>() += DamageBonusHead;
            player.maxMinions += MinionSlotsHead;
        }

        public override void ChestEquips(Item item, Player player)
        {
            player.GetDamage<GenericDamageClass>() += DamageBonusChest;
            player.maxMinions += MinionSlotsChest;
        }

        public override void LegsEquips(Item item, Player player)
        {
            player.GetDamage<GenericDamageClass>() += DamageBonusLegs;
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
