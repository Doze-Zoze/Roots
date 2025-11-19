using Roots.Utilities;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Roots.Items.ArmorSets
{
    public class SpookyArmor : BaseArmorSet
    {
        public override string SetID => "Spooky";
        public override List<int> HeadsToApplyTo => [ItemID.SpookyHelmet];
        public override List<int> ChestsToApplyTo => [ItemID.SpookyBreastplate];
        public override List<int> LegsToApplyTo => [ItemID.SpookyLeggings];

        public override void HeadEquips(Item item, Player player)
        {
            player.GetDamage<GenericDamageClass>() += 0.11f;
            player.slotsMinions++;
        }

        public override void ChestEquips(Item item, Player player)
        {
            player.GetDamage<GenericDamageClass>() += 0.11f;
            player.slotsMinions+=2;
        }

        public override void LegsEquips(Item item, Player player)
        {
            player.GetDamage<GenericDamageClass>() += 0.11f;
            player.slotsMinions++;
            player.moveSpeed += 0.2f;
        }
        public override void SetBonusEffect(Player player)
        {
            player.Roots().ModifyHitNPCWithProjectileFuncs.Add((player, proj, npc, mod) =>
            {
                if (proj.IsMinionOrSentryRelated)
                    player.Roots().AdditiveDamageMultipliersToApplyOnHit += 0.25f;
                return mod;
            });
        }
    }
}
