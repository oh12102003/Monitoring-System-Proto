using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;

namespace DataStreamType
{
    public class DrinkList : ICollection<Drink>
    {
        public List<Drink> drinkList;

        public DrinkList()
        {
            drinkList = new List<Drink>();
        }

        public static DrinkList Parse(string jsonString)
        {
            Console.WriteLine(jsonString);
            return JsonConvert.DeserializeObject<DrinkList>(jsonString);
        }

        int ICollection<Drink>.Count
        {
            get
            {
                return drinkList.Count;
            }
        }

        public bool IsReadOnly
        {
            get
            {
                return false;
            }
        }

        public void Clear()
        {
            drinkList.Clear();
        }

        public bool Contains(Drink drink)
        {
            return drinkList.Contains(drink);
        }

        public void Add(Drink drink)
        {
            drinkList.Add(drink);
        }


        public bool Remove(Drink drink)
        {
            return drinkList.Remove(drink);
        }

        public Drink find(string _drinkName)
        {
            if (_drinkName == null)
            {
                return null;
            }

            return drinkList.Find(drink => drink.drinkName.Equals(_drinkName));
        }

        public void addOrUpdate(string _drinkName, List<AmountPerDrinks> _recipe)
        {
            Drink target = find(_drinkName);

            if (target != null)
            {
                target.recipe = _recipe;
            }

            else
            {
                Add(new Drink(_drinkName, _recipe));
            }
        }

        public IEnumerator<Drink> GetEnumerator()
        {
            return drinkList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void CopyTo(Drink[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }
    }

    public class Drink
    {
        public string drinkName;
        public List<AmountPerDrinks> recipe;

        public Drink()
        {
            recipe = new List<AmountPerDrinks>();
        }

        public Drink(string _drinkName)
        {
            this.drinkName = _drinkName;
            recipe = new List<AmountPerDrinks>();
        }

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
