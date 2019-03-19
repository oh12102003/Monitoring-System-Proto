using System;
using System.Net.Sockets;
using DataStreamType;

namespace SensorValueGenerator
{
    sealed class VesselSensor : ISensor
    {
        public VesselSensor()
        {
            sensorType = "Vessel";
        }

        protected override void getSensorName()
        {
            do
            {
                Console.Write("Put this vessel name : ");
                inputSensorName = Console.ReadLine();
            } while (inputSensorName.Trim().Equals(""));
        }

        protected override void getInitialValue()
        {
            int tempValue;

            do
            {
                Console.Write("Put initial value (int value) : ");
                initSensorValue = Console.ReadLine();
            } while (!int.TryParse(initSensorValue, out tempValue));

            sensorValue = tempValue.ToString();
        }

        protected override void setSensorSettings()
        {
            asyncReceive();
        }

        protected override void printSensorStatus()
        {
            Console.WriteLine("{0} Sensor : {1}", this.sensorType, inputSensorName);
        }

        override protected void changeValue()
        {
            this.sensorValue = (int.Parse(this.sensorValue) - 1).ToString();
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
