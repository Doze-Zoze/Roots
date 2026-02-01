using Terraria;
using Terraria.ModLoader;

namespace RootsBeta.Projectiles
{
    public class RootsGlobalProjectile : GlobalProjectile
    {
        public override bool InstancePerEntity => true;
        #region Variables

        public bool isManaProjectile = false;

        #endregion
        public override void SetDefaults(Projectile entity)
        {
            isManaProjectile = ProjSets.ManaSpawnedProjectile[entity.type] || isManaProjectile;
        }
    }
}
