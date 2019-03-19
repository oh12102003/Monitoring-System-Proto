using System;
using System.Net.Sockets;
using DataStreamType;

namespace SensorValueGenerator_Vessel
{
    class Program
    {
        static void Main(string[] args)
        {
            VesselSensor sensor = new VesselSensor();
            sensor.start();
        }
    }

    class VesselSensor
    {
        static Socket toServer = null;
        string inputSensorName, initSensorValue;
        int sensorValue = 1;

        public void start()
        {
            Console.SetWindowSize(65, 5);

            do
            {
                Console.Write("Put this vessel name : ");
                inputSensorName = Console.ReadLine();
            } while (inputSensorName.Trim().Equals(""));

            // 정상 입력이 올때까지 입력 반복
            do
            {
                Console.Write("Put initial value (int value) : ");
                initSensorValue = Console.ReadLine();
            } while (!int.TryParse(initSensorValue, out sensorValue));

            // 소켓 통신 시작 (데이터 전송 시작)
            using (toServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                asyncConnect();

                while (true)
                {
                    Console.WriteLine("Vessel Sensor : " + inputSensorName);

                    try
                    {
                        asyncSend("notice", sensorValue.ToString());
                    }

                    catch
                    {
                        Console.WriteLine("No server appended.");
                    }

                    finally
                    {
                        System.Threading.Thread.Sleep(200);
                        Console.Clear();
                    }
                }
            }
        }

        void changeValue()
        {
            this.sensorValue--;
        }

        void asyncConnect()
        {
            toServer.BeginConnect(Global.serverAddr, connectCallback, toServer);
        }

        void connectCallback(IAsyncResult ar)
        {
            try
            {
                toServer.EndConnect(ar);
                asyncSend("register", inputSensorName);
                asyncReceive();

                Console.WriteLine("Success to connect server.");
            }

            // 서버가 열리지 않아서 연결이 되지 않을 시 연결 재시도
            catch
            {
                Console.WriteLine("Fail to connect server. Retry connect");
                asyncConnect();
            }
        }

        void asyncSend(string _type, string _value)
        {
            try
            {
                Sensor sensor = new Sensor(toServer, inputSensorName);
                sensor.sensorType = "vessel";
                sensor.setBuffer(_type, _value);

                sensor.socket.BeginSendTo(sensor.buffer, 0, sensor.length, 0,
                    sensor.whereFrom, sendCallback, sensor);
            }

            catch
            {
                // 연결에 문제가 발생했을 시 소켓 연결 해제 및 비동기 방식 재연결 시도
                toServer.Disconnect(true);
                asyncConnect();
            }
        }

        void sendCallback(IAsyncResult ar)
        {
            Sensor sensor = (Sensor)ar.AsyncState;
            sensor.socket.EndSendTo(ar);

            Console.WriteLine("send data : [" + sensor.getBuffer() + "]");
        }

        void asyncReceive()
        {
            try
            {
                Sensor sensor = new Sensor(toServer, inputSensorName);
                sensor.sensorType = "vessel";

                sensor.socket.BeginReceiveFrom(sensor.buffer, 0, sensor.length, 0,
                    ref sensor.whereFrom, receiveCallback, sensor);
            }

            catch
            {
                toServer.Disconnect(true);
                asyncConnect();
            }
        }

        void receiveCallback(IAsyncResult ar)
        {
            Sensor receiveSensor = (Sensor)ar.AsyncState;

            try
            {
                int recvData = receiveSensor.socket.EndReceiveFrom(ar, ref receiveSensor.whereFrom);

                if (recvData > 0)
                {
                    Sensor.TryParse(receiveSensor.getBuffer(), ref receiveSensor);
                    Console.WriteLine("received data : " + receiveSensor.getBuffer());

                    if (receiveSensor.patternMatching(messageType: "make"))
                    {
                        Console.WriteLine("Consume {0} for product {1}", inputSensorName, receiveSensor.messageValue);
                        changeValue();
                    }
                }

                receiveSensor.clear();
                receiveSensor.socket.BeginReceiveFrom(receiveSensor.buffer, 0, receiveSensor.length, 0,
                    ref receiveSensor.whereFrom, receiveCallback, receiveSensor);
            }

            catch
            {
                toServer.Disconnect(true);
                asyncConnect();
            }
        }
    }
}
