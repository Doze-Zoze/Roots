using Roots.Utilities;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Roots.Items.ArmorSets
{
    public class RedRidingArmor : BaseArmorSet
    {
        public override string SetID => "RedRiding";
        public override List<int> HeadsToApplyTo => [ItemID.HuntressAltHead];
        public override List<int> ChestsToApplyTo => [ItemID.HuntressAltShirt];
        public override List<int> LegsToApplyTo => [ItemID.HuntressAltPants];

        public override void HeadEquips(Item item, Player player)
        {
            player.maxTurrets+=2;
            player.GetCritChance<GenericDamageClass>() += 20f;
        }

        public override void ChestEquips(Item item, Player player)
        {

            player.Roots().ModifyHitNPCWithProjectileFuncs.Add((player, proj, npc, mod) =>
            {
                if (proj.IsMinionOrSentryRelated || (player.Distance(npc.Center) > 16 * 25))
                    player.Roots().AdditiveDamageMultipliersToApplyOnHit += 0.25f;
                return mod;
            });
        }

        public override void LegsEquips(Item item, Player player)
        {
            player.moveSpeed += 0.2f;
            player.ammoCost80 = true;
            player.Roots().ModifyHitNPCWithProjectileFuncs.Add((player, proj, npc, mod) =>
            {
                if (proj.IsMinionOrSentryRelated)
                    player.Roots().AdditiveDamageMultipliersToApplyOnHit += 0.35f;
                return mod;
            });
        }
        public override void SetBonusEffect(Player player)
        {
            player.maxTurrets++;
            player.setHuntressT2 = true;
            player.setHuntressT3 = true;
        }
    }
}
