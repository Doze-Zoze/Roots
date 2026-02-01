using RootsBeta.Utilities;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RootsBeta.Items.ArmorSets
{
    public class BeeArmor : BaseArmorSet
    {
        public override string SetID => "Bee";
        public override List<int> HeadsToApplyTo => [ItemID.BeeHeadgear];
        public override List<int> ChestsToApplyTo => [ItemID.BeeBreastplate];
        public override List<int> LegsToApplyTo => [ItemID.BeePants];

        public override void HeadEquips(Item item, Player player)
        {
            player.GetDamage<GenericDamageClass>() += 0.04f;
            player.slotsMinions++;
        }

        public override void ChestEquips(Item item, Player player)
        {
            player.GetDamage<GenericDamageClass>() += 0.04f;
            player.slotsMinions++;
        }

        public override void LegsEquips(Item item, Player player)
        {
            player.GetDamage<GenericDamageClass>() += 0.05f;
            
        }
        public override void SetBonusEffect(Player player)
        {
            player.Roots().ModifyHitNPCWithProjectileFuncs.Add((player, proj, npc, mod) =>
            {
                if (proj.IsMinionOrSentryRelated)
                    player.Roots().AdditiveDamageMultipliersToApplyOnHit += 0.1f;
                return mod;
            });
        }
    }
}
