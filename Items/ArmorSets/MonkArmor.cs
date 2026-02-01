using RootsBeta.Utilities;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RootsBeta.Items.ArmorSets
{
    public class MonkArmor : BaseArmorSet
    {
        public override string SetID => "Monk";
        public override List<int> HeadsToApplyTo => [ItemID.MonkBrows];
        public override List<int> ChestsToApplyTo => [ItemID.MonkShirt];
        public override List<int> LegsToApplyTo => [ItemID.MonkPants];

        public override void HeadEquips(Item item, Player player)
        {
            player.maxTurrets++;
            player.GetAttackSpeed<GenericDamageClass>() += 0.2f;
        }

        public override void ChestEquips(Item item, Player player)
        {

            player.Roots().ModifyHitNPCWithProjectileFuncs.Add((player, proj, npc, mod) =>
            {
                if (proj.IsMinionOrSentryRelated)
                    player.Roots().AdditiveDamageMultipliersToApplyOnHit += 0.2f;
                return mod;
            });
            player.GetCritChance<GenericDamageClass>() += 0.15f;
        }

        public override void LegsEquips(Item item, Player player)
        {
            player.moveSpeed += 0.2f;
            player.GetDamage<GenericDamageClass>() += 0.1f;
        }
        public override void SetBonusEffect(Player player)
        {
            player.maxTurrets++;
            player.setMonkT2 = true;
        }
    }
}
