using System.Collections.Generic;

namespace DataStreamType
{
    public class Drink
    {
        public string drinkName;
        public List<AmountPerDrinks> recipe;

        public Drink(string _drinkName, List<AmountPerDrinks> _recipe)
        {
            this.drinkName = _drinkName;
            this.recipe = _recipe;
        }

        public void addIngredient(AmountPerDrinks apd)
        {
            this.recipe.Add(apd);
        }

        public void updateIngredient(AmountPerDrinks apd)
        {
            foreach (var oneData in recipe)
            {
                if (oneData.ingredient.Equals(apd.ingredient))
                {
                    oneData.amount = apd.amount;
                    return;
                }
            }
        }

        public void deleteIngredient(string ingredient)
        {
            foreach (var oneData in recipe)
            {
                if (oneData.ingredient.Equals(ingredient))
                {
                    recipe.Remove(oneData);
                    return;
                }
            }
        }

        public void deleteIngredient(AmountPerDrinks apd)
        {
            this.deleteIngredient(apd.ingredient);
        }

        public string getAmount(string ingredientName)
        {
            foreach (var oneData in recipe)
            {
                if (oneData.ingredient.Equals(ingredientName))
                {
                    return oneData.amount;
                }
            }

            return null;
        }
    }
}
