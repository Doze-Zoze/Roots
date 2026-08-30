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
