using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RootsBeta.Items.ArmorSets
{
    public class NecroArmor : BaseArmorSet
    {
        #region Parameters
        public static float DamageBonusHead => 0.05f;
        public static float DamageBonusChest => 0.05f;
        public static float DamageBonusLegs => 0.05f;
        public static int CritChanceBonusSet => 10;
        #endregion

        public override string SetID => "Necro";
        public override List<int> HeadsToApplyTo => [ItemID.NecroHelmet, ItemID.AncientNecroHelmet];
        public override List<int> ChestsToApplyTo => [ItemID.NecroBreastplate];
        public override List<int> LegsToApplyTo => [ItemID.NecroGreaves];

        public override void HeadEquips(Item item, Player player)
        {
            player.GetDamage<GenericDamageClass>() += DamageBonusHead;
        }

        public override void ChestEquips(Item item, Player player)
        {
            player.GetDamage<GenericDamageClass>() += DamageBonusChest;
        }

        public override void LegsEquips(Item item, Player player)
        {
            player.GetDamage<GenericDamageClass>() += DamageBonusLegs;
        }

        public override void SetBonusEffect(Player player)
        {
            player.ammoCost80 = true;
            player.GetCritChance<GenericDamageClass>() += CritChanceBonusSet;
        }
    }
}
