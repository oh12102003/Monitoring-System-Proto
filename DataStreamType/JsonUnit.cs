using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataStreamType
{
    public interface IJsonType
    {
        string serialize();
    }

    public class JsonList : IJsonType
    {
        public List<JsonUnit> sensorList = null;

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

        public static JsonList Parse(string str)
        {
            return JsonConvert.DeserializeObject<JsonList>(str);
        }
    }

    public class JsonUnit : IJsonType
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
