using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RootsBeta.Items.ArmorSets
{
    public class FossilArmor : BaseArmorSet
    {
        public override string SetID => "Fossil";
        public override List<int> HeadsToApplyTo => [ItemID.FossilHelm];
        public override List<int> ChestsToApplyTo => [ItemID.FossilShirt];
        public override List<int> LegsToApplyTo => [ItemID.FossilPants];

        public override void HeadEquips(Item item, Player player)
        {
            player.GetCritChance<GenericDamageClass>() += 4;
        }

        public override void ChestEquips(Item item, Player player)
        {
            player.GetDamage<GenericDamageClass>() += 0.05f;
        }

        public override void LegsEquips(Item item, Player player)
        {
            player.GetCritChance<GenericDamageClass>() += 4;
        }

        public override void SetBonusEffect(Player player)
        {
            player.ammoCost80 = true;
        }
    }
}
