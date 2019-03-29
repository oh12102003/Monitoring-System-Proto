using System.IO;
using System.Net;

namespace DataStreamType
{
    public class Global
    {
        public static IPEndPoint serverAddr = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8888);

<<<<<<< HEAD
        //public static string jsonPath = Path.Combine(@"C:\Users", "user", "Desktop", "Monitoring-System-Proto", "WebApplication", "Sources");
        public static string jsonPath = Path.Combine(@"D:\백업 파일", "신입사원 2차 개발 과제", "Asynchornouse multi-socket client-server program", "WebApplication", "Sources");
=======
        public static string jsonPath = Path.Combine(@"C:\Users", "user", "Desktop", "Monitoring-System-Proto", "WebApplication", "Sources");
        //public static string jsonPath = Path.Combine(@"D:\백업 파일", "신입사원 2차 개발 과제", "Asynchornouse multi-socket client-server program", "WebApplication", "Sources");
>>>>>>> 88066cbd18f8e5f999b110bbadd1aeb8946dd0ba
    }
}
