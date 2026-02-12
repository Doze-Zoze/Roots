using Microsoft.Xna.Framework;
using RootsBeta.Players;
using RootsBeta.Utilities;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace RootsBeta.Items.Accessories.Magic
{
    public class ManaSickness : GlobalBuff
    {
        public override void ModifyBuffText(int type, ref string buffName, ref string tip, ref int rare)
        {
            tip = Language.GetTextValue("Mods.Roots.Buffs.ManaSickness.Tooltip");
        }
        public override bool IsLoadingEnabled(Mod mod) => Configs.instance.ManaChanges;
    }
}
