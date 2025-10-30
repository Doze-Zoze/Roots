using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Roots.Items.ArmorSets
{
    public class MoltenArmor : BaseArmorSet
    {
        public override string SetID => "Molten";
        public override List<int> HeadsToApplyTo => [ItemID.MoltenHelmet];
        public override List<int> ChestsToApplyTo => [ItemID.MoltenBreastplate];
        public override List<int> LegsToApplyTo => [ItemID.MoltenGreaves];

        public override void HeadEquips(Item item, Player player)
        {
            player.GetCritChance<GenericDamageClass>() += 7f;
        }

        public override void ChestEquips(Item item, Player player)
        {
            player.GetDamage<GenericDamageClass>() += 0.05f;
        }

        public override void LegsEquips(Item item, Player player)
        {
            player.GetAttackSpeed<GenericDamageClass>() += 0.07f;
        }

        public override void SetBonusEffect(Player player)
        {
            player.buffImmune[BuffID.OnFire] = true;
            player.buffImmune[BuffID.Burning] = true;
            player.GetDamage<GenericDamageClass>() += 0.1f;
        }
    }
}
