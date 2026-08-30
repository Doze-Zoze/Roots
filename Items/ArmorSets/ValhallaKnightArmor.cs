using RootsBeta.Utilities;
using System.Collections.Generic;
using RootsBeta.Players;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RootsBeta.Items.ArmorSets
{
    public class ValhallaKnightArmor : BaseArmorSet
    {
        #region Parameters
        public static int SentrySlotsHead => 2;
        public static int SentrySlotsSet => 1;
        public static float SummonOrCloseRangedDamageBonusHead => 0.1f;
        public static int LifeRegenChest => 8;
        public static float SummonDamageBonusChest => 0.3f;
        public static float SummonDamageBonusLegs => 0.2f;
        public static int CloseRangedCritChanceBonusLegs => 20;
        #endregion

        public override string SetID => "ValhallaKnight";
        public override List<int> HeadsToApplyTo => [ItemID.SquireAltHead];
        public override List<int> ChestsToApplyTo => [ItemID.SquireAltShirt];
        public override List<int> LegsToApplyTo => [ItemID.SquireAltPants];

        public override void HeadEquips(Item item, Player player)
        {
            player.maxTurrets += SentrySlotsHead;
            player.Roots().ModifyHitNPCWithProjectileFuncs.Add((plr, proj, npc, modifiers) =>
            {
                if (proj.IsMinionOrSentryRelated || (plr.Distance(npc.Center) <= RootsPlayer.CloseRangeDistance))
                    plr.Roots().AdditiveDamageMultipliersToApplyOnHit += SummonOrCloseRangedDamageBonusHead;
                return modifiers;
            });
            player.Roots().ModifyHitNPCWithItemFuncs.Add((plr, _, _, modifiers) =>
            {
                plr.Roots().AdditiveDamageMultipliersToApplyOnHit += SummonOrCloseRangedDamageBonusHead;
                return modifiers;
            });
        }

        public override void ChestEquips(Item item, Player player)
        {
            player.lifeRegen += LifeRegenChest;
            player.Roots().ModifyHitNPCWithProjectileFuncs.Add((plr, proj, _, modifiers) =>
            {
                if (proj.IsMinionOrSentryRelated)
                    plr.Roots().AdditiveDamageMultipliersToApplyOnHit += SummonDamageBonusChest;
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
            player.setSquireT3 = true;
        }
    }
}
