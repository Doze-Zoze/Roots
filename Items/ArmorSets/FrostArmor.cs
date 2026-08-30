using RootsBeta.Utilities;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RootsBeta.Items.ArmorSets
{
    public class FrostArmor : BaseArmorSet
    {
        #region Parameters

        public static int CritChanceBonusHead => 16;
        public static int CritChanceBonusChest => 11;
        public static float MoveSpeedBonusLegs => 0.08f;
        public static float ShotSpeedMultiplier => 1.1f;
        public static float PhysicalDamageBonusSet => 0.15f;
        public static int FrostburnDebuffFrames => 60;
        #endregion

        public override string SetID => "Frost";
        public override List<int> HeadsToApplyTo => [ItemID.FrostHelmet];
        public override List<int> ChestsToApplyTo => [ItemID.FrostBreastplate];
        public override List<int> LegsToApplyTo => [ItemID.FrostLeggings];

        public override void HeadEquips(Item item, Player player)
        {
            player.GetCritChance<GenericDamageClass>() += CritChanceBonusHead;
        }

        public override void ChestEquips(Item item, Player player)
        {
            player.GetCritChance<GenericDamageClass>() += CritChanceBonusChest;
        }

        public override void LegsEquips(Item item, Player player)
        {
            player.moveSpeed += MoveSpeedBonusLegs;
            player.Roots().ShootSpeedMult *= ShotSpeedMultiplier;
        }

        public override void SetBonusEffect(Player player)
        {
            player.frostArmor = true;
            player.Roots().PhysicalModifyHitNPCFuncs.Add((plr, target, modifier) =>
            {
                plr.Roots().AdditiveDamageMultipliersToApplyOnHit += PhysicalDamageBonusSet;
                target.AddBuff(BuffID.Frostburn2, FrostburnDebuffFrames);
                return modifier;
            });
        }
    }
}
