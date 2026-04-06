using RootsCore;
using System.Collections.Generic;
using Terraria.ID;
using Terraria.ModLoader;

namespace RootsBeta
{
    public partial class RootsBeta : Mod
    {
        readonly List<int> _projToMarkAsMana =
            [
                ProjectileID.TerraBlade2Shot,
                ProjectileID.Starfury,
                ProjectileID.EnchantedBeam,
                ProjectileID.TrueNightsEdge,
                ProjectileID.ChlorophyteOrb,
                ProjectileID.SporeCloud,
                ProjectileID.OrnamentStar,
                ProjectileID.OrnamentFriendly,
                ProjectileID.DeathSickle,
                ProjectileID.DD2SquireSonicBoom,
                ProjectileID.FrostBoltSword,
                ProjectileID.IceBolt,
                ProjectileID.IceSickle,
                ProjectileID.InfluxWaver,
                ProjectileID.Meowmere,
                ProjectileID.MonkStaffT3_AltShot,
                ProjectileID.StarWrath
            ];
        void FinishIDSets()
        {
            for (int i = 0; i < ProjSets.ManaSpawnedProjectile.Length; i++)
            {
                if (_projToMarkAsMana.Contains(i) || ContentSamples.ProjectilesByType[i].DamageType == DamageClass.Magic)
                {
                    ProjSets.ManaSpawnedProjectile[i] = true;
                }
            }
        }
    }
}
