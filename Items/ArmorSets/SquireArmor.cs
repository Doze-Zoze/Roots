using Roots.Config;
using RootsBeta.Players;
using RootsBeta.Utilities;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RootsBeta.Items.ArmorSets
{
    public class SquireArmor : BaseArmorSet<SquireArmor>, IConfigurableContent<SquireArmor>
    {
        public static ConfigGroup ConfigGroups => ConfigGroup.ArmorReworks;

        #region Parameters
        public static int SentrySlotsHead => 1;
        public static int SentrySlotsSet => 1;
        public static int LifeRegenHead => 4;
        public static float SummonOrCloseRangedDamageBonusChest => 0.15f;
        public static float SummonDamageBonusLegs => 0.15f;
        public static int CloseRangedCritChanceBonusLegs => 15;
        #endregion

        public override string SetID => "Squire";
        public override List<int> HeadsToApplyTo => [ItemID.SquireGreatHelm];
        public override List<int> ChestsToApplyTo => [ItemID.SquirePlating];
        public override List<int> LegsToApplyTo => [ItemID.SquireGreaves];

        public override void HeadEquips(Item item, Player player)
        {
            player.maxTurrets += SentrySlotsHead;
            player.lifeRegen += LifeRegenHead;
        }

        public override void ChestEquips(Item item, Player player)
        {
            player.Roots().ModifyHitNPCWithProjectileFuncs.Add((plr, proj, npc, modifiers) =>
            {
                if (proj.IsMinionOrSentryRelated || (plr.Distance(npc.Center) <= RootsPlayer.CloseRangeDistance))
                    plr.Roots().AdditiveDamageMultipliersToApplyOnHit += SummonOrCloseRangedDamageBonusChest;
                return modifiers;
            });
            player.Roots().ModifyHitNPCWithItemFuncs.Add((plr, _, _, modifiers) =>
            {
                plr.Roots().AdditiveDamageMultipliersToApplyOnHit += SummonOrCloseRangedDamageBonusChest;
                return modifiers;
            });
        }

        public override void LegsEquips(Item item, Player player)
        {
            player.Roots().ModifyHitNPCWithProjectileFuncs.Add((plr, proj, npc, modifiers) =>
            {
                if (proj.IsMinionOrSentryRelated)
                    plr.Roots().AdditiveDamageMultipliersToApplyOnHit += SummonDamageBonusLegs;
                if ((plr.Distance(npc.Center) > RootsPlayer.CloseRangeDistance))
                    proj.CritChance -= CloseRangedCritChanceBonusLegs;
                return modifiers;
            });
            player.GetCritChance<GenericDamageClass>() += CloseRangedCritChanceBonusLegs;
        }

        public override void SetBonusEffect(Player player)
        {
            player.maxTurrets += SentrySlotsSet;
            player.setSquireT2 = true;
        }
    }
}
