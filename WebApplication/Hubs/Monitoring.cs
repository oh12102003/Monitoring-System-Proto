using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System.Net.Sockets;
using DataStreamType;

namespace WebApplication.Hubs
{
    [HubName("monitoringHub")]
    public class Monitoring : Hub
    {
        Sensor toMiddleServer;

        public Monitoring()
        {
            // 미들 서버에 연결
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(Global.serverAddr);

            toMiddleServer = new Sensor(socket);

            // register 코드
            toMiddleServer.sensorName = "칠성사이다";
            toMiddleServer.sensorType = "webClient";
        }

        public void connect()
        {
            toMiddleServer.setBuffer("Register", "칠성사이다");
            toMiddleServer.socket.Send(toMiddleServer.buffer, 0, toMiddleServer.length, SocketFlags.None);
            Clients.All.printIt("Connected to Middle Server");
        }

        public void getData()
        {
            // Show 명령어 전송
            toMiddleServer.setBuffer("Show", "All");
            toMiddleServer.socket.Send(toMiddleServer.buffer, 0, toMiddleServer.length, SocketFlags.None);

            // 명령어에 대한 결과 값 get
            toMiddleServer.clear();
            toMiddleServer.socket.Receive(toMiddleServer.buffer, toMiddleServer.length, SocketFlags.None);

            Clients.All.printIt(toMiddleServer.getBuffer());
        }
    }
}