using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text.Json.Serialization;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.Core;

namespace Roots.Config
{
    #region Toggleable Content List System
    [Flags] public enum ConfigGroup
    {
        None = 0,
        WeaponReworks = 1,
        AccessoryReworks = 1 << 1,
        ArmorReworks = 1 << 2,
        BossReworks = 1 << 3,
        EnemyReworks = 1 << 4,
        BetaContent = 1 << 5
    }
    public interface IConfigurableContent<T>
    {
        public static virtual string ConfigName => typeof(T).Name;
        public static virtual ConfigGroup ConfigGroups => ConfigGroup.None;

        public static virtual bool DefaultState => true;
    }
    public class ConfigurationSystem
    {
        public class ContentConfigData(Type parentType, string name, bool enabled = true, ConfigGroup groups = ConfigGroup.None)
        {
            public string Name { get; init; } = name;
            [ReloadRequired] public bool Enabled { get; set; } = enabled;
            [JsonIgnore] internal ConfigGroup Groups { get; init; } = groups;
            [JsonIgnore] internal Type ParentType { get; init; } = parentType;
            public override bool Equals(object obj)
            {
                if (obj is ContentConfigData other)
                    return other.Name == Name && other.Enabled == Enabled;
                return base.Equals(obj);
            }

            public override int GetHashCode()
            {
             return HashCode.Combine(Name, Enabled);
            }
        }

        #region Toggleable Content List
        public static string[] LoadedContentToggleNames;
        public static ContentConfigData[] DefaultContentToggleData
        {
            get => LoadContentTable();
        }
        private static ContentConfigData[] LoadContentTable()
        {
            List<ContentConfigData> contentList = [];
            Type[] allTypes = AssemblyManager.GetLoadableTypes(typeof(RootsBeta.RootsBeta).Assembly);

            var factory= typeof(ConfigurationSystem).GetMethod(nameof(CreateConfigData), BindingFlags.NonPublic | BindingFlags.Static);

            foreach (Type type in allTypes)
            {
                if (type.IsAbstract) continue;
               
                var loadedInterface = type.GetInterfaces().FirstOrDefault(x => x.IsGenericType && x.GetGenericTypeDefinition() == typeof(IConfigurableContent<>));
                if (loadedInterface is null) continue;

                contentList.Add((ContentConfigData)factory.MakeGenericMethod(type).Invoke(null,null));

            }
            contentList.Sort((x, y) => x.Name.CompareTo(y.Name));

            LoadedContentToggleNames = [.. contentList.Select(x => x.Name)];
            return [.. contentList];
        }

        private static ContentConfigData CreateConfigData<T>() where T : IConfigurableContent<T>
        {
            return new ContentConfigData(
                typeof(T),
                T.ConfigName,
                T.DefaultState,
                T.ConfigGroups
            );
        }
        #endregion

    }

    public static class ConfigHelpers
    {
        public static bool ConfigEnabled(string ID) 
        {

            if (!ConfigurationSystem.LoadedContentToggleNames.Contains(ID))
            {
                Debug.Fail("ERROR: ID missing from config list");
               return true;
            }
            return RootsModConfig.Instance.ToggleableContent.FirstOrDefault(x => x.Name == ID,null)?.Enabled ?? true;
        }
        extension(ConfigGroup group)
        {
            public bool GetToggleState() => !RootsModConfig.Instance.ToggleableContent.Any(x => x.Groups.HasFlag(group) && !x.Enabled);
            public void SetToggleState(bool state)
            {
                foreach (ConfigurationSystem.ContentConfigData item in RootsModConfig.Instance.ToggleableContent)
                {
                    if (!item.Groups.HasFlag(group))
                        continue;
                    item.Enabled = state;
                }
            }
        }

        public static bool ConfigEnabled<T>() => ConfigEnabled(ConfigurationSystem.DefaultContentToggleData.FirstOrDefault(x => x.ParentType == typeof(T))?.Name ?? "");
    }
    #endregion

    #region Base Classes 
    public abstract class ConfigurableItemRework<T> : GlobalItem
    {
        public abstract int[] ItemIds { get; }
        public override bool AppliesToEntity(Item entity, bool lateInstantiation) => ItemIds.Contains(entity.type);
        public override bool IsLoadingEnabled(Mod mod) => ConfigHelpers.ConfigEnabled<T>();

    }
    #endregion
}
