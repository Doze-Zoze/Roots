using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Roots.NPCs
{
    public partial class RootsGlobalNPC : GlobalNPC
    {
        static Dictionary<int, Predicate<NPC>> PreAIOverrides = new();
        static Dictionary<int, Action<NPC>> AIOverrides = new();

        public override void SetStaticDefaults()
        {
            PreAIOverrides = new()
            {
                {NPCID.WallofFlesh, WallOfFlesh_PreAI },
                {NPCID.WallofFleshEye, WallOfFleshEye_PreAI }
            };
            AIOverrides = new()
            {
                { NPCID.WallofFlesh, WoFAiOverride }
            };
        }
        public override bool PreAI(NPC npc)
        {
            //bool hasPreAIOverride = false;
            //bool preAIReturn = false;
            if (PreAIOverrides.Keys.Contains(npc.type))
            {
                return PreAIOverrides[npc.type](npc);
               // hasPreAIOverride = true;
            }
            /*else if (AIOverrides.Keys.Contains(npc.type))
            {
                AIOverrides[npc.type](npc);
                if (!hasPreAIOverride)
                    return false;
            }
            if (hasPreAIOverride)
                return PreAIOverrides[npc.type](npc);
            */
            return true;
        }


    }
}
