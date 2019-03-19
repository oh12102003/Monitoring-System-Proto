using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using System.Net.Sockets;
using DataStreamType;
using System;

namespace WebApplication.Hubs
{
    [HubName("monitoringHub")]
    public class Monitoring : Hub
    {
        static IHubContext toClient = GlobalHost.ConnectionManager.GetHubContext<Monitoring>();
        static Sensor toMiddleServer;

        public Monitoring()
        {
            // 미들 서버에 연결
            Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.Connect(Global.serverAddr);
            toClient.Clients.All.printInConsole("Connected to Middle Server..");

            toMiddleServer = new Sensor(socket);

            // register 코드
            toMiddleServer.sensorName = "칠성사이다";
            toMiddleServer.sensorType = "webClient";

            toMiddleServer.setBuffer("Register", "칠성사이다");
            toMiddleServer.socket.Send(toMiddleServer.buffer, 0, toMiddleServer.length, SocketFlags.None);
            toClient.Clients.All.printInConsole("Send register request...");
        }

        public void Show(string type, string msg)
        {
            // Show 명령어 전송
            toMiddleServer.setBuffer(type, msg);
            toMiddleServer.socket.BeginSendTo(toMiddleServer.buffer, 0, toMiddleServer.length, SocketFlags.None,
                toMiddleServer.whereFrom, asyncSendShowCommand, toMiddleServer);
            toMiddleServer.socket.BeginReceiveFrom(toMiddleServer.buffer, 0, toMiddleServer.length, SocketFlags.None,
                ref toMiddleServer.whereFrom, asyncReceiveResponseData, toMiddleServer);
        }

        void asyncSendShowCommand(IAsyncResult ar)
        {
            Sensor toServer = ar.AsyncState as Sensor;
            toServer.socket.EndSend(ar);

            toServer.setBuffer("Show", "All");
            toServer.socket.BeginSendTo(toServer.buffer, 0, toServer.length, SocketFlags.None,
                toServer.whereFrom, asyncSendShowCommand, toServer);
        }

        void asyncReceiveResponseData(IAsyncResult ar)
        {
            Sensor fromServer = ar.AsyncState as Sensor;
            fromServer.socket.EndReceive(ar);
            toClient.Clients.All.printInPage(fromServer.getBuffer());

            fromServer.clear();
            fromServer.socket.BeginReceiveFrom(fromServer.buffer, 0, fromServer.length, SocketFlags.None,
                ref fromServer.whereFrom, asyncReceiveResponseData, fromServer);
        }
    }
}