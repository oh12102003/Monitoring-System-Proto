using System;
using DataStreamType;
using System.Collections.Generic;

namespace SensorValueGenerator
{
    sealed class MaterialSensor : ISensor
    {
        Queue<double> workQueue = new Queue<double>();

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

        override protected void changeValue()
        {
            if (workQueue.Count > 0)
            {
                double amount = workQueue.Dequeue();
                this.sensorValue = (double.Parse(sensorValue) - amount).ToString();
            }
        }

        protected override void addWork(string jsonString)
        {
            JsonUnit work = JsonUnit.Parse(jsonString);
            int workNumber = int.Parse(work.value);

            Random rand = new Random();

            JsonStream js = new JsonStream();
            double amount = double.Parse(js.getAmount(work.name, inputSensorName));

            for (int i = 0; i < workNumber; i++)
            {
                double randAmount = Math.Round(amount + rand.NextDouble(), 3);
                workQueue.Enqueue(randAmount);
            }
        }
    }
}
