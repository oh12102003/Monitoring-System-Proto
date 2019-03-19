using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace DataStreamType
{
    class amountPerDrinks
    {
        public string drink;
        public int amount;
    }

    class JsonStream
    {
        string filePath;
        StreamReader reader;
        StreamWriter writer;

        public JsonStream (string ingredients)
        {
            this.filePath = Global.jsonPath + "/" + ingredients + ".json";
        }

        // 추가
        public void addData(string drinks, string value)
        {
            

            // 한 줄씩 읽으면서 수정되었다면 수정
            using (reader = new StreamReader(filePath, Encoding.UTF8))
            {
                string oneLine;

                while (!reader.EndOfStream)
                {
                    oneLine = reader.ReadLine();

                }
            }
        }

        // 삭제
    }
}
