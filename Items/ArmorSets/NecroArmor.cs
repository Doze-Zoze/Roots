using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Roots.Items.ArmorSets
{
    public class NecroArmor : BaseArmorSet
    {
        public override string SetID => "Necro";
        public override List<int> HeadsToApplyTo => [ItemID.NecroHelmet, ItemID.AncientNecroHelmet];
        public override List<int> ChestsToApplyTo => [ItemID.NecroBreastplate];
        public override List<int> LegsToApplyTo => [ItemID.NecroGreaves];

        public override void HeadEquips(Item item, Player player)
        {
            player.GetDamage<GenericDamageClass>() += 0.05f;
        }

        public override void ChestEquips(Item item, Player player)
        {
            player.GetDamage<GenericDamageClass>() += 0.05f;
        }

        public override void LegsEquips(Item item, Player player)
        {
            player.GetDamage<GenericDamageClass>() += 0.05f;
        }

        public override void SetBonusEffect(Player player)
        {
            player.ammoCost80 = true;
            player.GetCritChance<GenericDamageClass>() += 10;
        }
    }
}
