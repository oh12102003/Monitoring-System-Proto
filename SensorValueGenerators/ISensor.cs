using System;
using System.Net.Sockets;
using DataStreamType;

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
                setSensorSettings();

                while (true)
                {
                    printSensorStatus();

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

        protected virtual void setSensorSettings()
        {

        }

        protected abstract void getSensorName();

        protected abstract void getInitialValue();

        protected abstract void changeValue();

        protected abstract void printSensorStatus();

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

        protected void sendCallback(IAsyncResult ar)
        {
            Sensor sensor = (Sensor)ar.AsyncState;
            sensor.socket.EndSendTo(ar);

            Console.WriteLine("send data : [" + sensor.getBuffer() + "]");
        }
    }
}
