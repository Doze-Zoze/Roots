using Roots.Config;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RootsBeta.Items.ArmorSets
{
    public class JungleArmor : BaseArmorSet<JungleArmor>, IConfigurableContent<JungleArmor>
    {
        public static ConfigGroup ConfigGroups => ConfigGroup.ArmorReworks;

        #region Parameters
        public static int ManaMaxBonusHead => 40;
        public static int ManaMaxBonusChest => 20;
        public static int ManaMaxBonusLegs => 20;
        public static int CritChanceBonusHead => 6;
        public static int CritChanceBonusLegs => 6;
        public static float DamageBonusChest => 0.06f;
        public static float ManaCostReductionLegs => 0.16f;
        #endregion

        public override string SetID => "Jungle";
        public override List<int> HeadsToApplyTo => [ItemID.JungleHat, ItemID.AncientCobaltHelmet];
        public override List<int> ChestsToApplyTo => [ItemID.JungleShirt, ItemID.AncientCobaltBreastplate];
        public override List<int> LegsToApplyTo => [ItemID.JunglePants, ItemID.AncientCobaltLeggings];

        public override void HeadEquips(Item item, Player player)
        {
            player.statManaMax2 += ManaMaxBonusHead;
            player.GetCritChance<GenericDamageClass>() += CritChanceBonusHead;
        }

        public override void ChestEquips(Item item, Player player)
        {
            player.statManaMax2 += ManaMaxBonusChest;
            player.GetDamage<GenericDamageClass>() += DamageBonusChest;
        }

        public override void LegsEquips(Item item, Player player)
        {
            player.statManaMax2 += ManaMaxBonusLegs;
            player.GetCritChance<GenericDamageClass>() += CritChanceBonusLegs;
        }

        public override void SetBonusEffect(Player player)
        {
            player.manaCost -= ManaCostReductionLegs;
        }
    }
}
