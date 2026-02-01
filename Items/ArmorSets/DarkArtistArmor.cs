using RootsBeta.Utilities;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RootsBeta.Items.ArmorSets
{
    public class DarkArtistArmor : BaseArmorSet
    {
        public override string SetID => "DarkArtist";
        public override List<int> HeadsToApplyTo => [ItemID.ApprenticeAltHead];
        public override List<int> ChestsToApplyTo => [ItemID.ApprenticeAltShirt];
        public override List<int> LegsToApplyTo => [ItemID.ApprenticeAltPants];

        public override void HeadEquips(Item item, Player player)
        {
            player.maxTurrets+= 2;
            player.GetDamage<GenericDamageClass>() += 0.15f;
        }

        public override void ChestEquips(Item item, Player player)
        {

            player.Roots().ModifyHitNPCWithProjectileFuncs.Add((player, proj, npc, mod) =>
            {
                if (proj.IsMinionOrSentryRelated)
                    player.Roots().AdditiveDamageMultipliersToApplyOnHit += 0.25f;
                if (proj.Roots().isManaProjectile)
                    player.Roots().AdditiveDamageMultipliersToApplyOnHit += 0.1f;
                return mod;
            });
        }

        public override void LegsEquips(Item item, Player player)
        {
            player.Roots().ModifyHitNPCWithProjectileFuncs.Add((player, proj, npc, mod) =>
            {
                if (proj.IsMinionOrSentryRelated)
                    player.Roots().AdditiveDamageMultipliersToApplyOnHit += 0.2f;
                if (proj.Roots().isManaProjectile)
                    proj.CritChance += 25;
                return mod;
            });
            player.moveSpeed += 0.2f;
            player.manaCost -= 0.1f;
        }
        public override void SetBonusEffect(Player player)
        {
            player.maxTurrets++;
            player.setApprenticeT2 = true;
            player.setApprenticeT3 = true;
        }
    }
}
