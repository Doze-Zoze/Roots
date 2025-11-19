using Roots.Utilities;
using System;
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
            On_Projectile.ghostHeal += SpectreHeal;
            On_Projectile.ghostHurt += SpectreHurt;
        }

        private void SpectreHurt(On_Projectile.orig_ghostHurt orig, Projectile self, int dmg, Microsoft.Xna.Framework.Vector2 Position, Entity victim)
        {
            //Decompiled vanilla code with the Magic check changed to isManaProjectile
            if (!self.Roots().isManaProjectile || self.damage <= 0)
            {
                return;
            }
            int num = self.damage;
            if (dmg <= 1)
            {
                return;
            }
            int num2 = -1;
            int num3 = 1500;
            if (Main.player[Main.myPlayer].ghostDmg > (float)num3)
            {
                return;
            }
            Main.player[Main.myPlayer].ghostDmg += num;
            int[] array = new int[200];
            int num4 = 0;
            _ = new int[200];
            int num5 = 0;
            for (int i = 0; i < 200; i++)
            {
                if (!Main.npc[i].CanBeChasedBy(this))
                {
                    continue;
                }
                float num6 = Math.Abs(Main.npc[i].position.X + (float)(Main.npc[i].width / 2) - self.position.X + (float)(self.width / 2)) + Math.Abs(Main.npc[i].position.Y + (float)(Main.npc[i].height / 2) - self.position.Y + (float)(self.height / 2));
                if (num6 < 800f)
                {
                    if (Collision.CanHit(self.position, 1, 1, Main.npc[i].position, Main.npc[i].width, Main.npc[i].height) && num6 > 50f)
                    {
                        array[num5] = i;
                        num5++;
                    }
                    else if (num5 == 0)
                    {
                        array[num4] = i;
                        num4++;
                    }
                }
            }
            if (num4 != 0 || num5 != 0)
            {
                num2 = ((num5 <= 0) ? array[Main.rand.Next(num4)] : array[Main.rand.Next(num5)]);
                float num7 = Main.rand.Next(-100, 101);
                float num8 = Main.rand.Next(-100, 101);
                float num9 = (float)Math.Sqrt(num7 * num7 + num8 * num8);
                num9 = 4f / num9;
                num7 *= num9;
                num8 *= num9;
                Projectile.NewProjectile(self.GetSource_OnHit(victim), Position.X, Position.Y, num7, num8, ProjectileID.SpectreWrath, num, 0f, self.owner, num2);
            }
        }

        private void SpectreHeal(On_Projectile.orig_ghostHeal orig, Projectile self, int dmg, Microsoft.Xna.Framework.Vector2 Position, Entity victim)
        {

            //Decompiled vanilla code with the Magic check changed to isManaProjectile
            if (!self.Roots().isManaProjectile)
                return;
            float num = 0.2f;
            num -= self.numHits * 0.05f;
            if (num <= 0f)
            {
                return;
            }
            float num2 = (float)dmg * num;
            if ((int)num2 <= 0 || Main.player[Main.myPlayer].lifeSteal <= 0f)
            {
                return;
            }
            Main.player[Main.myPlayer].lifeSteal -= num2;
            float num3 = 0f;
            int num4 = self.owner;
            for (int i = 0; i < 255; i++)
            {
                if (Main.player[i].active && !Main.player[i].dead && ((!Main.player[self.owner].hostile && !Main.player[i].hostile) || Main.player[self.owner].team == Main.player[i].team) && self.Distance(Main.player[i].Center) <= 3000f)
                {
                    int num5 = Main.player[i].statLifeMax2 - Main.player[i].statLife;
                    if ((float)num5 > num3)
                    {
                        num3 = num5;
                        num4 = i;
                    }
                }
            }
            Projectile.NewProjectile(self.GetSource_OnHit(victim), Position.X, Position.Y, 0f, 0f, ProjectileID.SpiritHeal, 0, 0f, self.owner, num4, num2);
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
                ItemLoader.UpdateAccessory(currentItem, self, hideVisual);
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