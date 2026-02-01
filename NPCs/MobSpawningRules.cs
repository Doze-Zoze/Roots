using System.Collections.Generic;
using Terraria.ID;
using Terraria.ModLoader;

namespace RootsBeta.NPCs
{
    public class MobSpawningRules : GlobalNPC
    {

        public override void EditSpawnPool(IDictionary<int, float> pool, NPCSpawnInfo spawnInfo)
        {
            if (spawnInfo.Player.ZoneJungle)
                pool.Add(NPCID.Snatcher, 0.1f);
        }
    }
}
