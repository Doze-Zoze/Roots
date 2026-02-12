using RootsBeta.Utilities;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RootsBeta.Items.ArmorSets
{
    public class ObsidianArmor : BaseArmorSet
    {
        public override string SetID => "Obsidian";
        public override List<int> HeadsToApplyTo => [ItemID.ObsidianHelm];
        public override List<int> ChestsToApplyTo => [ItemID.ObsidianShirt];
        public override List<int> LegsToApplyTo => [ItemID.ObsidianPants];

        public override void HeadEquips(Item item, Player player)
        {
            player.GetDamage<GenericDamageClass>() += 0.08f;
            player.slotsMinions++;
        }

        public override void ChestEquips(Item item, Player player)
        {
            player.slotsMinions++;
        }

        public override void LegsEquips(Item item, Player player)
        {
            player.GetDamage<GenericDamageClass>() += 0.08f;
            
        }
        public override void SetBonusEffect(Player player)
        {
            player.Roots().ModifyHitNPCWithProjectileFuncs.Add((player, proj, npc, mod) =>
            {
                if (proj.IsMinionOrSentryRelated || proj.DamageType == DamageClass.SummonMeleeSpeed)
                    player.Roots().AdditiveDamageMultipliersToApplyOnHit += 0.15f;
                return mod;
            });
            player.whipRangeMultiplier += 0.3f;
        }
    }
}
