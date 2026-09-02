using Roots.Config;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RootsBeta.Items.ArmorSets
{
    public class MeteorArmor : BaseArmorSet<MeteorArmor>, IConfigurableContent<MeteorArmor>
    {
        public static ConfigGroup ConfigGroups => ConfigGroup.ArmorReworks;

        #region Parameters
        public static float DamageBonusHead => 0.09f;
        public static float DamageBonusChest => 0.09f;
        public static float DamageBonusLegs => 0.09f;
        #endregion

        public override string SetID => "Meteor";
        public override List<int> HeadsToApplyTo => [ItemID.MeteorHelmet];
        public override List<int> ChestsToApplyTo => [ItemID.MeteorSuit];
        public override List<int> LegsToApplyTo => [ItemID.MeteorLeggings];

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
            player.spaceGun = true;
        }
    }
}
