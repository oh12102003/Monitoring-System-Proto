using System;
using System.Net.Sockets;
using DataStreamType;

namespace SensorValueGenerator
{
    sealed class MaterialSensor : ISensor
    {
        double amount;

        public MaterialSensor()
        {
            this.sensorType = "Material";
        }

        protected override void getSensorName()
        {
            do
            {
                Console.Write("Put this material name : ");
                inputSensorName = Console.ReadLine();
            } while (inputSensorName.Trim().Equals(""));
        }

        protected override void getInitialValue()
        {
            double tempValue;

            do
            {
                Console.Write("Put initial value (double value) : ");
                initSensorValue = Console.ReadLine();
            } while (!double.TryParse(initSensorValue, out tempValue));

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
            this.sensorValue = (double.Parse(sensorValue) - amount).ToString();
        }

        void asyncReceive()
        {
            try
            {
                Sensor sensor = new Sensor(toServer, inputSensorName);
                sensor.sensorType = sensorType;

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
                        Console.WriteLine("Consume {0} for product {1}", inputSensorName, receiveSensor.messageValue);

                        JsonStream js = new JsonStream();
                        amount = double.Parse(js.getAmount(receiveSensor.messageValue, inputSensorName));

                        changeValue();
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
