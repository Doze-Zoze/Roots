using Roots.Utilities;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Roots.Items.ArmorSets
{
    public class ForbiddenArmor : BaseArmorSet
    {
        public override string SetID => "Forbidden";
        public override List<int> HeadsToApplyTo => [ItemID.AncientBattleArmorHat];
        public override List<int> ChestsToApplyTo => [ItemID.AncientBattleArmorShirt];
        public override List<int> LegsToApplyTo => [ItemID.AncientBattleArmorPants];

        public override void HeadEquips(Item item, Player player)
        {
            player.GetDamage<GenericDamageClass>() += 0.15f;
        }

        public override void ChestEquips(Item item, Player player)
        {
            player.GetDamage<GenericDamageClass>() += 0.05f;
            player.statMana += 80;
        }

        public override void LegsEquips(Item item, Player player)
        {
            player.GetDamage<GenericDamageClass>() += 0.05f;
            player.slotsMinions += 2;

        }
        public override void SetBonusEffect(Player player)
        {
            player.setForbidden = true;
        }
    }
}
