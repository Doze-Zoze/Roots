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
            On_Player.GrantArmorBenefits += DisableArmorPieceBuffs;
            On_Player.UpdateArmorSets += DisableArmorSetBonus;
        }

        private void DisableArmorSetBonus(On_Player.orig_UpdateArmorSets orig, Player self, int i)
        {
            if (ItemSets.DontUseVanillaSetBonus[self.armor[0].type] || ItemSets.DontUseVanillaSetBonus[self.armor[2].type] || ItemSets.DontUseVanillaSetBonus[self.armor[2].type])
            {

                ItemLoader.UpdateArmorSet(self, self.armor[0], self.armor[1], self.armor[2]);
                return;
            }
            orig(self, i);
        }

        private void DisableArmorPieceBuffs(On_Player.orig_GrantArmorBenefits orig, Player self, Item armorPiece)
        {
            if (ItemSets.DontUseVanillaEquipEffects[armorPiece.type]) 
            {
                int type = armorPiece.type;
                self.RefreshInfoAccsFromItemType(armorPiece);
                self.RefreshMechanicalAccsFromItemType(type);

                self.statDefense += armorPiece.defense;
                self.lifeRegen += armorPiece.lifeRegen;
                if (armorPiece.shieldSlot > 0)
                    self.hasRaisableShield = true;

                if (armorPiece.type == ItemID.AmberRobe || (armorPiece.type >= ItemID.AmethystRobe && armorPiece.type <= ItemID.DiamondRobe))
                    self.hasGemRobe = true;

                ItemLoader.UpdateEquip(armorPiece, self);
                return; 
            }
            orig(self, armorPiece);
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