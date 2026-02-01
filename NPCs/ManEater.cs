using Microsoft.Xna.Framework;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Roots.NPCs
{
    public class ManEater : AIOverride
    {
        public ManEater(NPC npc) : base(npc) { }

        #region Balancing Stats

        static float BeginChargingThreshold => 640;

        static float IdleVineLength => 240;

        static float StopChargingThreshold => 800f;
        static float BaseMovementSpeed => 0.125f;
        #endregion
        
        #region AI
        public override bool PreAI()
        {
            return true;
        }

        public override void AI()
        {
            if (!isInWorld(vinePos) || attachPoint == null)
            {
                return;
            }
            if (!attachPoint.HasTile)
            {
                //NPC.life = -1;
                //NPC.HitEffect();
                //NPC.active = false;
                return;
            }
            FixExploitManEaters.ProtectSpot(vinePos.X, vinePos.Y);
            NPC.TargetClosest();
            if (!NPC.HasValidTarget)
                return;

            Vector2 toPlayer = NPC.DirectionTo(player.Center);
            Vector2 toPlayerFromVine = worldVinePos.DirectionTo(player.Center);
            float playerVineDis = worldVinePos.Distance(player.Center);

            if (NPC.ai[2] == 0 && playerVineDis < BeginChargingThreshold)
                NPC.ai[2] = 1;

            if (NPC.ai[2] == 1 && playerVineDis > StopChargingThreshold)
                NPC.ai[2] = 0;

            float vineLength = IdleVineLength;
            if (NPC.ai[2] == 1)
                vineLength =  playerVineDis;




            NPC.velocity += BaseMovementSpeed * (NPC.DirectionTo(worldVinePos + toPlayerFromVine * vineLength));
            NPC.velocity *= 0.98f;
            NPC.rotation = (toPlayer+toPlayerFromVine*2f).ToRotation() + MathHelper.Pi;
        }
        #endregion

        #region Helpers
        Point vinePos => new((int)NPC.ai[0], (int)NPC.ai[1]);

        Vector2 worldVinePos => vinePos.ToWorldCoordinates();
        Tile attachPoint => Main.tile[vinePos];

        Player player => Main.player[NPC.target];

        bool isInWorld(Point pos)
        {
            return pos.X >= 0 && pos.X < Main.maxTilesX && pos.Y >= 0 && pos.Y < Main.maxTilesY;
        }
        #endregion

    }
}
