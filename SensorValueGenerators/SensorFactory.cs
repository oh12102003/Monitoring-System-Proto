using System;

namespace SensorValueGenerator
{
    class SensorFactory
    {
        private SensorFactory() { }

        public static ISensor createSensor(string sensorType)
        {
            if (sensorType.Equals("material", StringComparison.OrdinalIgnoreCase))
            {
                return new MaterialSensor();
            }

            else if (sensorType.Equals("control", StringComparison.OrdinalIgnoreCase))
            {
                return new ControlSensor();
            }

            else if (sensorType.Equals("vessel", StringComparison.OrdinalIgnoreCase))
            {
                return new VesselSensor();
            }

            else
            {
                Console.WriteLine("올바르지 않은 타입입니다.");
                return null;
            }
        }
    }
}
