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
        static Socket toSimulator = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        static Sensor hubSensor;

        public void start()
        {
            // connect to server
            toSimulator.Connect(Global.serverAddr);
            hubSensor = new Sensor(toSimulator);

            // 차후에 유저 아이디 구해오는거 구현 예정
            // string userId = toClient.Clients.All.getUserId();
            string userId = "user";
            string str = string.Format("webClient#{0}#Register#All#{0}", userId);

            Sensor.TryParse(str, ref hubSensor);
            hubSensor.socket.SendTo(hubSensor.buffer, hubSensor.whereFrom);

            asyncReceive(hubSensor);
        }

        public void product(string productName, string productNumber)
        {
            JsonUnit productUnit = new JsonUnit(productName, productNumber);
            asyncSend(hubSensor, "Product", hubSensor.messageTarget, productUnit.serialize());
        }

        public void disconnect()
        {
            // 이상하게 작동하지 않는 함수....
            hubSensor.Dispose();
        }

        protected void asyncConnect()
        {
            toSimulator.BeginConnect(Global.serverAddr, connectCallback, toSimulator);
        }

        protected void connectCallback(IAsyncResult ar)
        {
            try
            {
                toSimulator.EndConnect(ar);
            }

            catch
            {
                Console.WriteLine("Fail to connect server. Retry connect");
                asyncConnect();
            }
        }

        void asyncReceive(Sensor sensor)
        {
            sensor.clear();
            sensor.socket.BeginReceiveFrom(sensor.buffer, 0, sensor.length, 0, ref sensor.whereFrom, receiveCallback, sensor);
        }

        void receiveCallback(IAsyncResult ar)
        {
            Sensor inputSensor = ar.AsyncState as Sensor;

            try
            {
                int dataSize = inputSensor.socket.EndReceiveFrom(ar, ref inputSensor.whereFrom);

                if (dataSize > 0)
                {
                    Sensor.TryParse(inputSensor.getBuffer(), ref inputSensor);
                    toClient.Clients.All.printInPage(inputSensor.getBuffer());
                    inputSensor.clear();
                }

                inputSensor.socket.BeginReceiveFrom(inputSensor.buffer, 0, inputSensor.length, 0,
                    ref inputSensor.whereFrom, receiveCallback, inputSensor);
            }

            catch
            {
                toSimulator.Disconnect(true);
                asyncConnect();
            }
        }

        void asyncSend(Sensor sensor, string messageType, string messageTarget, string messageValue)
        {
            sensor.setBuffer(messageType, messageTarget, messageValue);
            sensor.socket.BeginSendTo(sensor.buffer, 0, sensor.length, 0, sensor.whereFrom, sendCallback, sensor);
            sensor.clear();
        }

        void sendCallback(IAsyncResult ar)
        {
            Sensor sendSensor = ar.AsyncState as Sensor;

            try
            {
                sendSensor.socket.EndSendTo(ar);
            }

            catch
            {
                toSimulator.Disconnect(true);
                asyncConnect();
            }
        }
    }
}