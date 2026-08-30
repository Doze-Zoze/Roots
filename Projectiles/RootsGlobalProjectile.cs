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
        #endregion
        public override void SetDefaults(Projectile entity)
        {
            IsManaProjectile = ProjSets.ManaSpawnedProjectile[entity.type] || IsManaProjectile;
        }
    }
}
