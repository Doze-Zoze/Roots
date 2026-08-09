using RootsCore;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace RootsBeta.NPCs
{

    public partial class RootsAIOverrideSystem : GlobalNPC
    {
        public override void SetStaticDefaults()
        {
            foreach (var item in EnemyAIChanges)
            {
                NpcSets.AiOverrides[item.Key].Add(((n) => Configs.instance.EnemyChanges, item.Value));
            }

            foreach (var item in BossAIChanges)
            {
                NpcSets.AiOverrides[item.Key].Add(((n) => Configs.instance.BossChanges, item.Value));
            }

            if (Configs.instance.EnemyChanges)
            {
                NPCID.Sets.ImmuneToAllBuffs[NPCID.Snatcher] = true;
            }
        }

        public static Dictionary<int, Func<NPC, AIOverride>> EnemyAIChanges = new()
        {
            { NPCID.ManEater, x => new ManEater(x) },
            { NPCID.Snatcher, x => new Snatcher(x) },
            { NPCID.AngryTrapper, x => new AngryTrapper(x) }
        };


        public static Dictionary<int, Func<NPC, AIOverride>> BossAIChanges = new()
        {
            { NPCID.KingSlime, x => new KingSlime(x) },
            { NPCID.SlimeSpiked, x => new SpikedSlime(x) },
            { NPCID.WallofFlesh, x => new WoFMouth(x) },
            { NPCID.WallofFleshEye, x => new WoFEye(x) },
            { NPCID.TheHungry, x => new HungryAttached(x) },
            { NPCID.TheHungryII, x => new HungryDetached(x) },
        };
    }
    /// <summary>
    /// Used entirely just to tell Snatchers that they can actually take damage from this source
    /// </summary>
    public class PickaxeDamage : DamageClass { }
    public class AllowKillingSnatchers : GlobalTile
    {
        public override void KillTile(int i, int j, int type, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            if (Configs.instance.EnemyChanges && FixExploitManEaters.SpotProtected(i, j))
            {
                effectOnly = true;
                foreach (var item in Main.ActiveNPCs)
                {
                    if ((item.type == NPCID.Snatcher || item.type == NPCID.ManEater) && (int)item.ai[0] == i && (int)item.ai[1] == j)
                    {
                        NPC.HitInfo hit = new()
                        {
                            Damage = Main.LocalPlayer.HeldItem.pick > 0 ? Main.LocalPlayer.HeldItem.pick : 100,
                            DamageType = ModContent.GetInstance<PickaxeDamage>()
                        };
                        hit.Damage -= (int)(item.defense * 0.5f);
                        item.StrikeNPC(hit);
                        item.netUpdate = true;
                    }
                }
            }
        }
    }
}
