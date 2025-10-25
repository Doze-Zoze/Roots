using Roots.Utilities;
using Steamworks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Roots
{
	public partial class Roots : Mod
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
