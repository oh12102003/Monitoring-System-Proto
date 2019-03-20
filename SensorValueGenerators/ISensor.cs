using System;
using System.Net.Sockets;
using DataStreamType;
using System.Collections.Generic;

namespace SensorValueGenerator
{
    abstract class ISensor
    {
        protected Socket toServer = null;
        protected string inputSensorName, initSensorValue;
        protected string sensorType, sensorValue;

        protected void resizeConsole()
        {
            Console.SetWindowSize(65, 5);
        }

        public void start()
        {
            resizeConsole();

            // 센서 초기 설정
            getSensorName();
            getInitialValue();

            // 소켓 통신 시작 (데이터 전송 시작)
            using (toServer = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                asyncConnect();
                asyncReceive();

                while (true)
                {
                    changeValue();
                    Console.WriteLine("{0} Sensor : {1}", this.sensorType, inputSensorName);

                    try
                    {
                        asyncSend("notice", sensorValue);
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

        protected abstract void getSensorName();

        protected abstract void getInitialValue();

        protected abstract void changeValue();

        protected abstract void addWork(string numberPerDrink);

        protected void tryAsyncReconnect()
        {
            try
            {
                toServer.Disconnect(true);
                asyncConnect();
            }

            catch
            {
                // 소켓의 connect 자체가 시작되지 않은 케이스
            }
        }

        protected void asyncConnect()
        {
            toServer.BeginConnect(Global.serverAddr, connectCallback, toServer);
        }

        protected void connectCallback(IAsyncResult ar)
        {
            try
            {
                toServer.EndConnect(ar);
                asyncSend("register", inputSensorName);
            }

            catch
            {
                Console.WriteLine("Fail to connect server. Retry connect");
                asyncConnect();
            }
        }

        protected void asyncSend(string _type, string _value)
        {
            try
            {
                Sensor sensor = new Sensor(toServer, inputSensorName);
                sensor.sensorType = this.sensorType;

                sensor.setBuffer(_type, inputSensorName, _value);

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

        protected void sendCallback(IAsyncResult ar)
        {
            try
            {
                Sensor sensor = (Sensor)ar.AsyncState;
                sensor.socket.EndSendTo(ar);

                Console.WriteLine("send data : [" + sensor.getBuffer() + "]");
            }

            catch
            {
                toServer.Disconnect(true);
                asyncConnect();
            }
        }

        void asyncReceive()
        {
            try
            {
                Sensor sensor = new Sensor(toServer, inputSensorName);
                sensor.sensorType = this.sensorType;

                sensor.socket.BeginReceiveFrom(sensor.buffer, 0, sensor.length, 0,
                    ref sensor.whereFrom, receiveCallback, sensor);
            }

            catch
            {
                tryAsyncReconnect();
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
                        addWork(receiveSensor.messageValue);
                    }
                }

                receiveSensor.clear();
                receiveSensor.socket.BeginReceiveFrom(receiveSensor.buffer, 0, receiveSensor.length, 0,
                    ref receiveSensor.whereFrom, receiveCallback, receiveSensor);
            }

            catch
            {
                tryAsyncReconnect();
            }
        }
    }
}
