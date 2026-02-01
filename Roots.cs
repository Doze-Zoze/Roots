using RootsBeta.Utilities;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RootsBeta
{
	public partial class RootsBeta : Mod
	{
        public override void Load()
        {
            LoadEdits();
        }

        public override void PostSetupContent()
        {
            FinishIDSets();
        }
    }
}
