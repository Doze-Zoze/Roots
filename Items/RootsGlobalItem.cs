using Roots.Config;
using RootsBeta.Utilities;
using RootsCore;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RootsBeta.Items
{

    public partial class RootsGlobalItem : GlobalItem
    {
        #region Parameters
        public static float TitanGloveScale => 1.1f;
        #endregion

        public static HashSet<string> WhitelistedMods = ["Roots", "RootsBeta"];

        public override void SetDefaults(Item item)
        {
            if (RootsModConfig.Instance.RemoveClasses && (item.ModItem is null || WhitelistedMods.Contains(item.ModItem.FullName.Split('/')[0])))
                item.DamageType = DamageClass.Generic;

            //TODO - Config
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
            if (!RootsModConfig.Instance.RemoveClasses)
                return;
            if (player.meleeScaleGlove)
                scale *= TitanGloveScale;
        }
    }
}
