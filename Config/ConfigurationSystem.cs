using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text.Json.Serialization;
using RootsCore.ContentBaseClasses;
using Terraria;
using Terraria.GameContent.Prefixes;
using Terraria.ID;
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
                return obj is ContentConfigData other && Name == other.Name && other.Enabled == Enabled;
            }

            public override int GetHashCode()
            {
             return HashCode.Combine(Name, Enabled);
            }
        }

        #region Toggleable Content List
        public static string[] LoadedContentToggleNames { get; set; }
        public static ContentConfigData[] DefaultContentToggleData => LoadContentTable();

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

                Debug.Assert(factory != null, "ERROR: Something about the config data was broken, contentList is not getting added to");
                contentList.Add((ContentConfigData)factory.MakeGenericMethod(type).Invoke(null, null));
            }
            contentList.Sort((x, y) => string.Compare(x.Name, y.Name, StringComparison.Ordinal));

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
        public static bool ConfigEnabled(string id) 
        {
            if (ConfigurationSystem.LoadedContentToggleNames.Contains(id))
                return RootsModConfig.Instance.ToggleableContent.FirstOrDefault(x => x.Name == id, null)?.Enabled ??
                       true;
            Debug.Fail("ERROR: ID missing from config list");
            return true;
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
    public abstract class ConfigurableCustomSwing<T> : GlobalItem
    {
        public abstract int[] ItemIds { get; }
        public override bool AppliesToEntity(Item entity, bool lateInstantiation) => ItemIds.Contains(entity.type);
        public override bool IsLoadingEnabled(Mod mod) => ConfigHelpers.ConfigEnabled<T>();
        
        public virtual int ProjectileType { get; set; }
        public virtual bool SizeModifiers { get; set; } = true;
        public virtual bool RClickAutoswing { get; set; } = false;

        public override void SetStaticDefaults()
        {
            if (!RClickAutoswing) return;
            foreach (int id in ItemIds)
            {
                ItemID.Sets.ItemsThatAllowRepeatedRightClick[id] = true;
                PrefixLegacy.ItemSets.SwordsHammersAxesPicks[id] = SizeModifiers;
            }
        }

        public override void SetDefaults(Item item)
        {
            item.noMelee = true;
            item.noUseGraphic = true;
            item.shoot = ProjectileType;
            item.autoReuse = true;
            item.useTurn = false;
            item.UseSound = null;
            item.useStyle = ItemUseStyleID.Shoot;
        }
        public override bool CanUseItem(Item item, Player player)
        {
            if (player.itemTime > 0 || player.ownedProjectileCounts[ProjectileType] > 0)
            {
                return false;
            }
            return base.CanUseItem(item, player);
        }
    }
    #endregion
}
