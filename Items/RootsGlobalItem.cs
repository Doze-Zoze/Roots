using Roots.Utilities;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Roots.Items
{
    public partial class RootsGlobalItem : GlobalItem
    {

        public static HashSet<string> WhitelistedMods = ["Roots"];
        public override void SetDefaults(Item item)
        {
            if (Configs.instance.RemoveClasses &&( item.ModItem is null || WhitelistedMods.Contains(item.ModItem.FullName.Split('/')[0])))
                item.DamageType = DamageClass.Generic;

            if (ProjectileID.Sets.MinionTargettingFeature[item.shoot])
                item.mana = 0;
        }

        public override bool OnPickup(Item item, Player player)
        {
            if (ItemSets.ManaStarPickup[item.type])
                player.Roots().TimeSinceManaStarPickup = 0;
            return true;
        }

        public override void ModifyItemScale(Item item, Player player, ref float scale)
        {
            if (player.meleeScaleGlove)
                scale *= 1.1f;
        }
    }
}
