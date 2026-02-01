using RootsBeta.Utilities;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RootsBeta.Items.ArmorSets
{
    public class ApprenticeArmor : BaseArmorSet
    {
        public override string SetID => "Apprentice";
        public override List<int> HeadsToApplyTo => [ItemID.ApprenticeHat];
        public override List<int> ChestsToApplyTo => [ItemID.ApprenticeRobe];
        public override List<int> LegsToApplyTo => [ItemID.ApprenticeTrousers];

        public override void HeadEquips(Item item, Player player)
        {
            player.maxTurrets++;
            player.GetDamage<GenericDamageClass>() += 0.1f;
        }

        public override void ChestEquips(Item item, Player player)
        {

            player.Roots().ModifyHitNPCWithProjectileFuncs.Add((player, proj, npc, mod) =>
            {
                if (proj.IsMinionOrSentryRelated)
                    player.Roots().AdditiveDamageMultipliersToApplyOnHit += 0.2f;
                if (proj.Roots().isManaProjectile)
                    proj.CritChance += 20;
                return mod;
            });
        }

        public override void LegsEquips(Item item, Player player)
        {
            player.moveSpeed += 0.2f;
            player.manaCost -= 0.1f;
        }
        public override void SetBonusEffect(Player player)
        {
            player.maxTurrets++;
            player.setApprenticeT2 = true;
        }
    }
}
