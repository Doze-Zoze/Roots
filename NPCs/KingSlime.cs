using Microsoft.Xna.Framework;
using System;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.UI;

namespace RootsBeta.NPCs
{
    public class KingSlime : AIOverride
    {
        public KingSlime(NPC npc) : base(npc) { }

        #region Balancing Stats

        #endregion

        #region AI
        public override bool PreAI()
        {
            return true;
        }

        public override void AI()
        {
            if (Math.Abs(NPC.velocity.Y) == 0)
            {
                NPC.velocity.X *= 0.8f;
                if (JumpTimer > 0)
                    JumpTimer--;
            }
            NPC.TargetClosest();
            float liferatio = NPC.life / (float)NPC.lifeMax;
            float num255 = liferatio;
            num255 = num255 * 0.5f + 0.75f;
            if (num255 != NPC.scale)
            {
                NPC.position.X += NPC.width / 2;
                NPC.position.Y += NPC.height;
                NPC.scale = num255;
                NPC.width = (int)(98f * NPC.scale);
                NPC.height = (int)(92f * NPC.scale);
                NPC.position.X -= NPC.width / 2;
                NPC.position.Y -= NPC.height;
            }
            if (!NPC.HasValidTarget)
            {
                JumpCounter = 5;

                NPC.scale *= (30 - (TeleportCounter - 6)) / 30f;
                TeleportCounter++;
                if (TeleportCounter > 36)
                {

                    Gore.NewGore(NPC.GetSource_FromThis(), NPC.Center + new Vector2(-40f, -NPC.height / 2), NPC.velocity, 734);
                    NPC.active = false;
                    return;

                }
                for (var i = 0; i < 10; i++)
                {
                    int num254 = Dust.NewDust(NPC.position, NPC.width, NPC.height, 4, NPC.velocity.X, NPC.velocity.Y, 150, new Color(0, 80, 255, 80), 2f);
                    Main.dust[num254].noGravity = true;
                    var dust = Main.dust[num254];
                    dust.velocity *= 0.5f;
                }
                return;
            }
            int dir = (player.Center.X - NPC.Center.X) > 0 ? 1 : -1;
            if (NPC.velocity.X == 0 && !(NPC.velocity.Y == 0))
                NPC.velocity.X += dir;
            if (JumpTimer <= 0)
                switch (JumpCounter)
                {
                    case 0:
                        {
                            NPC.velocity += new Vector2(3 * dir, -10);
                            JumpTimer = 15 + liferatio * 30;
                            JumpCounter++;
                            TeleportCounter++;
                        }
                        break;
                    case 1:
                        {
                            NPC.velocity += new Vector2(5 * dir, -8);
                            JumpTimer = 10 + liferatio * 10;
                            JumpCounter = dir == -1 ? 2 : 3;
                            TeleportCounter++;
                        }
                        break;
                    case 2:
                        {
                            NPC.velocity += new Vector2(-8, -5);
                            JumpTimer = 5;
                            JumpCounter = 4;
                        }
                        break;
                    case 3:
                        {
                            NPC.velocity += new Vector2(8, -5);
                            JumpTimer = 5;
                            JumpCounter = 4;
                        }
                        break;
                    case 4:
                        {
                            for (var i = 0; i < (4 + MathF.Round(-2 * liferatio)) ; i++)
                            {
                                var slime = NPC.NewNPCDirect(NPC.GetSource_FromThis(), (int)NPC.Center.X, (int)NPC.Center.Y, NPCID.SlimeSpiked, ai0: 30, ai1: -1);
                                slime.velocity += new Vector2(0, -10).RotatedByRandom(1);
                                slime.lifeMax = (int)(slime.lifeMax * 0.33f);
                                slime.life = (int)(slime.lifeMax * liferatio);
                                if (slime.life < 1)
                                    slime.life = 1;
                                NPC.life -= slime.life;
                                    if (NPC.life < 1)
                                        NPC.life = 1;
                                slime.netUpdate = true;
                                NPC.netUpdate = true;
                                int num254 = Dust.NewDust(slime.position, slime.width, slime.height, 4, slime.velocity.X, slime.velocity.Y, 150, new Color(0, 80, 255, 80), 1.2f);
                                Main.dust[num254].noGravity = true;
                                var dust = Main.dust[num254];
                                dust.velocity *= 0.5f;
                            }
                            JumpTimer = 15 + liferatio * 30;
                            TeleportPos = player.Center;
                            JumpCounter = TeleportCounter < 6 ? 0 : 5;
                        }
                        break;
                    case 5:
                        {
                            NPC.scale *= (45 - (TeleportCounter - 6)) / 45f;
                            TeleportCounter++;
                            if (TeleportCounter > 51)
                            {

                                Gore.NewGore(NPC.GetSource_FromThis(),NPC.Center + new Vector2(-40f, -NPC.height / 2), NPC.velocity, 734);
                                NPC.Bottom = TeleportPos;
                                JumpCounter = 6;
                                TeleportCounter = 45;

                            }
                            for (var i = 0; i < 10; i++)
                            {
                                int num254 = Dust.NewDust(NPC.position, NPC.width, NPC.height, 4, NPC.velocity.X, NPC.velocity.Y, 150, new Color(0, 80, 255, 80), 2f);
                                Main.dust[num254].noGravity = true;
                                var dust = Main.dust[num254];
                                dust.velocity *= 0.5f;
                            }
                        }
                        break;
                    case 6:
                        {
                            NPC.scale *= (45 - (TeleportCounter - 6)) / 45f;
                            TeleportCounter--;
                            if (TeleportCounter <= 0)
                            {
                                JumpCounter = 0;
                                TeleportCounter = 0;
                                JumpTimer = 1;
                            }
                            for (var i = 0; i < 10; i++)
                            {
                                int num254 = Dust.NewDust(NPC.position, NPC.width, NPC.height, 4, NPC.velocity.X, NPC.velocity.Y, 150, new Color(0, 80, 255, 80), 2f);
                                Main.dust[num254].noGravity = true;
                                var dust = Main.dust[num254];
                                dust.velocity *= 0.5f;
                            }
                        }
                        break;
                }

        }
        #endregion

        #region Netcode
        #endregion

        #region Helpers
        Player player => Main.player[NPC.target];
        public ref float JumpTimer => ref NPC.ai[0];
        public ref float JumpCounter => ref NPC.ai[1];

        public ref float TeleportCounter => ref NPC.ai[2];

        public Vector2 TeleportPos = new();
        #endregion

    }

    public class SpikedSlime : AIOverride
    {
        public SpikedSlime(NPC npc) : base(npc) { }

        #region Balancing Stats

        #endregion

        #region AI
        public override bool PreAI()
        {
            return true;
        }

        public override void AI()
        {
            if (Math.Abs(NPC.velocity.Y) <= 0.25f)
            {
                NPC.velocity.X *= 0.8f;
                if (JumpTimer == 30)
                {
                    bool outOfRange = (NPC.HasValidTarget && player.Center.Y < NPC.Center.Y - 320);
                    for (int j = 0; j < (outOfRange ? 7 : 3); j++)
                    {

                        Vector2 vector5 = new Vector2(j - (outOfRange ? 3 : 1), -3f);
                        vector5.X *= 1f + (float)Main.rand.Next(-50, 51) * 0.005f;
                        vector5.Y *= 1f + (float)Main.rand.Next(-50, 51) * 0.005f;
                        vector5.Normalize();
                        vector5 *= 4f + (float)Main.rand.Next(-50, 51) * 0.01f;

                        int attackDamage_ForProjectiles2 = NPC.GetAttackDamage_ForProjectiles(9f, 9f);
                        Vector2 vector4 = new Vector2(NPC.position.X + (float)NPC.width * 0.5f, NPC.position.Y + (float)NPC.height * 0.5f);

                        if (NPC.HasValidTarget && player.Center.Y < NPC.Center.Y - 160)
                        {
                            vector5 *= -(player.Center.Y - NPC.Center.Y) * 0.25f;
                        }
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), vector4.X, vector4.Y, vector5.X, vector5.Y, 605, attackDamage_ForProjectiles2, 0f, Main.myPlayer);
                    }
                }
                if (JumpTimer > 0)
                    JumpTimer--;
                
            }
            var kslime = Main.npc.FirstOrDefault(x => x.active && x.type == NPCID.KingSlime);
            if (kslime is not null)
            {
                if (kslime.ai[2] >= 7 && JumpCounter >= 0)
                    JumpCounter = -2;
                if (JumpCounter == -2 && JumpTimer == 0)
                {
                    NPC.damage = 0;
                    NPC.scale -= 0.0334f;
                    if (NPC.scale <= 0)
                    {
                        kslime.life += NPC.life;
                        if (kslime.lifeMax < kslime.life)
                            kslime.life = kslime.lifeMax;
                        NPC.active = false;
                    }
                }
                if (JumpCounter >= 0)
                {
                    var kslime2 = Main.npc.FirstOrDefault(x => x.active && x.type == NPCID.KingSlime && x.Hitbox.Contains(NPC.Center.ToPoint()));
                    if (kslime2 is not null)
                    {
                        kslime2.life += NPC.life;
                        if (kslime2.lifeMax < kslime2.life)
                            kslime2.life = kslime2.lifeMax;
                        NPC.active = false;
                        return;
                    }
                }
            }
            NPC.TargetClosest();
            if (!NPC.HasValidTarget)
                return;
            int dir = (player.Center.X - NPC.Center.X) > 0 ? 1 : -1;
            if (JumpTimer <= 0)
                switch (JumpCounter)
                {

                    case -1:
                        {
                            JumpCounter++;
                        }
                        break;
                    case 0:
                        {
                            NPC.velocity += new Vector2(2 * dir, -10);
                            JumpTimer = 30;
                            JumpCounter++;
                        }
                        break;
                    case 1:
                        {
                            NPC.velocity += new Vector2(3 * dir, -8);
                            JumpTimer = 30;
                            JumpCounter--;
                        }
                        break;
                    case -2:
                        {

                             int num254 = Dust.NewDust(NPC.position, NPC.width, NPC.height, 4, NPC.velocity.X, NPC.velocity.Y, 150, new Color(0, 80, 255, 80), 1.25f);
                            Main.dust[num254].noGravity = true;
                            var dust = Main.dust[num254];
                            dust.velocity *= 0.5f;
                            if (kslime is null)
                            {
                                NPC.scale += 0.1f;
                                if (NPC.scale >= 1)
                                {
                                    JumpCounter = 0;
                                    NPC.damage = NPC.defDamage;
                                }
                            }
                        }
                        break;

                }
        }
        #endregion

        #region Helpers

        Player player => Main.player[NPC.target];
        public ref float JumpTimer => ref NPC.ai[0];
        public ref float JumpCounter => ref NPC.ai[1];
        #endregion

    }

}
