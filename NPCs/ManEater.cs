using Microsoft.Xna.Framework;
using Roots.Config;
using RootsCore;
using Terraria;
using Terraria.GameContent;

namespace RootsBeta.NPCs
{
    public class ManEater(NPC npc) : AIOverride(npc), IConfigurableContent<ManEater>
    {
        public static ConfigGroup ConfigGroups => ConfigGroup.EnemyReworks;
        #region Balancing Stats
        private static float BeginChargingThreshold => 640;

        private static float IdleVineLength => 240;

        private static float StopChargingThreshold => 800f;
        private static float BaseMovementSpeed => 0.075f;
        private static float Deceleration => 0.98f;
        #endregion

        #region AI
        public override void AI()
        {
            if (!IsInWorld(VinePos) || AttachPoint == null || !AttachPoint.HasTile)
            {
                return;
            }
            FixExploitManEaters.ProtectSpot(VinePos.X, VinePos.Y);
            NPC.TargetClosest();
            if (!NPC.HasValidTarget)
                return;

            Vector2 toPlayer = NPC.DirectionTo(Player.Center);
            Vector2 toPlayerFromVine = WorldVinePos.DirectionTo(Player.Center);
            float playerVineDis = WorldVinePos.Distance(Player.Center);

            if (NPC.ai[2] == 0 && playerVineDis < BeginChargingThreshold)
                NPC.ai[2] = 1;

            if (NPC.ai[2] == 1 && playerVineDis > StopChargingThreshold)
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
