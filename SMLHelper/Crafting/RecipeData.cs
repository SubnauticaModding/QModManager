#if BELOWZERO
#pragma warning disable IDE1006 // Naming Styles - Ignored for backwards compatibility
namespace SMLHelper.V2.Crafting
{
    using System.Collections.Generic;

    /// <summary>
    /// A class that fully describes a recipe for a <see cref="TechType"/> identified item.
    /// </summary>
    /// <seealso cref="TechData" />
    public class RecipeData
    {
        /// <summary>
        /// Gets or sets the how many copies of the item are created when crafting this recipe.
        /// </summary>
        /// <value>
        /// The quantity of the item this recipe yields.
        /// </value>
        public int craftAmount { get; set; }

        /// <summary>
        /// Gets the number of different ingredients for this recipe.
        /// </summary>
        /// <value>
        /// The number of ingredients for this recipe.
        /// </value>
        public int ingredientCount => Ingredients.Count;

        /// <summary>
        /// Gets the number of items linked to this recipe.
        /// </summary>
        /// <value>
        /// The number of linked items.
        /// </value>
        public int linkedItemCount => LinkedItems.Count;

        /// <summary>
        /// The list of ingredients required for this recipe.
        /// </summary>
        public List<Ingredient> Ingredients = new List<Ingredient>();

        /// <summary>
        /// The items that will also be created when this recipe is crafted.
        /// </summary>
        public List<TechType> LinkedItems = new List<TechType>();

        /// <summary>
        /// Initializes a new instance of the <see cref="TechData"/> class a custom recipe.
        /// </summary>
        public RecipeData() { }

        /// <summary>
        /// Initializes a new instance of the <see cref="TechData"/> class for a custom recipe with a list of ingridients.
        /// </summary>
        /// <param name="ingredients">The ingredients.</param>
        public RecipeData(List<Ingredient> ingredients)
        {
            Ingredients = ingredients;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TechData"/> class for a custom recipe with a collection of ingridients.
        /// </summary>
        /// <param name="ingredients">The ingredients.</param>
        public RecipeData(params Ingredient[] ingredients)
        {
            foreach (Ingredient ingredient in ingredients)
            {
                Ingredients.Add(ingredient);
            }
        }

        /// <summary>
        /// Gets the ingredient at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>The <see cref="Ingredient"/> at the requested the index if the index is value; Otherwise returns null.</returns>
        public Ingredient GetIngredient(int index)
        {
            if (Ingredients != null && Ingredients.Count > index)
            {
                return Ingredients[index];
            }

            return null;
        }

        /// <summary>
        /// Gets the linked item at the specified index.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <returns>The <see cref="TechType"/> at the requested the index if the index is value; Otherwise returns null.</returns>
        public TechType GetLinkedItem(int index)
        {
            if (LinkedItems != null && LinkedItems.Count > index)
            {
                return LinkedItems[index];
            }
            return TechType.None;
        }
    }
}
#pragma warning restore IDE1006 // Naming Styles
#endif