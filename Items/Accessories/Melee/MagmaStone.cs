using Roots.Config;
using RootsBeta.Utilities;
using System.Collections.Generic;
using RootsCore;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RootsBeta.Items.Accessories.Melee
{
    public class MagmaStone : ConfigurableItemRework<MagmaStone>, IConfigurableContent<MagmaStone>
    {
        public static ConfigGroup ConfigGroups => ConfigGroup.AccessoryReworks;
        public override int[] ItemIds => [ItemID.MagmaStone];
        public override void SetDefaults(Item entity) => ItemSets.DontUseVanillaEquipEffects[ItemID.MagmaStone] = true;
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips) =>
            tooltips.ReplaceTooltipWith("Accessories.MagmaStone.Tooltip");

        public static void OnHit(Player player, NPC target, NPC.HitInfo hit, int damageDone) {
            target.AddBuff(BuffID.OnFire3, 120);
        }

        public override void UpdateAccessory(Item item, Player player, bool hideVisual)
        {
            player.Roots().PhysicalOnHitNPCFuncs.Add(OnHit);
        }
    }
}