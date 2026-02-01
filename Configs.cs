using System.ComponentModel;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace Roots
{
    public class Configs : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ServerSide;

        public static Configs instance => ModContent.GetInstance<Configs>();

        [DefaultValue(true)]
        [ReloadRequired]
        public bool RemoveClasses;

        [DefaultValue(true)]
        [ReloadRequired]
        public bool LifeChanges;

        [DefaultValue(true)]
        [ReloadRequired]
        public bool ManaChanges;

        [DefaultValue(true)]
        [ReloadRequired]
        public bool AmmoChanges;

        [DefaultValue(true)]
        [ReloadRequired]
        public int WorldgenChanges;

        [DefaultValue(true)]
        [ReloadRequired]
        public bool AiChanges;
    }
}
