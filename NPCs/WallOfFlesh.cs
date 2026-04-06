using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using rail;
using RootsBeta.Utilities;
using RootsCore;
using Steamworks;
using System;
using System.Linq;
using System.Threading;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics.CameraModifiers;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using static RootsBeta.NPCs.WoFMouth;

namespace RootsBeta.NPCs;

public class WoFCameraModifier : ICameraModifier
{

    public string UniqueIdentity => "WofCameraMod";

    public bool Finished => intensity == 0 && !Main.npc.Any(x => x.active && x.type == NPCID.WallofFlesh);

    float intensity = 0;
    Vector2 goalScreenPos = default;

    public void Update(ref CameraInfo cameraPosition)
    {
        if (goalScreenPos == default)
            goalScreenPos = Main.screenPosition;
        if (Main.npc.Any(x => x.active && x.type == NPCID.WallofFlesh))
        {
            intensity += 0.025f;
            var WoF = Main.npc.First(x => x.active && x.type == NPCID.WallofFlesh);
            var off = WoF.direction * 160f;
            goalScreenPos.X = WoF.Center.X - off;
            goalScreenPos.Y = MathHelper.Lerp(WoF.Center.Y, Main.LocalPlayer.Center.Y, 0.25f) - Main.screenHeight / Main.GameViewMatrix.Zoom.Y * 0.5f;
        }
        else
        {
            intensity -= 0.025f;
        }
        intensity = MathHelper.Clamp(intensity, 0, 1);
        if (intensity == 0)
            goalScreenPos = default;
        else
        {
            cameraPosition.CameraPosition += (goalScreenPos - Main.GameViewMatrix.Translation - Main.screenPosition) * intensity;
        }
    }
}
public class WoFCameraSystem : ModSystem
{
    Vector2 goalScreenPos;
    bool active = false;
    public override void ModifyScreenPosition()
    {
        return;
        var lastgoal = goalScreenPos;
        if (false && Main.npc.Any(x => x.active && x.type == NPCID.WallofFlesh))
        {
            var WoF = Main.npc.First(x => x.active && x.type == NPCID.WallofFlesh);
            goalScreenPos.X = WoF.Center.X - (WoF.direction == -1 ? Main.screenWidth * .79f : Main.screenWidth * 0.21f);
            Console.WriteLine((goalScreenPos.X - WoF.Center.X) / Main.screenWidth);
            goalScreenPos.Y = MathHelper.Lerp(WoF.Center.Y - Main.screenHeight * 0.5f, Main.LocalPlayer.Center.Y - Main.screenHeight * 0.5f, 0.25f);
            active = true;
        }
        else
        {
            goalScreenPos = Main.screenPosition + new Vector2(Main.screenWidth / 4f, 0);
            active = true;
        }
        if (active)
        {
            var offset = goalScreenPos - Main.screenPosition;
            var a = offset.X / (Main.screenWidth) * 2;
            Main.screenPosition = Vector2.Lerp(Main.screenLastPosition, goalScreenPos - new Vector2(a * Main.GameViewMatrix.Translation.X, 0), 0.1f);
            if (Main.screenPosition.Distance(goalScreenPos) < 4)
            {
                active = false;
            }
        }
        Console.WriteLine(Main.screenPosition);
        //Main.screenPosition.X += Main.screenWidth/2f - Main.GameViewMatrix.Translation.X;
        base.ModifyScreenPosition();
    }
}

public class WoFMouth : AIOverride
{
    public WoFMouth(NPC npc) : base(npc) { }

    public enum WoFAttackState
    {
        LaserBarrage,
        Leeches,
        Hungry,
        Dashing
    }

    public ref float SpawnState => ref NPC.localAI[0];
    [Obsolete]
    public ref float LeechTimer => ref NPC.ai[1];
    [Obsolete]
    public ref float LeechCounter => ref NPC.ai[2];
    public ref float DespawnCounter => ref NPC.localAI[1];
    public ref float SoundCounter => ref NPC.localAI[3];

    public float HealthPercentage => NPC.life / (float)NPC.lifeMax;

    float Speed = 0;

    public ref float Timer => ref NPC.ai[0];
    public WoFAttackState CurrentAttack
    {
        get => (WoFAttackState)(NPC.ai[1]);
        set => NPC.ai[1] = (float)value;
    }
    Player target => (Main.player[NPC.target]?.dead ?? true) ? null : Main.player[NPC.target];

    public Rectangle? TargetArea => new((int)(NPC.Center.X - NPC.direction * 160), (int)(MathHelper.Lerp(NPC.Center.Y, target.Center.Y, 0.25f) - 1200f * 0.5f), 1920, 1200);

    WoFCameraModifier cameraMod = new();
    public override void AI()
    {
        //Despawn at world end
        if (NPC.position.X < 160f || NPC.Right.X > (float)((Main.maxTilesX - 10) * 16))
        {
            NPC.active = false;
            return;
        }
        Main.instance.CameraModifiers.Add(cameraMod);
        if (NPC.localAI[0] == 0f)
        {
            NPC.localAI[0] = 1f;
            Main.wofDrawAreaBottom = -1;
            Main.wofDrawAreaTop = -1;
        }
        CurrentAttack = WoFAttackState.Hungry;
        ResetVars();
        VerticalPositioning();
        DoAttacks();
        HorizontalVelocity();
        DespawnCheck();
        SpawnEyeChecks();

        if (target is not null)
        {
            NPC.rotation = Utils.AngleLerp(NPC.rotation, MathHelper.Clamp(NPC.DirectionTo(target.Center).ToRotation(), -0.75f, 0.75f), 0.1f);
            if (!target.Hitbox.Intersects(TargetArea.Value))
            {
                target.AddBuff(BuffID.TheTongue, 60);
            }

        }

    }

    void DoAttacks()
    {
        switch (CurrentAttack)
        {
            case WoFAttackState.LaserBarrage:
                {
                    Speed = 1.5f;
                    if (Timer > 300)
                        SwitchAttack(WoFAttackState.Dashing);
                    return;
                }

            case WoFAttackState.Leeches:
                {
                    return;
                }

            case WoFAttackState.Hungry:
                {
                    return;
                }

            case WoFAttackState.Dashing:
                {
                    float dashStartup = 30 + 150 * (1 - NPC.life / (float)NPC.lifeMax);
                    float dashDuration = 120 - 60 * (1 -NPC.life / (float)NPC.lifeMax);
                    float dashCooldown = 60;
                    if (Timer < dashStartup) 
                        Speed = 0;
                    else if (Timer < dashStartup + dashDuration)
                        Speed = 8 + 14 * (1 - NPC.life / (float)NPC.lifeMax);
                    if (Timer > dashStartup + dashDuration + dashCooldown)        
                        SwitchAttack(WoFAttackState.LaserBarrage);
                    if (Timer == dashStartup)
                        SoundEngine.PlaySound(SoundID.NPCDeath10, NPC.Center);
                    return;
                }
        }
    }

    void SwitchAttack(WoFAttackState attack)
    {
        CurrentAttack = attack;
        Timer = 0;
    }
    void ResetVars()
    {
        Speed = 1f;
        Timer++;
    }
    void HorizontalVelocity()
    {
        if (SpawnState < 2)
        {
            NPC.TargetClosest();
            if (Main.player[NPC.target].dead)
            {
                float num370 = float.PositiveInfinity;
                int num371 = 0;
                for (int num372 = 0; num372 < 255; num372++)
                {
                    Player player = Main.player[NPC.target];
                    if (player.active)
                    {
                        float num373 = NPC.Distance(player.Center);
                        if (num370 > num373)
                        {
                            num370 = num373;
                            num371 = ((NPC.Center.X < player.Center.X) ? 1 : (-1));
                        }
                    }
                }
                NPC.direction = num371;
            }
            NPC.velocity.X = NPC.direction;
        }
        NPC.velocity.X = NPC.direction * Speed;
        NPC.spriteDirection = NPC.direction;
    }
    void DespawnCheck()
    {

        if (Main.player[NPC.target].dead || !Main.player[NPC.target].gross)
        {
            NPC.TargetClosest_WOF();
        }
        if (Main.player[NPC.target].dead)
        {
            NPC.localAI[1] += 1f / 180f;
            if (NPC.localAI[1] >= 1f)
            {
                SoundEngine.PlaySound(SoundID.NPCDeath10, NPC.Center);
                NPC.life = 0;
                NPC.active = false;
                if (Main.netMode != 1)
                {
                    NetMessage.SendData(28, -1, -1, null, NPC.whoAmI, -1f);
                }
                return;
            }
        }
        else
        {
            NPC.localAI[1] = MathHelper.Clamp(NPC.localAI[1] - 1f / 30f, 0f, 1f);
        }
    }
    void VerticalPositioning()
    {
        /// Based on vanilla code.
        int maxHeight = Main.UnderworldLayer + 10;
        int minHeight = maxHeight + 70;
        Main.wofNPCIndex = NPC.whoAmI;
        int left_x_tilepos = (int)(NPC.position.X / 16f);
        int right_x_tilepos = (int)(NPC.Right.X / 16f);
        int center_y_tilepos = (int)(NPC.Center.Y / 16f);
        int loopvar = 0;
        int HoverPosition = center_y_tilepos + 7;
        while (loopvar < 15 && HoverPosition > Main.UnderworldLayer)
        {
            HoverPosition++;
            if (HoverPosition > Main.maxTilesY - 10)
            {
                HoverPosition = Main.maxTilesY - 10;
                break;
            }
            if (HoverPosition < maxHeight)
            {
                continue;
            }
            for (int num366 = left_x_tilepos; num366 <= right_x_tilepos; num366++)
            {
                try
                {
                    if (WorldGen.InWorld(num366, HoverPosition, 2) && (WorldGen.SolidTile(num366, HoverPosition) || Main.tile[num366, HoverPosition].LiquidAmount > 0))
                    {
                        loopvar++;
                    }
                }
                catch
                {
                    loopvar += 15;
                }
            }
        }
        HoverPosition += 4;
        if (Main.wofDrawAreaBottom == -1)
        {
            Main.wofDrawAreaBottom = HoverPosition * 16;
        }
        else if (Main.wofDrawAreaBottom > HoverPosition * 16)
        {
            Main.wofDrawAreaBottom--;
            if (Main.wofDrawAreaBottom < HoverPosition * 16)
            {
                Main.wofDrawAreaBottom = HoverPosition * 16;
            }
        }
        else if (Main.wofDrawAreaBottom < HoverPosition * 16)
        {
            Main.wofDrawAreaBottom++;
            if (Main.wofDrawAreaBottom > HoverPosition * 16)
            {
                Main.wofDrawAreaBottom = HoverPosition * 16;
            }
        }
        loopvar = 0;
        HoverPosition = center_y_tilepos - 7;
        while (loopvar < 15 && HoverPosition < Main.maxTilesY - 10)
        {
            HoverPosition--;
            if (HoverPosition <= 10)
            {
                HoverPosition = 10;
                break;
            }
            if (HoverPosition > minHeight)
            {
                continue;
            }
            if (HoverPosition < maxHeight)
            {
                HoverPosition = maxHeight;
                break;
            }
            for (int num367 = left_x_tilepos; num367 <= right_x_tilepos; num367++)
            {
                try
                {
                    if (WorldGen.InWorld(num367, HoverPosition, 2) && (WorldGen.SolidTile(num367, HoverPosition) || Main.tile[num367, HoverPosition].LiquidAmount > 0))
                    {
                        loopvar++;
                    }
                }
                catch
                {
                    loopvar += 15;
                }
            }
        }
        HoverPosition -= 4;
        if (Main.wofDrawAreaTop == -1)
        {
            Main.wofDrawAreaTop = HoverPosition * 16;
        }
        else if (Main.wofDrawAreaTop > HoverPosition * 16)
        {
            Main.wofDrawAreaTop--;
            if (Main.wofDrawAreaTop < HoverPosition * 16)
            {
                Main.wofDrawAreaTop = HoverPosition * 16;
            }
        }
        else if (Main.wofDrawAreaTop < HoverPosition * 16)
        {
            Main.wofDrawAreaTop++;
            if (Main.wofDrawAreaTop > HoverPosition * 16)
            {
                Main.wofDrawAreaTop = HoverPosition * 16;
            }
        }
        Main.wofDrawAreaTop = (int)MathHelper.Clamp(Main.wofDrawAreaTop, (float)maxHeight * 16f, (float)minHeight * 16f);
        Main.wofDrawAreaBottom = (int)MathHelper.Clamp(Main.wofDrawAreaBottom, (float)maxHeight * 16f, (float)minHeight * 16f);
        if (Main.wofDrawAreaTop > Main.wofDrawAreaBottom - 160)
        {
            Main.wofDrawAreaTop = Main.wofDrawAreaBottom - 160;
        }
        else if (Main.wofDrawAreaBottom < Main.wofDrawAreaTop + 160)
        {
            Main.wofDrawAreaBottom = Main.wofDrawAreaTop + 160;
        }
        NPC.position.Y = (Main.wofDrawAreaBottom + Main.wofDrawAreaTop) / 2 - NPC.height / 2;
    }
    void SpawnEyeChecks()
    {
        if (Main.netMode != 1 && NPC.localAI[0] == 1f)
        {
            NPC.localAI[0] = 2f;
            float num386 = (NPC.Center.Y + (float)Main.wofDrawAreaTop) / 2f;
            int num387 = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.position.X, (int)num386, 114, NPC.whoAmI, 1f);
            float num388 = (NPC.Center.Y + (float)Main.wofDrawAreaBottom) / 2f;
            num387 = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.position.X, (int)num388, 114, NPC.whoAmI, -1f);
            float num389 = (NPC.Center.Y + (float)Main.wofDrawAreaBottom) / 2f;
            for (int num390 = 0; num390 < 11; num390++)
            {
                num387 = NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.position.X, (int)num389, 115, NPC.whoAmI, (float)num390 * 0.1f - 0.05f);
            }
        }
    }
}

public class WoFEye : AIOverride
{
    public WoFEye(NPC npc) : base(npc) { }

    public bool isTop => NPC.ai[0] >= 0;

    public ref float LaserTimer => ref NPC.localAI[1];
    public ref float LaserCount => ref NPC.localAI[2];

    Player target => (Main.player[NPC.target]?.dead ?? true) ? null : Main.player[NPC.target];

    WoFMouth Mouth => Main.npc[Main.wofNPCIndex].TryGetGlobalNPC<AIOverrideSystem>(out var x) ? x.CurrentAiOverride as WoFMouth : null;
    public override void AI()
    {
        if (Main.wofNPCIndex < 0 || Mouth is null)
        {
            NPC.active = false;
            return;
        }
        NPC.realLife = Main.wofNPCIndex;
        if (Main.npc[Main.wofNPCIndex].life > 0)
        {
            NPC.life = Main.npc[Main.wofNPCIndex].life;
        }
        NPC.target = Mouth.NPC.target;

        NPC.direction = Main.npc[Main.wofNPCIndex].direction;
        NPC.spriteDirection = NPC.direction;

        NPC.position.X = Mouth.NPC.position.X;
        //NPC.Center = new(Main.npc[Main.wofNPCIndex].Center.X, (Main.npc[Main.wofNPCIndex].Center.Y + (isTop ? Main.wofDrawAreaTop : Main.wofDrawAreaBottom)) / 2f);
        if (target is not null)
        {
            NPC.rotation = Utils.AngleLerp(NPC.rotation, MathHelper.Clamp(NPC.DirectionTo(target.Center).ToRotation(), -0.75f, 0.75f), 0.1f); 
            DoAttacks();
        }
    }

    void DoAttacks()
    {
        switch (Mouth.CurrentAttack)
        {
            case WoFAttackState.LaserBarrage:
                {
                    if (Mouth.HealthPercentage > 0.5f)
                    {
                        var center = NPC.Center;
                        var area = Mouth.TargetArea.Value;
                        center.Y = MathHelper.Lerp(center.Y, float.Lerp(area.Y, area.Y + area.Height, RootsUtils.Sine0to1(Mouth.Timer * 0.033f + (isTop ? 0 : MathHelper.Pi)) * 0.75f + 0.125f), 0.1f);
                        NPC.Center = center;
                        float interval = (Mouth.HealthPercentage > 0.75f ? 20 : 15);
                        if ((isTop ? LaserTimer : LaserTimer + (int)(interval*0.5f)) % (Mouth.HealthPercentage > 0.75f ? 20 : 15) == 0)
                        {
                            Projectile.NewProjectileDirect(NPC.GetSource_FromThis(), NPC.Center, Vector2.UnitX * NPC.direction * 5, ProjectileID.EyeLaser, 100, 1).tileCollide = false;
                        }

                        NPC.rotation = 0;
                    }
                    else
                    {

                        var center = NPC.Center;
                        var area = Mouth.TargetArea.Value;
                        center.Y = MathHelper.Lerp(center.Y, float.Lerp(area.Y, area.Y + area.Height, RootsUtils.Sine0to1(Mouth.Timer * 0.06f + (isTop ? 0 : MathHelper.TwoPi/3)) * 0.75f + 0.125f), 0.1f);
                        
                        float interval = (Mouth.HealthPercentage > 0.25f ? 120 : 90);
                        if ((isTop ? LaserTimer : LaserTimer + (int)(interval*0.5f)) % interval <= 30 && LaserTimer % 6 == 0)
                        {
                            Projectile.NewProjectileDirect(NPC.GetSource_FromThis(), NPC.Center, Vector2.UnitX * NPC.direction * 5, ProjectileID.EyeLaser, 100, 1).tileCollide = false;
                        }
                        NPC.Center = center;
                    }
                    LaserTimer++;
                    return;
                }

            default:
                {
                    NPC.position.Y = MathHelper.Lerp(NPC.position.Y,(Main.npc[Main.wofNPCIndex].Center.Y + (isTop ? Main.wofDrawAreaTop : Main.wofDrawAreaBottom)) / 2f - NPC.height/2,0.1f);
                    return;
                }
        }
    }
}

public class HungryAttached : AIOverride
{
    public HungryAttached(NPC npc) : base(npc) { }

    #region Balancing Stats

    static float BeginChargingThreshold => 300;

    static float IdleVineLength => 320;

    static float StopChargingThreshold => 300;
    static float BaseMovementSpeed => 0.25f;
    #endregion

    #region AI
    public override bool PreAI()
    {
        return true;
    }

    public override void AI()
    {   
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
            vineLength = playerVineDis;

        if (NPC.Distance(worldVinePos) > IdleVineLength)
            NPC.Center = worldVinePos + NPC.DirectionFrom(worldVinePos) * IdleVineLength;


        NPC.velocity += BaseMovementSpeed * (NPC.DirectionTo(worldVinePos + toPlayerFromVine * vineLength));
        NPC.velocity *= 0.98f;
        NPC.rotation = (toPlayer + toPlayerFromVine * 2f).ToRotation() + MathHelper.Pi;
        NPC.position += Mouth.NPC.velocity;
    }
    #endregion

    #region Helpers
    WoFMouth Mouth => Main.npc[Main.wofNPCIndex].TryGetGlobalNPC<AIOverrideSystem>(out var x) ? x.CurrentAiOverride as WoFMouth : null;

    Vector2 worldVinePos =>  new Vector2(Main.npc[Main.wofNPCIndex].Center.X, Mouth.TargetArea.Value.Y + MathHelper.Lerp(1200f * 0.1f,1200f * 0.9f, NPC.ai[0]));

    Player player => Main.player[NPC.target];
    #endregion

}

public class HungryDetachedGlobal : GlobalNPC
{
    public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
    {
        return entity.type == NPCID.TheHungry;
    }
    public override bool PreDraw(NPC npc, SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
    {
        return base.PreDraw(npc, spriteBatch, screenPos, drawColor);
    }

    public override void OnKill(NPC npc)
    {
        Main.npc[Main.wofNPCIndex].SimpleStrikeNPC(npc.lifeMax,0);
        base.OnKill(npc);
    }

    public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers)
    {
        if (projectile.penetrate != 1 && Main.player[projectile.owner].Distance(npc.Center) > 16 * 10)
            modifiers.SourceDamage *= 0.25f;
    }
}

public class HungryDetached : AIOverride
{
    public HungryDetached(NPC npc) : base(npc) { }

    public override void AI()
    {
        base.AI();
    }
}

/* Vanilla WoF Code Variable Analysis

-- Mouth Variables --
localAI[0] - Check for spawning eyes and resetting Main.wofDrawArea ints to -1.
    0 - Just spawned
    1 - Has set draw areas to -1
    2 - Has spawned eyes / starting hungry

ai[1] - Timer for leeches

ai[2] - Leech count in current volley

localAi[1] - Timer for despawn. Despawns at 1f.

localAI[3] - Making NPCDeath10 sounds

Hungry Spawning is done entirely by random checks based on number of attached hungry.

-- Eye Variables -- 
ai[0] - Whether eye is top or bottom.
    < 0 (-1) - bottom
    >= 0 (1) - top

localAi[1] - Timer for laser beams.

localAi[2] - Laser beam amount fired in the current volley


See line 19359 in NPC.cs












































        else if (aiStyle == 28)
        {
            Vector2 vector39 = new Vector2(position.X + (float)width * 0.5f, position.Y + (float)height * 0.5f);
            float num392 = Main.player[target].position.X + (float)(Main.player[target].width / 2) - vector39.X;
            float num393 = Main.player[target].position.Y + (float)(Main.player[target].height / 2) - vector39.Y;
            float num394 = (float)Math.Sqrt(num392 * num392 + num393 * num393);
            float num395 = num394;
            num392 *= num394;
            num393 *= num394;
            bool flag27 = true;
            if (direction > 0)
            {
                if (Main.player[target].position.X + (float)(Main.player[target].width / 2) > position.X + (float)(width / 2))
                {
                    rotation = (float)Math.Atan2(0f - num393, 0f - num392) + 3.14f;
                }
                else
                {
                    rotation = 0f;
                    flag27 = false;
                }
            }
            else if (Main.player[target].position.X + (float)(Main.player[target].width / 2) < position.X + (float)(width / 2))
            {
                rotation = (float)Math.Atan2(num393, num392) + 3.14f;
            }
            else
            {
                rotation = 0f;
                flag27 = false;
            }
            if (Main.netMode == 1)
            {
                return;
            }
            int num396 = 4;
            localAI[1] += 1f;
            if ((double)Main.npc[Main.wofNPCIndex].life < (double)Main.npc[Main.wofNPCIndex].lifeMax * 0.75)
            {
                localAI[1] += 1f;
                num396++;
            }
            if ((double)Main.npc[Main.wofNPCIndex].life < (double)Main.npc[Main.wofNPCIndex].lifeMax * 0.5)
            {
                localAI[1] += 1f;
                num396++;
            }
            if ((double)Main.npc[Main.wofNPCIndex].life < (double)Main.npc[Main.wofNPCIndex].lifeMax * 0.25)
            {
                localAI[1] += 1f;
                num396 += 2;
            }
            if ((double)Main.npc[Main.wofNPCIndex].life < (double)Main.npc[Main.wofNPCIndex].lifeMax * 0.1)
            {
                localAI[1] += 2f;
                num396 += 3;
            }
            if (Main.expertMode)
            {
                localAI[1] += 0.5f;
                num396++;
                if ((double)Main.npc[Main.wofNPCIndex].life < (double)Main.npc[Main.wofNPCIndex].lifeMax * 0.1)
                {
                    localAI[1] += 2f;
                    num396 += 3;
                }
            }
            if (localAI[2] == 0f)
            {
                if (localAI[1] > 600f)
                {
                    localAI[2] = 1f;
                    localAI[1] = 0f;
                }
            }
            else
            {
                if (!(localAI[1] > 45f) || !Collision.CanHit(position, width, height, Main.player[target].position, Main.player[target].width, Main.player[target].height))
                {
                    return;
                }
                localAI[1] = 0f;
                localAI[2] += 1f;
                if (localAI[2] >= (float)num396)
                {
                    localAI[2] = 0f;
                }
                if (flag27)
                {
                    float num397 = 9f;
                    int num398 = 11;
                    int num399 = 83;
                    if ((double)Main.npc[Main.wofNPCIndex].life < (double)Main.npc[Main.wofNPCIndex].lifeMax * 0.5)
                    {
                        num398++;
                        num397 += 1f;
                    }
                    if ((double)Main.npc[Main.wofNPCIndex].life < (double)Main.npc[Main.wofNPCIndex].lifeMax * 0.25)
                    {
                        num398++;
                        num397 += 1f;
                    }
                    if ((double)Main.npc[Main.wofNPCIndex].life < (double)Main.npc[Main.wofNPCIndex].lifeMax * 0.1)
                    {
                        num398 += 2;
                        num397 += 2f;
                    }
                    vector39 = new Vector2(position.X + (float)width * 0.5f, position.Y + (float)height * 0.5f);
                    num392 = Main.player[target].position.X + (float)Main.player[target].width * 0.5f - vector39.X;
                    num393 = Main.player[target].position.Y + (float)Main.player[target].height * 0.5f - vector39.Y;
                    num394 = (float)Math.Sqrt(num392 * num392 + num393 * num393);
                    num394 = num397 / num394;
                    num392 *= num394;
                    num393 *= num394;
                    vector39.X += num392;
                    vector39.Y += num393;
                    int num400 = Projectile.NewProjectile(GetSpawnSource_ForProjectile(), vector39.X, vector39.Y, num392, num393, num399, num398, 0f, Main.myPlayer);
                }
            }
        }
        

*/