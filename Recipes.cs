using Roots.Config;
using RootsBeta.Items.Accessories.Magic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace RootsBeta
{
    public class RecipeChanges : ModSystem
    {
        public override void PostAddRecipes()
        {
            for (int i = 0; i < Recipe.numRecipes; i++)
            {
                Recipe recipe = Main.recipe[i];

                if (ConfigHelpers.ConfigEnabled(typeof(ManaCloak).Name))
                {
                    if (recipe.TryGetIngredient(ItemID.ManaFlower, out _) && recipe.TryGetResult(ItemID.ManaCloak, out _))
                    {
                        recipe.RemoveIngredient(ItemID.ManaFlower);
                        recipe.AddIngredient(ItemID.CelestialCuffs);
                    }
                }

                //TODO - Config
                if (!RootsModConfig.Instance.AmmoChanges) continue;

                #region Arrows
                if (recipe.TryGetResult(ItemID.WoodenArrow, out var result))
                    result.stack = 10;

                if (recipe.TryGetResult(ItemID.FlamingArrow, out result) && recipe.TryGetIngredient(ItemID.WoodenArrow, out var ingredient))
                    result.stack = ingredient.stack = 10;

                if (recipe.TryGetResult(ItemID.FrostburnArrow, out result) && recipe.TryGetIngredient(ItemID.WoodenArrow, out ingredient))
                    result.stack = ingredient.stack = 10;
                    
                if (recipe.TryGetResult(ItemID.HellfireArrow, out result) && recipe.TryGetIngredient(ItemID.WoodenArrow, out ingredient))
                    result.stack = ingredient.stack = 50;

                if (recipe.TryGetResult(ItemID.CursedArrow, out result) && recipe.TryGetIngredient(ItemID.WoodenArrow, out ingredient))
                    result.stack = ingredient.stack = 10;

                if (recipe.TryGetResult(ItemID.HolyArrow, out result) && recipe.TryGetIngredient(ItemID.WoodenArrow, out ingredient))
                    result.stack = ingredient.stack = 20;

                if (recipe.TryGetResult(ItemID.UnholyArrow, out result) && recipe.TryGetIngredient(ItemID.WoodenArrow, out ingredient))
                    result.stack = ingredient.stack = 20;

                if (recipe.TryGetResult(ItemID.IchorArrow, out result) && recipe.TryGetIngredient(ItemID.WoodenArrow, out ingredient))
                    result.stack = ingredient.stack = 10;

                if (recipe.TryGetResult(ItemID.VenomArrow, out result) && recipe.TryGetIngredient(ItemID.WoodenArrow, out ingredient))
                    result.stack = ingredient.stack = 10;
                #endregion

                #region Bullets
                if (recipe.TryGetResult(ItemID.MeteorShot, out result) && recipe.TryGetIngredient(ItemID.MusketBall, out ingredient))
                    result.stack = ingredient.stack = 25;

                if (recipe.TryGetResult(ItemID.SilverBullet, out result) && recipe.TryGetIngredient(ItemID.MusketBall, out ingredient))
                    result.stack = ingredient.stack = 25;

                if (recipe.TryGetResult(ItemID.TungstenBullet, out result) && recipe.TryGetIngredient(ItemID.MusketBall, out ingredient))
                    result.stack = ingredient.stack = 25;

                if (recipe.TryGetResult(ItemID.ChlorophyteBullet, out result) && recipe.TryGetIngredient(ItemID.MusketBall, out ingredient))
                    result.stack = ingredient.stack = 25;

                if (recipe.TryGetResult(ItemID.CrystalBullet, out result) && recipe.TryGetIngredient(ItemID.MusketBall, out ingredient))
                    result.stack = ingredient.stack = 15;

                if (recipe.TryGetResult(ItemID.IchorBullet, out result) && recipe.TryGetIngredient(ItemID.MusketBall, out ingredient))
                    result.stack = ingredient.stack = 15;

                if (recipe.TryGetResult(ItemID.CursedBullet, out result) && recipe.TryGetIngredient(ItemID.MusketBall, out ingredient))
                    result.stack = ingredient.stack = 15;
                #endregion
            }
        }
    }
}
