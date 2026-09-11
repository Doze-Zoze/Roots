using Roots.Config;
using RootsBeta.Utilities;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RootsBeta.Items.ArmorSets
{
    public class ObsidianArmor : BaseArmorSet<ObsidianArmor>, IConfigurableContent<ObsidianArmor>
    {
        public static ConfigGroup ConfigGroups => ConfigGroup.ArmorReworks;

        #region Parameters
        public static float DamageBonusHead => 0.08f;
        public static float DamageBonusLegs => 0.08f;
        public static int MinionSlotsHead => 1;
        public static int MinionSlotsChest => 1;
        public static float SummonOrWhipDamageBonusSet => 0.15f;
        public static float WhipRangeBonusSet => 0.3f;
        #endregion

        public override string SetID => "Obsidian";
        public override List<int> HeadsToApplyTo => [ItemID.ObsidianHelm];
        public override List<int> ChestsToApplyTo => [ItemID.ObsidianShirt];
        public override List<int> LegsToApplyTo => [ItemID.ObsidianPants];

        public override void HeadEquips(Item item, Player player)
        {
            player.GetDamage<GenericDamageClass>() += DamageBonusHead;
            player.maxMinions += MinionSlotsHead;
        }

        public override void ChestEquips(Item item, Player player)
        {
            player.maxMinions += MinionSlotsChest;
        }

        public override void LegsEquips(Item item, Player player)
        {
            player.GetDamage<GenericDamageClass>() += DamageBonusLegs;
            
        }

        public override void SetBonusEffect(Player player)
        {
            player.Roots().ModifyHitNPCWithProjectileFuncs.Add((plr, proj, _, modifiers) =>
            {
                if (proj.IsMinionOrSentryRelated || proj.DamageType == DamageClass.SummonMeleeSpeed)
                    plr.Roots().AdditiveDamageMultipliersToApplyOnHit += SummonOrWhipDamageBonusSet;
                return modifiers;
            });
            player.whipRangeMultiplier += WhipRangeBonusSet;
        }
    }
}
