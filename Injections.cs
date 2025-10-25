using Roots.Utilities;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Roots
{
    public partial class Roots : Mod
    {
        void LoadEdits()
        {
            On_Player.ItemCheck_PayMana += AllowItemUsageWithImproperMana;
            On_Player.ItemCheck_ApplyManaRegenDelay += DisableManaRegenDelayWhenOutOfMana;
            On_Player.ApplyEquipFunctional += DisableVanillaEquipmentBuffs;
        }

        private void DisableVanillaEquipmentBuffs(On_Player.orig_ApplyEquipFunctional orig, Player self, Item currentItem, bool hideVisual)
        {
            if (ItemSets.DontUseVanillaEquipEffects[currentItem.type])
            {
                ItemLoader.UpdateAccessory(currentItem,self,hideVisual);
                return;
            }
            orig(self, currentItem, hideVisual);
            return;
        }

        private void DisableManaRegenDelayWhenOutOfMana(On_Player.orig_ItemCheck_ApplyManaRegenDelay orig, Player self, Item sItem)
        {
            if (ItemSets.ShouldResetManaRegen[sItem.type]((self, sItem)))
                orig(self, sItem);
        }

        private bool AllowItemUsageWithImproperMana(On_Player.orig_ItemCheck_PayMana orig, Player self, Item sItem, bool canUse)
        {
            if (ItemSets.DontConsumeManaOnSwing[sItem.type])
                return canUse;
            return orig(self, sItem, canUse);
        }
    }
}