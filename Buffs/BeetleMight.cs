using RootsBeta.Items.ArmorSets;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RootsBeta.Buffs
{
    public class BeetleMight : GlobalBuff
    {
        #region Parameters
        public static float BeetleMightDamageModifier => BeetleArmor.BeetleMightDamageModifier;
        #endregion
        
        private const float BeetleMightDamageModifierVanilla = 0.1f;

        public override bool IsLoadingEnabled(Mod mod) => Configs.Instance.RemoveClasses;

        public override void Update(int type, Player player, ref int buffIndex)
        {
            if (type is < BuffID.BeetleMight1 or > BuffID.BeetleMight3) return;
            player.GetDamage<GenericDamageClass>() += BeetleMightDamageModifier * player.beetleOrbs;
            player.GetAttackSpeed<GenericDamageClass>() += BeetleMightDamageModifier * player.beetleOrbs;
            //cancel the vanilla buff so it doesn't double stack for other mods lol
            player.GetDamage<MeleeDamageClass>() -= BeetleMightDamageModifierVanilla * player.beetleOrbs;
            player.GetAttackSpeed<MeleeDamageClass>() -= BeetleMightDamageModifierVanilla * player.beetleOrbs;
        }
    }
}
