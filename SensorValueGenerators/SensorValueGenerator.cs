using System;

namespace SensorValueGenerator
{
    class SensorValueGenerator
    {
        public static void Main(string[] args)
        {
            string sensorInput;
            ISensor sensor;

            while (true)
            {
                Console.SetWindowSize(65, 5);

                Console.Write("Input sensor name : ");
                sensorInput = Console.ReadLine();

                sensor = SensorFactory.createSensor(sensorInput);

                if (sensor != null)
                {
                    break;
                }
            }

            sensor.start();
        }
    }
}
