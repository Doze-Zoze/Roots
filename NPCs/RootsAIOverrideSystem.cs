using Roots.Config;
using RootsCore;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace RootsBeta.NPCs
{

    public class RootsAIOverrideSystem : GlobalNPC
    {
        public override void SetStaticDefaults()
        {
            foreach (var item in EnemyAIChanges)
            {
                NpcSets.AIOverrides[item.Key].Add((item.Value.predicate, item.Value.aiOverride));
            }

            foreach (var item in BossAIChanges)
            {
                NpcSets.AIOverrides[item.Key].Add((item.Value.predicate, item.Value.aiOverride));
            }

            if (RootsModConfig.Instance.EnemyChanges)
            {
                NPCID.Sets.ImmuneToAllBuffs[NPCID.Snatcher] = true;
            }
        }

        public static Dictionary<int, (Predicate<NPC> predicate,  Func<NPC, AIOverride> aiOverride)> EnemyAIChanges = new()
        {
            { NPCID.ManEater, (x => ConfigHelpers.ConfigEnabled<ManEater>(),  x => new ManEater(x)) },
            { NPCID.Snatcher, (x => ConfigHelpers.ConfigEnabled<Snatcher>(), x => new Snatcher(x)) },
            { NPCID.AngryTrapper, (x => ConfigHelpers.ConfigEnabled<AngryTrapper>(), x => new AngryTrapper(x)) }
        };


        public static Dictionary<int, (Predicate<NPC> predicate, Func<NPC, AIOverride> aiOverride)> BossAIChanges = new()
        {
            { NPCID.KingSlime, (x => ConfigHelpers.ConfigEnabled<KingSlime>(), x => new KingSlime(x)) },
            { NPCID.SlimeSpiked, (x => ConfigHelpers.ConfigEnabled<KingSlime>(), x => new SpikedSlime(x)) },
            { NPCID.WallofFlesh, (x => ConfigHelpers.ConfigEnabled<WoFMouth>(), x => new WoFMouth(x)) },
            { NPCID.WallofFleshEye, (x => ConfigHelpers.ConfigEnabled<WoFMouth>(), x => new WoFEye(x)) },
            { NPCID.TheHungry, (x => ConfigHelpers.ConfigEnabled<WoFMouth>(), x => new HungryAttached(x)) },
            { NPCID.TheHungryII, (x => ConfigHelpers.ConfigEnabled<WoFMouth>(), x => new HungryDetached(x)) },
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
            if (!RootsModConfig.Instance.EnemyChanges || !FixExploitManEaters.SpotProtected(i, j)) return;
            effectOnly = true;
            foreach (var item in Main.ActiveNPCs)
            {
                if ((item.type != NPCID.Snatcher && item.type != NPCID.ManEater) || (int)item.ai[0] != i ||
                    (int)item.ai[1] != j) continue;
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
