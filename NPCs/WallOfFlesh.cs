using Microsoft.Xna.Framework;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RootsBeta.NPCs
{


        public class WoFCameraSystem : ModSystem
    {
        public override void ModifyScreenPosition()
        {
            if (Main.npc.Any(x => x.active && x.type == NPCID.WallofFlesh))
            {
                var WoF = Main.npc.First(x => x.active && x.type == NPCID.WallofFlesh);
                Vector2 goalScrenPos = default;
                goalScrenPos.X = WoF.Center.X - (WoF.direction == -1 ? Main.screenWidth * .79f :  Main.screenWidth * 0.21f);
                goalScrenPos.Y = MathHelper.Lerp(WoF.Center.Y - Main.screenHeight * 0.5f,Main.LocalPlayer.Center.Y - Main.screenHeight * 0.5f, 0.25f);
                if (Main.LocalPlayer.HasBuff(BuffID.Horrified))
                    Main.screenPosition = Vector2.Lerp(Main.screenLastPosition, goalScrenPos, 0.1f);
            }
            base.ModifyScreenPosition();
        }
    }
}
