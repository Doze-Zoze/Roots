using RootsBeta.Utilities;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RootsBeta.Items.ArmorSets
{
    public class ValhallaKnightArmor : BaseArmorSet
    {
        public override string SetID => "ValhallaKnight";
        public override List<int> HeadsToApplyTo => [ItemID.SquireAltHead];
        public override List<int> ChestsToApplyTo => [ItemID.SquireAltShirt];
        public override List<int> LegsToApplyTo => [ItemID.SquireAltPants];

        public override void HeadEquips(Item item, Player player)
        {
            player.maxTurrets+=2;
            player.Roots().ModifyHitNPCWithProjectileFuncs.Add((player, proj, npc, mod) =>
            {
                if (proj.IsMinionOrSentryRelated || (player.Distance(npc.Center) <= 16 * 25))
                    player.Roots().AdditiveDamageMultipliersToApplyOnHit += 0.1f;
                return mod;
            });
            player.Roots().ModifyHitNPCWithItemFuncs.Add((player, item, npc, modifiers) =>
            {
                player.Roots().AdditiveDamageMultipliersToApplyOnHit += 0.1f;
                return modifiers;
            });
        }

        public override void ChestEquips(Item item, Player player)
        {
            player.lifeRegen += 8;
            player.Roots().ModifyHitNPCWithProjectileFuncs.Add((player, proj, npc, mod) =>
            {
                if (proj.IsMinionOrSentryRelated)
                    player.Roots().AdditiveDamageMultipliersToApplyOnHit += 0.3f;
                return mod;
            });
        }

        public override void LegsEquips(Item item, Player player)
        {
            player.Roots().ModifyHitNPCWithProjectileFuncs.Add((player, proj, npc, mod) =>
            {
                if (proj.IsMinionOrSentryRelated)
                    player.Roots().AdditiveDamageMultipliersToApplyOnHit += 0.2f;
                if ((player.Distance(npc.Center) > 16 * 25))
                    proj.CritChance -= 20;
                return mod;
            });
            player.GetCritChance<GenericDamageClass>() += 20;

        }
        public override void SetBonusEffect(Player player)
        {
            player.maxTurrets++;
            player.setSquireT2 = true;
            player.setSquireT3 = true;
        }
    }
}
