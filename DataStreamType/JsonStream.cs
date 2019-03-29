using System.IO;
using Newtonsoft.Json;
using System.Collections.Generic;
using System;

namespace DataStreamType
{


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

        public void writeData(DrinkList data)
        {
            using (writer = new StreamWriter(filePath))
            {
                writer.Write(JsonConvert.SerializeObject(data));
            }
        }

        public DrinkList readData()
        {
            string returnString;

            using (reader = new StreamReader(filePath))
            {
                returnString = reader.ReadToEnd();
            }

            return JsonConvert.DeserializeObject<DrinkList>(returnString);
        }

        public bool isRegisteredDrink(string _drinkName)
        {
            return getAmountPerDrinks(_drinkName) != null;
        }

        public List<AmountPerDrinks> getAmountPerDrinks(string _drinkName)
        {
            DrinkList drinkList = readData();
            Drink drink = drinkList.find(_drinkName);

            return (drink == null) ? null : drink.recipe;
        }

        /// <summary>
        /// 특정 음료수에 대한 필요 재료 수치를 수정 혹은 추가
        /// </summary>
        /// <param name="drink"> 특정 음료수 명 </param>
        /// <param name="value"> 필요 재료 수치(수정 or 추가) </param>
        public void applyInputData(string _drinkName, List<AmountPerDrinks> _recipe)
        {
            DrinkList drinkList = readData();

            // 파일 여부 확인, 없으면 생성
            FileStream file = new FileStream(this.filePath, FileMode.OpenOrCreate);
            file.Close();

            drinkList.addOrUpdate(_drinkName, _recipe);
            writeData(drinkList);
        }

        public void applyInputData(DrinkList inputDataList)
        {
            DrinkList drinkList = readData();

            // 파일 여부 확인, 없으면 생성
            FileStream file = new FileStream(this.filePath, FileMode.OpenOrCreate);
            file.Close();

            foreach (var inputData in inputDataList)
            {
                drinkList.addOrUpdate(inputData.drinkName, inputData.recipe);
            }
            
            writeData(drinkList);
        }

        public void hardApplyInputData(DrinkList inputDataList)
        {
            // 파일 여부 확인, 없으면 생성
            FileStream file = new FileStream(this.filePath, FileMode.OpenOrCreate);
            file.Close();

            writeData(inputDataList);
        }

        /// <summary>
        /// 특정 음료수에 대한 필요 재료 수치 제거
        /// </summary>
        /// <param name="drink"> 음료수 명 </param>
        public void deleteData(string _drinkName)
        {
            DrinkList drinkList = readData();

            Drink target = drinkList.find(_drinkName);
            drinkList.Remove(target);

            writeData(drinkList);
        }

        /// <summary>
        /// 음료수 명을 input으로 받아서 해당 음료수에 대한 필요한 재료 수치를 반환하는 함수
        /// </summary>
        /// <param name="drink"> input 음료수 명 </param>
        /// <returns> 특정 음료수에 대한 필요 재료 수치 (없다면 null 반환) </returns>
        public string getAmount(string _drinkName, string _ingredientName)
        {
            DrinkList drinkList = readData();

            Drink target = drinkList.find(_drinkName);

            return (target == null) ? null : target.getAmount(_ingredientName);
        }
    }
}