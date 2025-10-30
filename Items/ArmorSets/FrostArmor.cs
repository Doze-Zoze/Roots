using Roots.Utilities;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Roots.Items.ArmorSets
{
    public class FrostArmor : BaseArmorSet
    {
        public override string SetID => "Frost";
        public override List<int> HeadsToApplyTo => [ItemID.FrostHelmet];
        public override List<int> ChestsToApplyTo => [ItemID.FrostBreastplate];
        public override List<int> LegsToApplyTo => [ItemID.FrostLeggings];

        public override void HeadEquips(Item item, Player player)
        {
            player.GetCritChance<GenericDamageClass>() += 0.16f;
        }

        public override void ChestEquips(Item item, Player player)
        {
            player.GetCritChance<GenericDamageClass>() += 11;
        }

        public override void LegsEquips(Item item, Player player)
        {
            player.moveSpeed += 0.08f;
            player.Roots().shootSpeedMult *= 1.1f;
        }

        public override void SetBonusEffect(Player player)
        {
            player.frostArmor = true;
            player.Roots().PhysicalModifyHitNPCFuncs.Add((player, target, modifier) =>
            {
                player.Roots().AdditiveDamageMultipliersToApplyOnHit += 0.15f;
                target.AddBuff(BuffID.Frostburn2, 60);
                return modifier;
            });
        }
    }
}
