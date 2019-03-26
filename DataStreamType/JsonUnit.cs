using Newtonsoft.Json;
using System.Collections.Generic;

namespace DataStreamType
{
    public class JsonList
    {
        List<JsonUnit> sensorList;

        public JsonList()
        {
            sensorList = new List<JsonUnit>();
        }

        public int length
        {
            get
            {
                return sensorList.Count;
            }
        }

        public JsonUnit this[int index]
        {
            get
            {
                if (index > sensorList.Count)
                {
                    return null;
                }

                return sensorList[index];
            }
        }

        public void add(JsonUnit unit)
        {
            sensorList.Add(unit);
        }

        public void remove(JsonUnit unit)
        {
            sensorList.Remove(unit);
        }

        public string serialize()
        {
            return JsonConvert.SerializeObject(this);
        }
    }

    public class JsonUnit
    {
        public string name;
        public string type;
        public string value;

        public JsonUnit(string _name, string _type, string _value)
        {
            name = _name;
            type = _type;
            value = _value;
        }

        public string serialize()
        {
            return JsonConvert.SerializeObject(this);
        }

        public static JsonUnit Parse(string str)
        {
            return JsonConvert.DeserializeObject<JsonUnit>(str);
        }
    }
}
