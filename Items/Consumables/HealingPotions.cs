using RootsBeta.Utilities;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RootsBeta.Items.Consumables
{
    public class HealingPotions : GlobalItem
    {
        static List<int> ItemsToCount => new List<int> {
                ItemID.LesserHealingPotion,
                ItemID.GreaterHealingPotion,
                ItemID.HealingPotion,
                ItemID.SuperHealingPotion,
                ItemID.RestorationPotion,
                ItemID.LesserRestorationPotion,
                ItemID.BottledHoney,
                ItemID.BottledWater,
                ItemID.Honeyfin,
                ItemID.Eggnog
            };
        public override bool IsLoadingEnabled(Mod mod) => Configs.instance.LifeChanges;
        public override bool AppliesToEntity(Item item, bool lateInstantiation) => ItemsToCount.Contains(item.type);

        public override void SetDefaults(Item entity)
        {
            entity.healLife *= 2;
        }

        public override void OnConsumeItem(Item item, Player player)
        {
            // if (player.HasBuff(BuffID.PotionSickness))
            // player.buffTime[player.FindBuffIndex(BuffID.PotionSickness)] = 10 * 60;
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            return;
            int tooltipIndex = 0;
            for (var i = 0; i < tooltips.Count; i++)
            {
                var tooltip = tooltips[i];
                if (tooltip.Name.Contains("Tooltip"))
                {
                    tooltip.Hide();
                    tooltipIndex = i;
                }
            }
            if (tooltipIndex > 0)
                tooltips.Insert(tooltipIndex, new TooltipLine(Mod, "Tooltip", RootsUtils.GetLocalizedTextValue("Accessories.ManaCloak.Tooltip")));
        }
    }
}
