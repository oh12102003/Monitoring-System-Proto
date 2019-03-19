using System.IO;
using System.Text;
using Newtonsoft.Json;
using System.Collections.Generic;
using System;

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
            foreach(var oneData in recipe)
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

    public class AmountPerDrinks
    {
        public string ingredient;
        public string amount;

        public AmountPerDrinks(string ingredientName, string amount)
        {
            this.ingredient = ingredientName;
            this.amount = amount;
        }
    }

    public class JsonStream
    {
        public string filePath;
        StreamReader reader;
        StreamWriter writer;

        public JsonStream(string fileName = "drink")
        {
            this.filePath = Path.Combine(Global.jsonPath, fileName + ".json");
            Console.WriteLine(filePath);
        }

        private void apply(StringBuilder sb)
        {
            using (writer = new StreamWriter(filePath))
            {
                writer.Write(sb.ToString());
            }
        }

        public bool isRegisteredDrink(string _drinkName)
        {
            return getAmountPerDrinks(_drinkName) != null;
        }

        public List<AmountPerDrinks> getAmountPerDrinks(string _drinkName)
        {
            using (reader = new StreamReader(filePath))
            {
                while (!reader.EndOfStream)
                {
                    Drink drink = JsonConvert.DeserializeObject<Drink>(reader.ReadLine());

                    if (drink.drinkName.Equals(_drinkName))
                    {
                        return drink.recipe;
                    }
                }
            }

            return null;
        }

        /// <summary>
        /// 특정 음료수에 대한 필요 재료 수치를 수정 혹은 추가
        /// </summary>
        /// <param name="drink"> 특정 음료수 명 </param>
        /// <param name="value"> 필요 재료 수치(수정 or 추가) </param>
        public void applyInputData(string _drinkName, List<AmountPerDrinks> _recipe)
        {
            StringBuilder sb = new StringBuilder();
            bool isUpdated = false;

            // 파일 여부 확인, 없으면 생성
            FileStream file = new FileStream(this.filePath, FileMode.OpenOrCreate);
            file.Close();

            // 한 줄씩 읽으면서 수정되었다면 수정
            using (reader = new StreamReader(filePath, Encoding.UTF8))
            {
                while (!reader.EndOfStream)
                {
                    Drink drink = JsonConvert.DeserializeObject<Drink>(reader.ReadLine());

                    if (drink.drinkName.Equals(_drinkName))
                    {
                        isUpdated = true;
                        drink.recipe = _recipe;
                    }

                    sb.AppendLine(JsonConvert.SerializeObject(drink));
                }

                // (업데이트 되지 않음 = 추가) 마지막 라인에 추가
                if (!isUpdated)
                {
                    Drink drink = new Drink(_drinkName, _recipe);
                    sb.AppendLine(JsonConvert.SerializeObject(drink));
                }
            }

            apply(sb);
        }

        /// <summary>
        /// 특정 음료수에 대한 필요 재료 수치 제거
        /// </summary>
        /// <param name="drink"> 음료수 명 </param>
        public void deleteData(string _drinkName)
        {
            StringBuilder sb = new StringBuilder();

            using (reader = new StreamReader(filePath, Encoding.UTF8))
            {
                while (!reader.EndOfStream)
                {
                    Drink oneData = JsonConvert.DeserializeObject<Drink>(reader.ReadLine());

                    if (!oneData.drinkName.Equals(_drinkName))
                    {
                        sb.AppendLine(JsonConvert.SerializeObject(oneData));
                    }
                }
            }

            apply(sb);
        }

        /// <summary>
        /// 음료수 명을 input으로 받아서 해당 음료수에 대한 필요한 재료 수치를 반환하는 함수
        /// </summary>
        /// <param name="drink"> input 음료수 명 </param>
        /// <returns> 특정 음료수에 대한 필요 재료 수치 (없다면 null 반환) </returns>
        public string getAmount(string drinkName, string ingredientName)
        {
            using (reader = new StreamReader(filePath, Encoding.UTF8))
            {
                while (!reader.EndOfStream)
                {
                    Drink oneData = JsonConvert.DeserializeObject<Drink>(reader.ReadLine());

                    if (oneData.drinkName.Equals(drinkName))
                    {
                        reader.Close();
                        return oneData.getAmount(ingredientName);
                    }
                }
            }

            return null;
        }
    }
}
