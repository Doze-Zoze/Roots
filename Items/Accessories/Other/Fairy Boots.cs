using Microsoft.Xna.Framework;
using Roots.Utilities;
using System.Collections.Generic;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Roots.Items.Accessories.Magic
{
    public class FairyBoots : GlobalItem
    {
        public override bool IsLoadingEnabled(Mod mod) => Configs.instance.ManaChanges;
        public override bool AppliesToEntity(Item item, bool lateInstantiation) => item.type == ItemID.FairyBoots;
        public override void SetStaticDefaults()
        {
            ItemSets.DontUseVanillaEquipEffects[ItemID.FairyBoots] = true;
        }

        public override void UpdateAccessory(Item item, Player player, bool hideVisual)
        {

            player.rocketBoots = player.vanityRocketBoots = 1;
            player.accRunSpeed = 6;
            player.rocketTime = 0;
            player.canRocket = player.statMana >= 10 && player.jump == 0 && !(player.velocity.Y == 0);
            if (player.wingsLogic == 0 || player.wingTimeMax == 0)
            {
                if (player.controlJump && player.rocketDelay == 0 && player.canRocket && player.rocketRelease && !player.AnyExtraJumpUsable())
                {
                    player.statMana -= (int)(10 * player.manaCost);
                    player.manaRegenDelay = MathHelper.Max(40, player.manaRegenDelay);
                    player.rocketDelay = 10;
                    if (player.rocketSoundDelay <= 0)
                    {
                        player.rocketSoundDelay = 30;
                        SoundEngine.PlaySound(SoundID.Item13, player.Center);
                    }
                }
            } else
            {
                if (player.wingTime == 0 && player.wingTimeMax > 0 && player.statMana >= 2 && !player.mount.Active)
                {
                    player.wingTime++;
                    player.statMana -= 2;
                    player.manaRegenDelay = MathHelper.Max(30, player.manaRegenDelay);
                }
            }
        }
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            tooltips.ReplaceTooltipWith("Accessories.FairyBoots.Tooltip");
        }
    }
}
