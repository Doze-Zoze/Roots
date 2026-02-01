using System;
using System.Collections.Generic;
using System.Formats.Asn1;
using System.IO;
using System.Reflection;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace RootsBeta.NPCs
{
    public abstract class AIOverride
    {
        public NPC NPC = null;
        protected AIOverride(NPC npc)
        {
            NPC = npc;
        }
        /// <inheritdoc cref="GlobalNPC.PreAI(NPC)" />
        public virtual bool PreAI() => true;

        /// <inheritdoc cref="GlobalNPC.AI(NPC)" />
        public virtual void AI() { }

        /// <inheritdoc cref="GlobalNPC.SendExtraAI(NPC, BitWriter, BinaryWriter)" />
        public virtual void SendExtraAI(BitWriter bitWriter, BinaryWriter binaryWriter) { }

        /// <inheritdoc cref="GlobalNPC.ReceiveExtraAI(NPC, BitReader, BinaryReader)" />
        public virtual void ReceiveExtraAI(BitReader bitReader, BinaryReader binaryReader) { }
        public virtual void SetDefaults() { }

        /// <inheritdoc cref="GlobalNPC.OnSpawn(NPC, IEntitySource)" />
        public virtual void OnSpawn(IEntitySource source) { }

        /// <inheritdoc cref="GlobalNPC.OnKill(NPC)" />
        [Obsolete("Not Implemented")]
        public virtual void OnKill() { }

    }
    public partial class RootsAIOverrideSystem : GlobalNPC
    {

        public override bool InstancePerEntity => true;
        public AIOverride CurrentAiOverride = null;

        public static Dictionary<int, Func<NPC, AIOverride>> AiOverridesDictionary = new()
        {
            { NPCID.ManEater, x => new ManEater(x) },
            { NPCID.Snatcher, x => new Snatcher(x) },
            { NPCID.AngryTrapper, x => new AngryTrapper(x) },
            { NPCID.KingSlime, x => new KingSlime(x) },
            { NPCID.SlimeSpiked, x => new SpikedSlime(x) },
        };

        public override void SetDefaults(NPC npc)
        {
            base.SetDefaults(npc);
            if (Configs.instance.AiChanges && CurrentAiOverride is null && AiOverridesDictionary.ContainsKey(npc.type))
                CurrentAiOverride = AiOverridesDictionary[npc.type].Invoke(npc);

            CurrentAiOverride?.SetDefaults();
        }
        public override bool PreAI(NPC npc)
        {
            if (CurrentAiOverride is null)
                return true;

            if (CurrentAiOverride.PreAI())
            {
                CurrentAiOverride.AI();
            }
            return false;
        }

        public override void SendExtraAI(NPC npc, BitWriter bitWriter, BinaryWriter binaryWriter)
        {
            CurrentAiOverride?.SendExtraAI(bitWriter, binaryWriter);
        }

        public override void ReceiveExtraAI(NPC npc, BitReader bitReader, BinaryReader binaryReader)
        {
            CurrentAiOverride?.ReceiveExtraAI(bitReader, binaryReader);
        }

        public override void OnSpawn(NPC npc, IEntitySource source)
        {
            CurrentAiOverride?.OnSpawn(source);
        }

    }

    public class AllowKillingSnatchers : GlobalTile
    {
        public override void KillTile(int i, int j, int type, ref bool fail, ref bool effectOnly, ref bool noItem)
        {
            if (Configs.instance.AiChanges && FixExploitManEaters.SpotProtected(i, j))
            {
                effectOnly = true;
                foreach (var item in Main.ActiveNPCs)
                {
                    if ((item.type == NPCID.Snatcher || item.type == NPCID.ManEater) && (int)item.ai[0] == i && (int)item.ai[1] == j)
                    {
                        item.SimpleStrikeNPC(50, 0);
                        item.netUpdate = true;
                    }
                }
            }
        }
    }
}
