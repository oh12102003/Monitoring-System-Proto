using Newtonsoft.Json;
using System.Collections.Generic;

namespace DataStreamType
{
    public class JsonList
    {
        List<JsonUnit> list;

        public int length
        {
            get
            {
                return list.Count;
            }
        }

        public JsonUnit this[int index]
        {
            get
            {
                if (index > list.Count)
                {
                    return null;
                }

                return list[index];
            }
        }

        public void add(JsonUnit unit)
        {
            list.Add(unit);
        }

        public void remove(JsonUnit unit)
        {
            list.Remove(unit);
        }
    }

    public class JsonUnit
    {
        public string name;
        public string value;

        public JsonUnit(string _name, string _value)
        {
            name = _name;
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
