using RootsCore;
using Terraria;
using Terraria.ModLoader;

namespace RootsBeta.Projectiles
{
    public class RootsGlobalProjectile : GlobalProjectile
    {
        public override bool InstancePerEntity => true;
        #region Variables
        public bool IsManaProjectile;

        public StatModifier DamageModifier = new();
        #endregion
        public override void SetDefaults(Projectile entity)
        {
            IsManaProjectile = ProjSets.ManaSpawnedProjectile[entity.type] || IsManaProjectile;
        }

        public override void ModifyHitNPC(Projectile projectile, NPC target, ref NPC.HitModifiers modifiers)
        {
            modifiers.SourceDamage.Flat += DamageModifier.Flat;
            modifiers.SourceDamage *= DamageModifier.Multiplicative;
            modifiers.SourceDamage *= DamageModifier.Additive;
        }
        public override void ModifyHitPlayer(Projectile projectile, Player target, ref Player.HurtModifiers modifiers)
        {
            modifiers.SourceDamage.Flat += DamageModifier.Flat;
            modifiers.SourceDamage *= DamageModifier.Multiplicative;
            modifiers.SourceDamage *= DamageModifier.Additive;
        }
    }
}
