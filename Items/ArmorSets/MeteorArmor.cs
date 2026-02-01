using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RootsBeta.Items.ArmorSets
{
    public class MeteorArmor : BaseArmorSet
    {
        public override string SetID => "Meteor";
        public override List<int> HeadsToApplyTo => [ItemID.MeteorHelmet];
        public override List<int> ChestsToApplyTo => [ItemID.MeteorSuit];
        public override List<int> LegsToApplyTo => [ItemID.MeteorLeggings];

        public override void HeadEquips(Item item, Player player)
        {
            player.GetDamage<GenericDamageClass>() += 0.09f;
        }

        public override void ChestEquips(Item item, Player player)
        {
            player.GetDamage<GenericDamageClass>() += 0.09f;
        }

        public override void LegsEquips(Item item, Player player)
        {
            player.GetDamage<GenericDamageClass>() += 0.09f;
        }

        public override void SetBonusEffect(Player player)
        {
            player.spaceGun = true;
        }
    }
}
