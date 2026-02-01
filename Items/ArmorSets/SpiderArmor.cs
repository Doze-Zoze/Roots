using RootsBeta.Utilities;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RootsBeta.Items.ArmorSets
{
    public class SpiderArmor : BaseArmorSet
    {
        public override string SetID => "Spider";
        public override List<int> HeadsToApplyTo => [ItemID.SpiderMask];
        public override List<int> ChestsToApplyTo => [ItemID.SpiderBreastplate];
        public override List<int> LegsToApplyTo => [ItemID.SpiderGreaves];

        public override void HeadEquips(Item item, Player player)
        {
            player.GetDamage<GenericDamageClass>() += 0.05f;
            player.slotsMinions++;
        }

        public override void ChestEquips(Item item, Player player)
        {
            player.GetDamage<GenericDamageClass>() += 0.05f;
            player.slotsMinions++;
        }

        public override void LegsEquips(Item item, Player player)
        {
            player.GetDamage<GenericDamageClass>() += 0.05f;
            player.slotsMinions++;

        }
        public override void SetBonusEffect(Player player)
        {

            player.Roots().ModifyHitNPCWithProjectileFuncs.Add((player, proj, npc, mod) =>
            {
                if (proj.IsMinionOrSentryRelated)
                    player.Roots().AdditiveDamageMultipliersToApplyOnHit += 0.12f;
                return mod;
            });
        }
    }
}
