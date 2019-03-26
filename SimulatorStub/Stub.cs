using DataStreamType;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;

namespace SimulatorStub
{
    class Stub
    {
        static void Main(string[] args)
        {
            string reading;

            do
            {
                Console.WriteLine("Manual : middleServer(s) or sensorGenerator(g) or consoleStub(t)");
                Console.WriteLine("Console stub means using this console as stub program.");
                Console.Write("Input : ");
                reading = Console.ReadLine();

                if (reading.Equals("g", StringComparison.OrdinalIgnoreCase))
                {
                    using (var process = new Process())
                    {
                        process.StartInfo.FileName = @"..\..\..\SensorValueGenerators\bin\Debug\SensorValueGenerator.exe";
                        process.Start();
                    }
                }

                else if (reading.Equals("s", StringComparison.OrdinalIgnoreCase))
                {
                    using (var process = new Process())
                    {
                        process.StartInfo.FileName = @"..\..\..\MiddleServer\bin\Debug\MiddleServer.exe";
                        process.Start();
                    }
                }

                Console.Clear();

            } while (!reading.Equals("t", StringComparison.OrdinalIgnoreCase));

            consoleStub();
        }

        static void consoleStub()
        {
            createRecipe();
            Console.SetWindowSize(65, 20);

            bool isRegistered = false;
            string clientCommand;
            IPEndPoint endPoint = new IPEndPoint(IPAddress.Parse("127.0.0.1"), 8888);

            Socket toSimulator = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            Sensor drinkSensor;

            // start to conect server
            Console.Write("Write the user name : ");
            string userName = Console.ReadLine();

            // connect to server
            toSimulator.Connect(endPoint);
            drinkSensor = new Sensor(toSimulator);

            try
            {
                while (true)
                {
                    Console.Write("Command : ");
                    clientCommand = Console.ReadLine();

                    if (!isRightCommand(clientCommand))
                    {
                        Console.WriteLine("Incorrect command\r\n");
                    }

                    else if (clientCommand.Equals("Exit", StringComparison.OrdinalIgnoreCase))
                    {
                        break;
                    }

                    else
                    {
                        if (clientCommand.Equals("register", StringComparison.OrdinalIgnoreCase))
                        {
                            string str = string.Format("webClient#{0}#Register#All#{0}", userName);

                            Sensor.TryParse(str, ref drinkSensor);
                            isRegistered = true;
                            drinkSensor.socket.SendTo(drinkSensor.buffer, drinkSensor.whereFrom);
                        }

                        else if (clientCommand.Equals("Product", StringComparison.OrdinalIgnoreCase) && isRegistered)
                        {
                            /*
                            Console.Write("생산 대상 : ");
                            string productName = Console.ReadLine();

                            Console.Write("생산 개수 : ");
                            string productNumber = Console.ReadLine();*/
                            string productName = "패트병";
                            string productNumber = "20";

                            JsonUnit productUnit = new JsonUnit(productName, "drink", productNumber);
                            drinkSensor.setBuffer("Product", drinkSensor.messageTarget, productUnit.serialize());
                            Console.WriteLine(drinkSensor.getBuffer());
                            drinkSensor.socket.SendTo(drinkSensor.buffer, drinkSensor.whereFrom);
                        }

                        else if (clientCommand.Equals("Show", StringComparison.OrdinalIgnoreCase) && isRegistered)
                        {
                            drinkSensor.setBuffer(drinkSensor.messageTarget, "Show", "All");
                            drinkSensor.socket.SendTo(drinkSensor.buffer, drinkSensor.whereFrom);

                            drinkSensor.clear();
                            drinkSensor.socket.ReceiveFrom(drinkSensor.buffer, ref drinkSensor.whereFrom);

                            Console.WriteLine("Received Status");
                            Console.WriteLine("{0}", drinkSensor.getBuffer());
                        }
                    }
                }
            }
            
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
        }

        public static bool isRightCommand(string str)
        {
            if (str.Equals("Exit", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            else if (str.Equals("Product", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            else if (str.Equals("Register", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            else if (str.Equals("Show", StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }

            return false;
        }

        static void createRecipe()
        {
            JsonStream js = new JsonStream("drink");

            List<AmountPerDrinks> chilsung = new List<AmountPerDrinks>();
            List<AmountPerDrinks> pepsi = new List<AmountPerDrinks>();
            List<AmountPerDrinks> water = new List<AmountPerDrinks>();
            List<AmountPerDrinks> controlTest = new List<AmountPerDrinks>();
            List<AmountPerDrinks> vesselTest = new List<AmountPerDrinks>();

            chilsung.Add(new AmountPerDrinks("1.5L 칠성사이다 패트병", "1"));
            chilsung.Add(new AmountPerDrinks("탄산 혼합물", "20"));
            chilsung.Add(new AmountPerDrinks("구연산", "30"));

            pepsi.Add(new AmountPerDrinks("1.5L 팹시 패트병", "1"));
            pepsi.Add(new AmountPerDrinks("탄산 혼합물", "15"));
            pepsi.Add(new AmountPerDrinks("혼합제제", "50"));

            water.Add(new AmountPerDrinks("패트병", "1"));
            water.Add(new AmountPerDrinks("물", "20"));

            vesselTest.Add(new AmountPerDrinks("패트병", "1"));

            js.applyInputData("패트병", vesselTest);
            js.applyInputData("테스트", controlTest);
            js.applyInputData("생수", water);
            js.applyInputData("칠성사이다", chilsung);
            js.applyInputData("펩시", pepsi);
        }
    }
}
