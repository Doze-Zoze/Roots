using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using static Roots.Config.ConfigurationSystem;

namespace Roots.Config
{
    public class RootsModConfig : ModConfig
    {
        public override ConfigScope Mode => ConfigScope.ServerSide;

        public static RootsModConfig Instance => ModContent.GetInstance<RootsModConfig>();

        

        [DefaultValue(true)]
        [ReloadRequired]
        public bool RemoveClasses;


        [DefaultValue(true)]
        [ReloadRequired]
        public bool AmmoChanges;

        [ReloadRequired]
        [JsonIgnore]
        [ShowDespiteJsonIgnore]
        public bool WeaponChanges
        {
            get => ConfigGroup.WeaponReworks.GetToggleState();
            set => ConfigGroup.WeaponReworks.SetToggleState(value);
        }


        [ReloadRequired]
        [JsonIgnore]
        [ShowDespiteJsonIgnore]
        public bool AccessoryChanges
        {
            get => ConfigGroup.AccessoryReworks.GetToggleState();
            set => ConfigGroup.AccessoryReworks.SetToggleState(value);
        }



        [ReloadRequired]
        [JsonIgnore]
        [ShowDespiteJsonIgnore]
        public bool ArmorReworks
        {
            get => ConfigGroup.ArmorReworks.GetToggleState();
            set => ConfigGroup.ArmorReworks.SetToggleState(value);
        }

        [ReloadRequired]
        [JsonIgnore]
        [ShowDespiteJsonIgnore]
        public bool EnemyChanges
        {
            get => ConfigGroup.EnemyReworks.GetToggleState();
            set => ConfigGroup.EnemyReworks.SetToggleState(value);
        }

        [ReloadRequired]
        [JsonIgnore]
        [ShowDespiteJsonIgnore]
        public bool BossChanges
        {
            get => ConfigGroup.BossReworks.GetToggleState();
            set => ConfigGroup.BossReworks.SetToggleState(value);
        }

        [ReloadRequired]
        public ContentConfigData[] ToggleableContent
        {
            get => field ??= DefaultContentToggleData;
            set
            {
                field ??= DefaultContentToggleData;
                if (value is null)
                    return;
                foreach (ContentConfigData item in value)
                {
                    if (field.Contains(item))
                        continue;
                    if (!field.Any(x => x.Name == item.Name))
                    {
                        Debug.Fail("ERROR: Saved config data contains invalid class names");
                        continue;
                    }
                    field.First(x => x.Name == item.Name).Enabled = item.Enabled;
                }
            }
        }
    }
}
