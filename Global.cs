using System.Net;

namespace DataStreamType
{
    class Global
    {
        public static IPEndPoint serverAddr = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8888);
        public static string jsonPath = "./jsons";
    }
}
