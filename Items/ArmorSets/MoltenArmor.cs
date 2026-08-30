using System.Collections.Generic;
using RootsBeta.Players;
using RootsBeta.Utilities;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RootsBeta.Items.ArmorSets
{
    public class MoltenArmor : BaseArmorSet
    {
        #region Parameters
        public static int CritChanceBonusHead => 7;
        public static float DamageBonusChest => 0.05f;
        public static float AttackSpeedBonusLegs => 0.07f;
        public static float CloseRangeDamageBonusSet => 0.1f;
        #endregion

        public override string SetID => "Molten";
        public override List<int> HeadsToApplyTo => [ItemID.MoltenHelmet];
        public override List<int> ChestsToApplyTo => [ItemID.MoltenBreastplate];
        public override List<int> LegsToApplyTo => [ItemID.MoltenGreaves];

        public override void HeadEquips(Item item, Player player)
        {
            player.GetCritChance<GenericDamageClass>() += CritChanceBonusHead;
        }

        public override void ChestEquips(Item item, Player player)
        {
            player.GetDamage<GenericDamageClass>() += DamageBonusChest;
        }

        public override void LegsEquips(Item item, Player player)
        {
            player.GetAttackSpeed<GenericDamageClass>() += 0.07f;
        }

        public override void SetBonusEffect(Player player)
        {
            player.buffImmune[BuffID.OnFire] = true;
            player.buffImmune[BuffID.Burning] = true;
            player.Roots().ModifyHitNPCFuncs.Add((plr, npc, modifiers) =>
            {
                if (plr.Distance(npc.Center) > RootsPlayer.CloseRangeDistance)
                    plr.Roots().AdditiveDamageMultipliersToApplyOnHit += CloseRangeDamageBonusSet;
                return modifiers;
            });
        }
    }
}
