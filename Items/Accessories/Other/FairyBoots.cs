using Microsoft.Xna.Framework;
using Roots.Config;
using RootsBeta.Utilities;
using RootsCore;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace RootsBeta.Items.Accessories.Other
{
    public class FairyBoots : ConfigurableItemRework
    {
        #region Parameters
        public static int RunSpeed => 6;
        public static int ManaCost => 10;
        public static int ManaCostWings => 2;
        public static int ManaRegenDelay => 30;
        public static int RocketDelay => 10;
        public static int RocketSoundDelay => 30;
        #endregion

        public override int[] ItemIds => [ItemID.FairyBoots];
        public override void SetStaticDefaults() => ItemSets.DontUseVanillaEquipEffects[ItemID.FairyBoots] = true;
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips) =>
            tooltips.ReplaceTooltipWith("Accessories.FairyBoots.Tooltip");

        public override void UpdateAccessory(Item item, Player player, bool hideVisual)
        {
            player.rocketBoots = player.vanityRocketBoots = 2;
            player.fairyBoots = true;
            player.accRunSpeed = RunSpeed;
            player.rocketTime = 0;
            player.canRocket = player.statMana >= ManaCost && player.jump == 0 && player.velocity.Y != 0;
            if (player.wingsLogic == 0 || player.wingTimeMax == 0)
            {
                if (!player.controlJump || player.rocketDelay != 0 || !player.canRocket || !player.rocketRelease ||
                    player.AnyExtraJumpUsable()) return;
                player.statMana -= (int)(ManaCost * player.manaCost);
                player.manaRegenDelay = MathHelper.Max(ManaRegenDelay + RocketDelay, player.manaRegenDelay);
                player.rocketDelay = RocketDelay;
                if (player.rocketSoundDelay > 0) return;
                player.rocketSoundDelay = RocketSoundDelay;
                SoundEngine.PlaySound(SoundID.Item13, player.Center);
            }
            else
            {
                if (player.wingTime != 0 || player.wingTimeMax <= 0 || player.statMana < ManaCostWings ||
                    player.mount.Active) return;
                player.wingTime++;
                player.statMana -= ManaCostWings;
                player.manaRegenDelay = MathHelper.Max(ManaRegenDelay, player.manaRegenDelay);
            }
        }
    }
}
