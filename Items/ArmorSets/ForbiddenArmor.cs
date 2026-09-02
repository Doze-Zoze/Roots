using Roots.Config;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RootsBeta.Items.ArmorSets
{
    public class ForbiddenArmor : BaseArmorSet<ForbiddenArmor>, IConfigurableContent<ForbiddenArmor>
    {
        public static ConfigGroup ConfigGroups => ConfigGroup.ArmorReworks;

        #region Parameters
        public static float DamageBonusHead => 0.15f;
        public static float DamageBonusChest => 0.05f;
        public static float DamageBonusLegs => 0.05f;
        public static int ManaMaxBonusChest => 80;
        public static int MinionSlotsLegs => 2;
        #endregion

        public override string SetID => "Forbidden";
        public override List<int> HeadsToApplyTo => [ItemID.AncientBattleArmorHat];
        public override List<int> ChestsToApplyTo => [ItemID.AncientBattleArmorShirt];
        public override List<int> LegsToApplyTo => [ItemID.AncientBattleArmorPants];

        public override void HeadEquips(Item item, Player player)
        {
            player.GetDamage<GenericDamageClass>() += DamageBonusHead;
        }

        public override void ChestEquips(Item item, Player player)
        {
            player.GetDamage<GenericDamageClass>() += DamageBonusChest;
            player.statMana += ManaMaxBonusChest;
        }

        public override void LegsEquips(Item item, Player player)
        {
            player.GetDamage<GenericDamageClass>() += DamageBonusLegs;
            player.slotsMinions += MinionSlotsLegs;

        }

        public override void SetBonusEffect(Player player)
        {
            player.setForbidden = true;
        }
    }
}
