using Roots.Utilities;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Roots.Items.ArmorSets
{
    public class TikiArmor : BaseArmorSet
    {
        public override string SetID => "Tiki";
        public override List<int> HeadsToApplyTo => [ItemID.TikiMask];
        public override List<int> ChestsToApplyTo => [ItemID.TikiShirt];
        public override List<int> LegsToApplyTo => [ItemID.TikiPants];

        public override void HeadEquips(Item item, Player player)
        {
            player.GetDamage<GenericDamageClass>() += 0.10f;
            player.slotsMinions++;
            player.whipRangeMultiplier += 0.1f;
        }

        public override void ChestEquips(Item item, Player player)
        {
            player.GetDamage<GenericDamageClass>() += 0.10f;
            player.slotsMinions++;
        }

        public override void LegsEquips(Item item, Player player)
        {
            player.GetDamage<GenericDamageClass>() += 0.10f;
            player.slotsMinions++;
        }
        public override void SetBonusEffect(Player player)
        {
            player.slotsMinions++;
            player.whipRangeMultiplier += 0.2f;
        }
    }
}
