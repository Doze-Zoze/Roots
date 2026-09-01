using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.Json.Serialization;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;
using Terraria.ModLoader.Core;

namespace Roots.Config
{
    #region Toggleable Content List System
    [Flags]
    public enum ConfigGroup
    {
        None = 0,
        WeaponReworks,
        AccessoryReworks,
        ClassReworks,
        BossReworks,
        EnemyReworks


    }
    public interface IConfigurableContent
    {
        public string? ConfigName => GetType().Name;
        public ConfigGroup ConfigGroups => ConfigGroup.None;
    }

    public class ConfigurationSystem
    {
        public class ContentConfigData
        {
            public ContentConfigData(string name, bool enabled = true, ConfigGroup groups = ConfigGroup.None)
            {
                Name = name;
                Enabled = enabled;
                Groups = groups;
            }
            public string Name { get; init; }
            [ReloadRequired] public bool Enabled { get; set; }
            [JsonIgnore] internal ConfigGroup Groups { get; init; }
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
            get
            {
                if (field is not null)
                    return field;

                return LoadContentTable();
            }
            set
            {
                if (field is null)
                    field = LoadContentTable();
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
        private static ContentConfigData[] LoadContentTable()
        {
            List<ContentConfigData> contentList = [];
            Type[] allTypes = AssemblyManager.GetLoadableTypes(typeof(RootsBeta.RootsBeta).Assembly);

            foreach (Type type in allTypes)
            {
                if (type.IsAbstract || !type.GetInterfaces().Contains(typeof(IConfigurableContent))|| Activator.CreateInstance(type) is not IConfigurableContent content)
                    continue;
                contentList.Add(new(content.ConfigName, true, content.ConfigGroups));
            }

            LoadedContentToggleNames = [.. contentList.Select(x => x.Name)];
            return [.. contentList];
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
            return RootsModConfig.Instance.ToggleableContent.First(x => x.Name == ID).Enabled;
        }
        extension(ConfigGroup group)
        {
            public bool GetToggleState() => !RootsModConfig.Instance.ToggleableContent.Any(x => x.Groups.HasFlag(group) && !x.Enabled);
            public void SetToggleState(bool state)
            {
                foreach (ConfigurationSystem.ContentConfigData item in RootsModConfig.Instance.ToggleableContent)
                {
                    if (!item.Groups.HasFlag(group))
                        return;
                    item.Enabled = state;
                }
            }
        }

        extension(IConfigurableContent content) 
        {
            public bool ConfigEnabled => ConfigEnabled(content.ConfigName);
        }
    }
    #endregion

    #region Base Classes 
    public abstract class ConfigurableItemRework : GlobalItem, IConfigurableContent
    {
        public virtual ConfigGroup ConfigGroups => ConfigGroup.None;
        public abstract int[] ItemIds { get; }
        public override bool IsLoadingEnabled(Mod mod) => ConfigHelpers.ConfigEnabled((this as IConfigurableContent).ConfigName);
        public override bool AppliesToEntity(Item entity, bool lateInstantiation) => ItemIds.Contains(entity.type);

    }
    #endregion
}
