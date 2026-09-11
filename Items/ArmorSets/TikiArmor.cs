using Roots.Config;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RootsBeta.Items.ArmorSets
{
    public class TikiArmor : BaseArmorSet<TikiArmor>, IConfigurableContent<TikiArmor>
    {
        public static ConfigGroup ConfigGroups => ConfigGroup.ArmorReworks;

        #region Parameters
        public static float DamageBonusHead => 0.1f;
        public static float DamageBonusChest => 0.1f;
        public static float DamageBonusLegs => 0.1f;
        public static int MinionSlotsHead => 1;
        public static int MinionSlotsChest => 1;
        public static int MinionSlotsLegs => 1;
        public static int MinionSlotsSet => 1;
        public static float WhipRangeBonusHead => 0.1f;
        public static float WhipRangeBonusSet => 0.2f;
        #endregion

        public override string SetID => "Tiki";
        public override List<int> HeadsToApplyTo => [ItemID.TikiMask];
        public override List<int> ChestsToApplyTo => [ItemID.TikiShirt];
        public override List<int> LegsToApplyTo => [ItemID.TikiPants];

        public override void HeadEquips(Item item, Player player)
        {
            player.GetDamage<GenericDamageClass>() += DamageBonusHead;
            player.maxMinions += MinionSlotsHead;
            player.whipRangeMultiplier += WhipRangeBonusHead;
        }

        public override void ChestEquips(Item item, Player player)
        {
            player.GetDamage<GenericDamageClass>() += DamageBonusChest;
            player.maxMinions += MinionSlotsChest;
        }

        public override void LegsEquips(Item item, Player player)
        {
            player.GetDamage<GenericDamageClass>() += DamageBonusLegs;
            player.maxMinions += MinionSlotsLegs;
        }

        public override void SetBonusEffect(Player player)
        {
            player.maxMinions += MinionSlotsSet;
            player.whipRangeMultiplier += WhipRangeBonusSet;
        }
    }
}
