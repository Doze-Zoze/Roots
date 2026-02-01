using RootsBeta.Utilities;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RootsBeta.Items.ArmorSets
{
    public class HuntressArmor : BaseArmorSet
    {
        public override string SetID => "Huntress";
        public override List<int> HeadsToApplyTo => [ItemID.HuntressWig];
        public override List<int> ChestsToApplyTo => [ItemID.HuntressJerkin];
        public override List<int> LegsToApplyTo => [ItemID.HuntressPants];

        public override void HeadEquips(Item item, Player player)
        {
            player.maxTurrets++;
            player.GetCritChance<GenericDamageClass>() += 0.1f;
        }

        public override void ChestEquips(Item item, Player player)
        {

            player.Roots().ModifyHitNPCWithProjectileFuncs.Add((player, proj, npc, mod) =>
            {
                if (proj.IsMinionOrSentryRelated || (player.Distance(npc.Center) > 16 * 25))
                    player.Roots().AdditiveDamageMultipliersToApplyOnHit += 0.2f;
                return mod;
            });
        }

        public override void LegsEquips(Item item, Player player)
        {
            player.moveSpeed += 0.2f;
            player.huntressAmmoCost90 = true;
        }
        public override void SetBonusEffect(Player player)
        {
            player.maxTurrets++;
            player.setHuntressT2 = true;
        }
    }
}
