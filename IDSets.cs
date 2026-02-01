using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RootsBeta
{
    public partial class RootsBeta : Mod
    {
        void FinishIDSets()
        {
            for (int i = 0; i < ProjSets.ManaSpawnedProjectile.Length; i++)
            {
                if (ContentSamples.ProjectilesByType[i].DamageType == DamageClass.Magic)
                {
                    ProjSets.ManaSpawnedProjectile[i] = true;
                }
            }
        }
    }
    [ReinitializeDuringResizeArrays]
    public static class ItemSets
    {
        public static bool[] DontConsumeManaOnSwing = ItemID.Sets.Factory
            .CreateNamedSet("DontConsumeManaOnSwing")
            .Description("Stops items with Item.mana set from consuming Mana on swing")
            .RegisterBoolSet(false);

        public static bool[] DontUseVanillaEquipEffects = ItemID.Sets.Factory
            .CreateNamedSet("DontUseVanillaEquipEffects")
            .Description("Stops items from applying their vanilla equipment effects")
            .RegisterBoolSet(false);

        public static bool[] DontUseVanillaSetBonus = ItemID.Sets.Factory
            .CreateNamedSet("DontUseVanillaSetBonus")
            .Description("Stops items from applying their vanilla set bonus")
            .RegisterBoolSet(false);

        public static bool[] ManaStarPickup = ItemID.Sets.Factory
            .CreateNamedSet("ManaStarPickup")
            .Description("Items that count as Mana Star pickups")
            .RegisterBoolSet(false, ItemID.Star, ItemID.ManaCloakStar, ItemID.SoulCake, ItemID.SugarPlum);

        public static Predicate<(Player, Item)>[] ShouldResetManaRegen = ItemID.Sets.Factory
            .CreateNamedSet("ShouldResetManaRegen")
            .Description("Predicate to decide when an item should reset mana regeneration")
            .RegisterCustomSet<Predicate<(Player, Item)>>(x => x.Item1.statMana >= (int)(x.Item2.mana * x.Item1.manaCost));
    }

    [ReinitializeDuringResizeArrays]
    public static class ProjSets
    {
        public static bool[] ManaSpawnedProjectile = ProjectileID.Sets.Factory
            .CreateNamedSet("ManaSpawnedProjectile")
            .Description("Projectiles spanwed from mana-consuming attacks")
            .RegisterBoolSet(false,
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
            );
    }
}
