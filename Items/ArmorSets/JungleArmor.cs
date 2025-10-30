using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Roots.Items.ArmorSets
{
    public class JungleArmor : BaseArmorSet
    {
        public override string SetID => "Jungle";
        public override List<int> HeadsToApplyTo => [ItemID.JungleHat, ItemID.AncientCobaltHelmet];
        public override List<int> ChestsToApplyTo => [ItemID.JungleShirt, ItemID.AncientCobaltBreastplate];
        public override List<int> LegsToApplyTo => [ItemID.JunglePants, ItemID.AncientCobaltLeggings];

        public override void HeadEquips(Item item, Player player)
        {
            player.statManaMax2 += 40;
            player.GetCritChance<GenericDamageClass>() += 6;
        }

        public override void ChestEquips(Item item, Player player)
        {
            player.statManaMax2 += 20;
            player.GetDamage<GenericDamageClass>() += 0.06f;
        }

        public override void LegsEquips(Item item, Player player)
        {
            player.statManaMax2 += 20;
            player.GetCritChance<GenericDamageClass>() += 6;
        }

        public override void SetBonusEffect(Player player)
        {
            player.manaCost *= 0.84f;
        }
    }
}
