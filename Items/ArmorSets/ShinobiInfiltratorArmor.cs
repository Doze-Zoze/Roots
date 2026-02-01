using RootsBeta.Utilities;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RootsBeta.Items.ArmorSets
{
    public class ShinobiInfiltratorArmor : BaseArmorSet
    {
        public override string SetID => "ShinobiInfiltrator";
        public override List<int> HeadsToApplyTo => [ItemID.MonkAltHead];
        public override List<int> ChestsToApplyTo => [ItemID.MonkAltShirt];
        public override List<int> LegsToApplyTo => [ItemID.MonkAltPants];

        public override void HeadEquips(Item item, Player player)
        {
            player.maxTurrets+=2;
            player.GetDamage<GenericDamageClass>() += 0.2f;
        }

        public override void ChestEquips(Item item, Player player)
        {

            player.Roots().ModifyHitNPCWithProjectileFuncs.Add((player, proj, npc, mod) =>
            {
                if (proj.IsMinionOrSentryRelated)
                    player.Roots().AdditiveDamageMultipliersToApplyOnHit += 0.2f;
                return mod;
            });
            player.GetAttackSpeed<GenericDamageClass>() += 0.2f;
            player.GetCritChance<GenericDamageClass>() += 5f;
        }

        public override void LegsEquips(Item item, Player player)
        {
            player.Roots().ModifyHitNPCWithProjectileFuncs.Add((player, proj, npc, mod) =>
            {
                if (proj.IsMinionOrSentryRelated)
                    player.Roots().AdditiveDamageMultipliersToApplyOnHit += 0.2f;
                return mod;
            });

            player.GetCritChance<GenericDamageClass>() += 20f;
            player.moveSpeed += 0.3f;
        }
        public override void SetBonusEffect(Player player)
        {
            player.maxTurrets++;
            player.setMonkT2 = true;
            player.setMonkT3 = true;
        }
    }
}
