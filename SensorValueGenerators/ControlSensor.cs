using System;
using DataStreamType;
using System.Collections.Generic;

namespace SensorValueGenerator
{
    sealed class ControlSensor : ISensor
    {
        Queue<double> workQueue = new Queue<double>();

        public ControlSensor()
        {
            this.sensorType = "Control";
        }

        protected override void getSensorName()
        {
            do
            {
                Console.Write("Put this control(env.) name : ");
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

        override protected void changeValue()
        {
            double tempNum = double.Parse(this.sensorValue);
            double variance;

            if (workQueue.Count > 0)
            {
                variance = workQueue.Dequeue();
                tempNum += variance;
            }

            else
            {
                Random rand = new Random();
                variance = rand.NextDouble();
                tempNum += (variance > 0.5) ? (variance - 0.5) : -variance;
            }

            this.sensorValue = Math.Round(tempNum, 3).ToString();
        }

        protected override void addWork(string jsonString)
        {
            JsonUnit work = JsonUnit.Parse(jsonString);
            int number = int.Parse(work.value);

            for (int i = 0; i < number; i++)
            {
                workQueue.Enqueue(0.2);
            }
        }
    }
}
