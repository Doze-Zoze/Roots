using Microsoft.Xna.Framework;
using RootsCore;
using Terraria;
using Terraria.GameContent;

namespace RootsBeta.NPCs
{
    public class AngryTrapper(NPC npc) : AIOverride(npc)
    {
        #region Balancing Stats
        private static float BeginChargingThreshold => 640;

        private static float IdleVineLength => 160;

        private static float StopChargingThreshold => 200f;
        private static float BaseMovementSpeed => 0.25f;
        private static float DashSpeed => 32f;
        private static float Deceleration => 0.98f;

        private static float DashCooldown => 60f;
        #endregion

        #region AI
        public override bool PreAI()
        {
            return true;
        }

        public override void AI()
        {
            if (!IsInWorld(VinePos) || AttachPoint == null)
            {
                return;
            }
            if (!AttachPoint.HasTile)
            {
                NPC.life = -1;
                NPC.HitEffect();
                NPC.active = false;
                return;
            }
            FixExploitManEaters.ProtectSpot(VinePos.X, VinePos.Y);
            NPC.TargetClosest();
            if (!NPC.HasValidTarget)
                return;

            Vector2 toPlayer = NPC.DirectionTo(Player.Center);
            Vector2 toPlayerFromVine = WorldVinePos.DirectionTo(Player.Center);
            float playerVineDis = WorldVinePos.Distance(Player.Center);
            float trapperVineDis = WorldVinePos.Distance(NPC.Center);

            if (NPC.ai[2] == 0 && playerVineDis < BeginChargingThreshold)
            {
                NPC.ai[2] = 1;
                NPC.velocity += DashSpeed * toPlayer;
            }

            if (trapperVineDis < StopChargingThreshold && NPC.ai[2]++ > DashCooldown)
                NPC.ai[2] = 0;

            float vineLength = IdleVineLength;
            if (NPC.ai[2] == 1)
                vineLength = playerVineDis;
            
            NPC.velocity += BaseMovementSpeed * (NPC.DirectionTo(WorldVinePos + toPlayerFromVine * vineLength));
            NPC.velocity *= Deceleration;
            NPC.rotation = (toPlayer + toPlayerFromVine * 2f).ToRotation() + MathHelper.Pi;
        }
        #endregion

        #region Helpers

        private Point VinePos => new((int)NPC.ai[0], (int)NPC.ai[1]);

        private Vector2 WorldVinePos => VinePos.ToWorldCoordinates();
        private Tile AttachPoint => Main.tile[VinePos];

        private Player Player => Main.player[NPC.target];

        private static bool IsInWorld(Point pos)
        {
            return pos.X >= 0 && pos.X < Main.maxTilesX && pos.Y >= 0 && pos.Y < Main.maxTilesY;
        }
        #endregion
    }
}
