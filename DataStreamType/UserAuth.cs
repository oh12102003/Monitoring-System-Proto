using Newtonsoft.Json;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;

namespace DataStreamType
{
    public class UserAuthList : ICollection<UserAuth>
    {
        List<UserAuth> userList;
        public int length { get { return userList.Count; }}

        public UserAuthList()
        {
            userList = new List<UserAuth>();
        }

        public UserAuth this[int i]
        {
            get { return userList[i]; }
        }

        public static bool TryParse(string jsonString, out UserAuthList returnList)
        {
            try
            {
                returnList = JsonConvert.DeserializeObject<UserAuthList>(jsonString);
                return true;
            }

            catch (Exception e)
            {
                returnList = null;
                return false;
            }
        }

        int ICollection<UserAuth>.Count
        {
            get
            {
                return userList.Count;
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
            userList.Clear();
        }

        public bool Contains(UserAuth user)
        {
            return userList.Contains(user);
        }

        public void Add(UserAuth user)
        {
            userList.Add(user);
        }


        public bool Remove(UserAuth user)
        {
            return userList.Remove(user);
        }

        public IEnumerator<UserAuth> GetEnumerator()
        {
            return userList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void CopyTo(UserAuth[] array, int arrayIndex)
        {
            throw new NotImplementedException();
        }
    }

    public class UserAuth
    {
        public string userId;
        public string monitoringAuth;
        public string recipeAuth;
        public string managementAuth;

        public  static UserAuth deserialize(string jsonString)
        {
            return JsonConvert.DeserializeObject<UserAuth>(jsonString);
        }

        public string serialize()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
