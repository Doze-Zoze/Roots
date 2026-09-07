using Roots.Config;
using RootsBeta.Items.Consumables;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace RootsBeta.Buffs
{
    public class ManaSickness : GlobalBuff
    {
        public override bool IsLoadingEnabled(Mod mod) => ConfigHelpers.ConfigEnabled<ManaPotions>();

        public override void ModifyBuffText(int type, ref string buffName, ref string tip, ref int rare)
        {
            if (type != BuffID.ManaSickness) return;
            tip = Language.GetTextValue("Mods.RootsBeta.Buffs.ManaSickness.Tooltip");
        }
    }
}
