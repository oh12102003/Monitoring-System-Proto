using System;
using System.Net.Sockets;
using DataStreamType;

namespace SensorValueGenerator
{
    sealed class ControlSensor : ISensor
    {
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

        protected override void printSensorStatus()
        {
            Console.WriteLine("{0} Sensor : {1}", this.sensorType, inputSensorName);
            changeValue();
        }

        override protected void changeValue()
        {
            Random rand = new Random();
            double tempNum = double.Parse(this.sensorValue);
            double randNum = rand.NextDouble();

            tempNum += (randNum > 0.5) ? (randNum - 0.5) : -randNum;
            this.sensorValue = Math.Round(tempNum, 3).ToString();
        }
    }
}
