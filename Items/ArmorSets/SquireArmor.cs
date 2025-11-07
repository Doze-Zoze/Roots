using Roots.Utilities;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Roots.Items.ArmorSets
{
    public class SquireArmor : BaseArmorSet
    {
        public override string SetID => "Squire";
        public override List<int> HeadsToApplyTo => [ItemID.SquireGreatHelm];
        public override List<int> ChestsToApplyTo => [ItemID.SquirePlating];
        public override List<int> LegsToApplyTo => [ItemID.SquireGreaves];

        public override void HeadEquips(Item item, Player player)
        {
            player.maxTurrets++;
            player.lifeRegen+= 4;
        }

        public override void ChestEquips(Item item, Player player)
        {
            player.Roots().ModifyHitNPCWithProjectileFuncs.Add((player, proj, npc, mod) =>
            {
                if (proj.IsMinionOrSentryRelated || (player.Distance(npc.Center) <= 16 * 25))
                    player.Roots().AdditiveDamageMultipliersToApplyOnHit += 0.15f;
                return mod;
            });
            player.Roots().ModifyHitNPCWithItemFuncs.Add((player, item, npc, modifiers) =>
            {
                player.Roots().AdditiveDamageMultipliersToApplyOnHit += 0.15f;
                return modifiers;
            });
        }

        public override void LegsEquips(Item item, Player player)
        {
            player.Roots().ModifyHitNPCWithProjectileFuncs.Add((player, proj, npc, mod) =>
            {
                if (proj.IsMinionOrSentryRelated)
                    player.Roots().AdditiveDamageMultipliersToApplyOnHit += 0.15f;
                if ((player.Distance(npc.Center) > 16 * 25))
                    proj.CritChance -= 15;
                return mod;
            });
            player.GetCritChance<GenericDamageClass>() += 15;
        }
        public override void SetBonusEffect(Player player)
        {
            player.maxTurrets++;
            player.setSquireT2 = true;
        }
    }
}
