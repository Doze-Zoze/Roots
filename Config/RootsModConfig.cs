using System.Collections.Generic;
using System.ComponentModel;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace Roots.Config
{
    public class RootsModConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ServerSide;

        public static RootsModConfig Instance => ModContent.GetInstance<RootsModConfig>();

        [ReloadRequired]
        public ConfigurationSystem.ContentConfigData[] ToggleableContent 
        {
            get => field ??= ConfigurationSystem.DefaultContentToggleData; 
            set;
        }

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
        public bool EnemyChanges;

        [DefaultValue(false)]
        [ReloadRequired]
        public bool BossChanges;
    }
}
