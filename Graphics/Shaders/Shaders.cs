using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace Roots.Graphics.Shaders
{
    public class Shaders : ModSystem
    {

        public static Asset<Effect>? RainbowShader;
        public override void PostSetupContent()
        {
            RainbowShader ??= ModContent.GetInstance<RootsBeta.RootsBeta>().Assets.Request<Effect>("Graphics/Shaders/Rainbow", AssetRequestMode.ImmediateLoad);
            GameShaders.Misc["Roots:RainbowShader"] = new MiscShaderData(RainbowShader, "RainbowPass");
        }
    }
}
