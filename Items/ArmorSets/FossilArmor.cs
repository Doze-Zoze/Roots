using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RootsBeta.Items.ArmorSets
{
    public class FossilArmor : BaseArmorSet
    {
        #region Parameters
        public static int CritChanceBonusHead => 4;
        public static int CritChanceBonusLegs => 4;
        public static float DamageBonusChest => 0.05f;
        #endregion

        public override string SetID => "Fossil";
        public override List<int> HeadsToApplyTo => [ItemID.FossilHelm];
        public override List<int> ChestsToApplyTo => [ItemID.FossilShirt];
        public override List<int> LegsToApplyTo => [ItemID.FossilPants];

        public override void HeadEquips(Item item, Player player)
        {
            player.GetCritChance<GenericDamageClass>() += CritChanceBonusHead;
        }

        public override void ChestEquips(Item item, Player player)
        {
            player.GetDamage<GenericDamageClass>() += DamageBonusChest;
        }

        public override void LegsEquips(Item item, Player player)
        {
            player.GetCritChance<GenericDamageClass>() += CritChanceBonusLegs;
        }

        public override void SetBonusEffect(Player player)
        {
            player.ammoCost80 = true;
        }
    }
}
