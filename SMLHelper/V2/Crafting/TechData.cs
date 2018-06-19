namespace SMLHelper.V2.Crafting
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    public class TechData : ITechData
    {
        public int craftAmount { get; set; }

        public int ingredientCount => Ingredients.Count;
        public int linkedItemCount => LinkedItems.Count;

        public List<Ingredient> Ingredients = new List<Ingredient>();
        public List<TechType> LinkedItems = new List<TechType>();

        public TechData() { }

        public TechData(List<Ingredient> ingredients)
        {
            Ingredients = ingredients;
        }

        public TechData(params Ingredient[] ingredients)
        {
            foreach(var ingredient in ingredients)
            {
                Ingredients.Add(ingredient); 
            }
        }

        public IIngredient GetIngredient(int index)
        {
            if(Ingredients != null && Ingredients.Count > index)
            {
                return Ingredients[index];
            }

            return null;
        }

        public TechType GetLinkedItem(int index)
        {
            if (LinkedItems != null && Ingredients.Count > index)
            {
                return LinkedItems[index];
            }

            return TechType.None;
        }
    }

    public class Ingredient : IIngredient
    {
        public TechType techType { get; set; }
        public int amount { get; set; }

        public Ingredient(TechType techType, int amount)
        {
            this.techType = techType;
            this.amount = amount;
        }
    }
}
