using System;
using DataStreamType;
using System.Collections.Generic;

namespace SensorValueGenerator
{
    sealed class VesselSensor : ISensor
    {
        Queue<int> workQueue = new Queue<int>();

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

        protected override void addWork(string jsonString)
        {
            int number = int.Parse(JsonUnit.Parse(jsonString).value);

            for (int i = 0; i < number; i++)
            {
                workQueue.Enqueue(1);
            }
        }

        override protected void changeValue()
        {
            if (workQueue.Count > 0)
            {
                int amount = workQueue.Dequeue();
                this.sensorValue = (int.Parse(this.sensorValue) - amount).ToString();
            }
        }
    }
}
