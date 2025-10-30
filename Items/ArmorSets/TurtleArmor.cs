using Roots.Utilities;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Roots.Items.ArmorSets
{
    public class TurtleArmor : BaseArmorSet
    {
        public override string SetID => "Turtle";
        public override List<int> HeadsToApplyTo => [ItemID.TurtleHelmet];
        public override List<int> ChestsToApplyTo => [ItemID.TurtleScaleMail];
        public override List<int> LegsToApplyTo => [ItemID.TurtleLeggings];

        void AdditiveDamageAtCloseRange(Player player, float amount)
        {
            player.Roots().ModifyHitNPCWithProjectileFuncs.Add((player, projectile, npc, modifiers) => {
                if (player.Distance(npc.Center) <= 16 * 25)
                    player.Roots().AdditiveDamageMultipliersToApplyOnHit += 0.15f;
                return modifiers;
            });
            player.Roots().ModifyHitNPCWithItemFuncs.Add((player, item, npc, modifiers) =>
            {
                player.Roots().AdditiveDamageMultipliersToApplyOnHit += 0.15f;
                return modifiers;
            });
        }
        public override void HeadEquips(Item item, Player player)
        {
            AdditiveDamageAtCloseRange(player, 0.06f);
        }

        public override void ChestEquips(Item item, Player player)
        {
            AdditiveDamageAtCloseRange(player, 0.08f);
            player.GetCritChance<GenericDamageClass>() += 8;
        }

        public override void LegsEquips(Item item, Player player)
        {
            player.GetCritChance<GenericDamageClass>() += 4;
        }

        public override void SetBonusEffect(Player player)
        {
            player.turtleArmor = true;
            player.turtleThorns = true;
            player.endurance += 0.15f;
        }
    }
}
