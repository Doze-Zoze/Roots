using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Roots.Config;
using RootsBeta.Utilities;
using RootsCore;
using System;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.CameraModifiers;
using Terraria.ID;
using Terraria.ModLoader;
using static RootsBeta.NPCs.WoFMouth;

namespace RootsBeta.NPCs;

public class WoFCameraModifier : ICameraModifier
{

    public string UniqueIdentity => "WofCameraMod";

    public bool Finished => _intensity == 0 && !Main.npc.Any(x => x.active && x.type == NPCID.WallofFlesh);

    private float _intensity;
    private Vector2 _goalScreenPos;

    public void Update(ref CameraInfo cameraPosition)
    {
        if (_goalScreenPos == default)
            _goalScreenPos = Main.screenPosition;
        if (Main.npc.Any(x => x.active && x.type == NPCID.WallofFlesh))
        {
            _intensity += 0.025f;
            var wof = Main.npc.First(x => x.active && x.type == NPCID.WallofFlesh);
            var off = wof.direction * 160f;
            _goalScreenPos.X = wof.Center.X - off;
            _goalScreenPos.Y = MathHelper.Lerp(wof.Center.Y, Main.LocalPlayer.Center.Y, 0.25f) - Main.screenHeight / Main.GameViewMatrix.Zoom.Y * 0.5f;
        }
        else
        {
            _intensity -= 0.025f;
        }
        _intensity = MathHelper.Clamp(_intensity, 0, 1);
        if (_intensity == 0)
            _goalScreenPos = default;
        else
        {
            cameraPosition.CameraPosition += (_goalScreenPos - Main.GameViewMatrix.Translation - Main.screenPosition) * _intensity;
        }
    }
}

public class WoFCameraSystem : ModSystem
{
    private Vector2 _goalScreenPos;
    private bool _active;
    public override void ModifyScreenPosition()
    {
        return;
        var lastgoal = _goalScreenPos;
        if (false && Main.npc.Any(x => x.active && x.type == NPCID.WallofFlesh))
        {
            var wof = Main.npc.First(x => x.active && x.type == NPCID.WallofFlesh);
            _goalScreenPos.X = wof.Center.X - (wof.direction == -1 ? Main.screenWidth * .79f : Main.screenWidth * 0.21f);
            Console.WriteLine((_goalScreenPos.X - wof.Center.X) / Main.screenWidth);
            _goalScreenPos.Y = MathHelper.Lerp(wof.Center.Y - Main.screenHeight * 0.5f, Main.LocalPlayer.Center.Y - Main.screenHeight * 0.5f, 0.25f);
            _active = true;
        }
        else
        {
            _goalScreenPos = Main.screenPosition + new Vector2(Main.screenWidth / 4f, 0);
            _active = true;
        }
        if (_active)
        {
            var offset = _goalScreenPos - Main.screenPosition;
            var a = offset.X / (Main.screenWidth) * 2;
            Main.screenPosition = Vector2.Lerp(Main.screenLastPosition, _goalScreenPos - new Vector2(a * Main.GameViewMatrix.Translation.X, 0), 0.1f);
            if (Main.screenPosition.Distance(_goalScreenPos) < 4)
            {
                _active = false;
            }
        }
        Console.WriteLine(Main.screenPosition);
        //Main.screenPosition.X += Main.screenWidth/2f - Main.GameViewMatrix.Translation.X;
        base.ModifyScreenPosition();
    }
}

public class WoFMouth(NPC npc) : AIOverride(npc), IConfigurableContent
{
    public string? ConfigName => "Wall Of Flesh";   
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

    private float _speed;

    public ref float Timer => ref NPC.ai[0];
    public WoFAttackState CurrentAttack
    {
        get => (WoFAttackState)(NPC.ai[1]);
        set => NPC.ai[1] = (float)value;
    }

    private Player Target => (Main.player[NPC.target]?.dead ?? true) ? null : Main.player[NPC.target];

    public Rectangle? TargetArea => new((int)(NPC.Center.X - NPC.direction * 160), (int)(MathHelper.Lerp(NPC.Center.Y, Target.Center.Y, 0.25f) - 1200f * 0.5f), 1920, 1200);

    private readonly WoFCameraModifier _cameraMod = new();
    public override void AI()
    {
        //Despawn at world end
        if (NPC.position.X < 160f || NPC.Right.X > (Main.maxTilesX - 10) * 16)
        {
            NPC.active = false;
            return;
        }
        Main.instance.CameraModifiers.Add(_cameraMod);
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

        if (Target is null) return;
        NPC.rotation = NPC.rotation.AngleLerp(MathHelper.Clamp(NPC.DirectionTo(Target.Center).ToRotation(), -0.75f, 0.75f), 0.1f);
        if (TargetArea != null && !Target.Hitbox.Intersects(TargetArea.Value))
        {
            Target.AddBuff(BuffID.TheTongue, 60);
        }

    }

    private void DoAttacks()
    {
        switch (CurrentAttack)
        {
            case WoFAttackState.LaserBarrage:
                {
                    _speed = 1.5f;
                    if (Timer > 300)
                        SwitchAttack(WoFAttackState.Dashing);
                    return;
                }

            case WoFAttackState.Leeches:

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
                        _speed = 0;
                    else if (Timer < dashStartup + dashDuration)
                        _speed = 8 + 14 * (1 - NPC.life / (float)NPC.lifeMax);
                    if (Timer > dashStartup + dashDuration + dashCooldown)        
                        SwitchAttack(WoFAttackState.LaserBarrage);
                    if (Timer == dashStartup)
                        SoundEngine.PlaySound(SoundID.NPCDeath10, NPC.Center);
                    return;
                }
        }
    }

    private void SwitchAttack(WoFAttackState attack)
    {
        CurrentAttack = attack;
        Timer = 0;
    }

    private void ResetVars()
    {
        _speed = 1f;
        Timer++;
    }

    private void HorizontalVelocity()
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
                    if (!player.active) continue;
                    float num373 = NPC.Distance(player.Center);
                    if (!(num370 > num373)) continue;
                    num370 = num373;
                    num371 = ((NPC.Center.X < player.Center.X) ? 1 : (-1));
                }
                NPC.direction = num371;
            }
            NPC.velocity.X = NPC.direction;
        }
        NPC.velocity.X = NPC.direction * _speed;
        NPC.spriteDirection = NPC.direction;
    }

    private void DespawnCheck()
    {

        if (Main.player[NPC.target].dead || !Main.player[NPC.target].gross)
        {
            NPC.TargetClosest_WOF();
        }
        if (Main.player[NPC.target].dead)
        {
            NPC.localAI[1] += 1f / 180f;
            if (!(NPC.localAI[1] >= 1f)) return;
            SoundEngine.PlaySound(SoundID.NPCDeath10, NPC.Center);
            NPC.life = 0;
            NPC.active = false;
            if (Main.netMode != NetmodeID.MultiplayerClient)
            {
                NetMessage.SendData(MessageID.DamageNPC, -1, -1, null, NPC.whoAmI, -1f);
            }
            return;
        }
        NPC.localAI[1] = MathHelper.Clamp(NPC.localAI[1] - 1f / 30f, 0f, 1f);
    }

    private void VerticalPositioning()
    {
        // Based on vanilla code.
        int maxHeight = Main.UnderworldLayer + 10;
        int minHeight = maxHeight + 70;
        Main.wofNPCIndex = NPC.whoAmI;
        int leftXTilepos = (int)(NPC.position.X / 16f);
        int rightXTilepos = (int)(NPC.Right.X / 16f);
        int centerYTilepos = (int)(NPC.Center.Y / 16f);
        int loopVar = 0;
        int hoverPosition = centerYTilepos + 7;
        while (loopVar < 15 && hoverPosition > Main.UnderworldLayer)
        {
            hoverPosition++;
            if (hoverPosition > Main.maxTilesY - 10)
            {
                hoverPosition = Main.maxTilesY - 10;
                break;
            }
            if (hoverPosition < maxHeight)
            {
                continue;
            }
            for (int num366 = leftXTilepos; num366 <= rightXTilepos; num366++)
            {
                try
                {
                    if (WorldGen.InWorld(num366, hoverPosition, 2) && (WorldGen.SolidTile(num366, hoverPosition) || Main.tile[num366, hoverPosition].LiquidAmount > 0))
                    {
                        loopVar++;
                    }
                }
                catch
                {
                    loopVar += 15;
                }
            }
        }
        hoverPosition += 4;
        if (Main.wofDrawAreaBottom == -1)
        {
            Main.wofDrawAreaBottom = hoverPosition * 16;
        }
        else if (Main.wofDrawAreaBottom > hoverPosition * 16)
        {
            Main.wofDrawAreaBottom--;
            if (Main.wofDrawAreaBottom < hoverPosition * 16)
            {
                Main.wofDrawAreaBottom = hoverPosition * 16;
            }
        }
        else if (Main.wofDrawAreaBottom < hoverPosition * 16)
        {
            Main.wofDrawAreaBottom++;
            if (Main.wofDrawAreaBottom > hoverPosition * 16)
            {
                Main.wofDrawAreaBottom = hoverPosition * 16;
            }
        }
        loopVar = 0;
        hoverPosition = centerYTilepos - 7;
        while (loopVar < 15 && hoverPosition < Main.maxTilesY - 10)
        {
            hoverPosition--;
            if (hoverPosition <= 10)
            {
                hoverPosition = 10;
                break;
            }
            if (hoverPosition > minHeight)
            {
                continue;
            }
            if (hoverPosition < maxHeight)
            {
                hoverPosition = maxHeight;
                break;
            }
            for (int num367 = leftXTilepos; num367 <= rightXTilepos; num367++)
            {
                try
                {
                    if (WorldGen.InWorld(num367, hoverPosition, 2) && (WorldGen.SolidTile(num367, hoverPosition) || Main.tile[num367, hoverPosition].LiquidAmount > 0))
                    {
                        loopVar++;
                    }
                }
                catch
                {
                    loopVar += 15;
                }
            }
        }
        hoverPosition -= 4;
        if (Main.wofDrawAreaTop == -1)
        {
            Main.wofDrawAreaTop = hoverPosition * 16;
        }
        else if (Main.wofDrawAreaTop > hoverPosition * 16)
        {
            Main.wofDrawAreaTop--;
            if (Main.wofDrawAreaTop < hoverPosition * 16)
            {
                Main.wofDrawAreaTop = hoverPosition * 16;
            }
        }
        else if (Main.wofDrawAreaTop < hoverPosition * 16)
        {
            Main.wofDrawAreaTop++;
            if (Main.wofDrawAreaTop > hoverPosition * 16)
            {
                Main.wofDrawAreaTop = hoverPosition * 16;
            }
        }
        Main.wofDrawAreaTop = (int)MathHelper.Clamp(Main.wofDrawAreaTop, maxHeight * 16f, minHeight * 16f);
        Main.wofDrawAreaBottom = (int)MathHelper.Clamp(Main.wofDrawAreaBottom, maxHeight * 16f, minHeight * 16f);
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

    private void SpawnEyeChecks()
    {
        if (Main.netMode == NetmodeID.MultiplayerClient || NPC.localAI[0] != 1f) return;
        NPC.localAI[0] = 2f;
        float num386 = (NPC.Center.Y + Main.wofDrawAreaTop) / 2f;
        NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.position.X, (int)num386, NPCID.WallofFleshEye, NPC.whoAmI, 1f);
        float num388 = (NPC.Center.Y + Main.wofDrawAreaBottom) / 2f;
        NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.position.X, (int)num388, NPCID.WallofFleshEye, NPC.whoAmI, -1f);
        float num389 = (NPC.Center.Y + Main.wofDrawAreaBottom) / 2f;
        for (int num390 = 0; num390 < 11; num390++)
        {
            NPC.NewNPC(NPC.GetSource_FromAI(), (int)NPC.position.X, (int)num389, NPCID.TheHungry, NPC.whoAmI, num390 * 0.1f - 0.05f);
        }
    }
}

public class WoFEye(NPC npc) : AIOverride(npc)
{
    private bool IsTop => NPC.ai[0] >= 0;

    private ref float LaserTimer => ref NPC.localAI[1];
    public ref float LaserCount => ref NPC.localAI[2];

    private Player Target => (Main.player[NPC.target]?.dead ?? true) ? null : Main.player[NPC.target];

    private static WoFMouth Mouth => Main.npc[Main.wofNPCIndex].TryGetGlobalNPC<AIOverrideSystem>(out var x) ? x.CurrentAiOverride as WoFMouth : null;
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
        if (Target is null) return;
        NPC.rotation = NPC.rotation.AngleLerp(MathHelper.Clamp(NPC.DirectionTo(Target.Center).ToRotation(), -0.75f, 0.75f), 0.1f); 
        DoAttacks();
    }

    private void DoAttacks()
    {
        switch (Mouth.CurrentAttack)
        {
            case WoFAttackState.LaserBarrage:
                {
                    if (Mouth.HealthPercentage > 0.5f)
                    {
                        var center = NPC.Center;
                        if (Mouth.TargetArea != null)
                        {
                            var area = Mouth.TargetArea.Value;
                            center.Y = MathHelper.Lerp(center.Y, float.Lerp(area.Y, area.Y + area.Height, RootsUtils.Sine0To1(Mouth.Timer * 0.033f + (IsTop ? 0 : MathHelper.Pi)) * 0.75f + 0.125f), 0.1f);
                        }

                        NPC.Center = center;
                        float interval = (Mouth.HealthPercentage > 0.75f ? 20 : 15);
                        if ((IsTop ? LaserTimer : LaserTimer + (int)(interval*0.5f)) % (Mouth.HealthPercentage > 0.75f ? 20 : 15) == 0)
                        {
                            Projectile.NewProjectileDirect(NPC.GetSource_FromThis(), NPC.Center, Vector2.UnitX * NPC.direction * 5, ProjectileID.EyeLaser, 100, 1).tileCollide = false;
                        }

                        NPC.rotation = 0;
                    }
                    else
                    {

                        var center = NPC.Center;
                        if (Mouth.TargetArea != null)
                        {
                            var area = Mouth.TargetArea.Value;
                            center.Y = MathHelper.Lerp(center.Y, float.Lerp(area.Y, area.Y + area.Height, RootsUtils.Sine0To1(Mouth.Timer * 0.06f + (IsTop ? 0 : MathHelper.TwoPi/3)) * 0.75f + 0.125f), 0.1f);
                        }

                        float interval = (Mouth.HealthPercentage > 0.25f ? 120 : 90);
                        if ((IsTop ? LaserTimer : LaserTimer + (int)(interval*0.5f)) % interval <= 30 && LaserTimer % 6 == 0)
                        {
                            Projectile.NewProjectileDirect(NPC.GetSource_FromThis(), NPC.Center, Vector2.UnitX * NPC.direction * 5, ProjectileID.EyeLaser, 100, 1).tileCollide = false;
                        }
                        NPC.Center = center;
                    }
                    LaserTimer++;
                    return;
                }

            case WoFAttackState.Leeches:
            case WoFAttackState.Hungry:
            case WoFAttackState.Dashing:
            default:
                {
                    NPC.position.Y = MathHelper.Lerp(NPC.position.Y,(Main.npc[Main.wofNPCIndex].Center.Y + (IsTop ? Main.wofDrawAreaTop : Main.wofDrawAreaBottom)) / 2f - NPC.height/2,0.1f);
                    return;
                }
        }
    }
}

public class HungryAttached(NPC npc) : AIOverride(npc)
{
    #region Balancing Stats
    private static float BeginChargingThreshold => 300;

    private static float IdleVineLength => 320;

    private static float StopChargingThreshold => 300;
    private static float BaseMovementSpeed => 0.25f;
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

        if (NPC.Distance(WorldVinePos) > IdleVineLength)
            NPC.Center = WorldVinePos + NPC.DirectionFrom(WorldVinePos) * IdleVineLength;


        NPC.velocity += BaseMovementSpeed * (NPC.DirectionTo(WorldVinePos + toPlayerFromVine * vineLength));
        NPC.velocity *= 0.98f;
        NPC.rotation = (toPlayer + toPlayerFromVine * 2f).ToRotation() + MathHelper.Pi;
        NPC.position += Mouth.NPC.velocity;
    }
    #endregion

    #region Helpers

    private static WoFMouth Mouth => Main.npc[Main.wofNPCIndex].TryGetGlobalNPC<AIOverrideSystem>(out var x) ? x.CurrentAiOverride as WoFMouth : null;

    private Vector2 WorldVinePos => new(Main.npc[Main.wofNPCIndex].Center.X, Mouth.TargetArea.Value.Y + MathHelper.Lerp(1200f * 0.1f,1200f * 0.9f, NPC.ai[0]));

    private Player Player => Main.player[NPC.target];
    #endregion

}

public class HungryDetachedGlobal : GlobalNPC
{
    public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
    {
        return entity.type == NPCID.TheHungry;
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

public class HungryDetached(NPC npc) : AIOverride(npc)
{
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