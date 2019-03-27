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

                foreach (char readChar in reading)
                {
                    if (readChar == 'g' || readChar == 'G')
                    {
                        using (var process = new Process())
                        {
                            process.StartInfo.FileName = @"..\..\..\SensorValueGenerators\bin\Debug\SensorValueGenerator.exe";
                            process.Start();
                        }
                    }

                    else if (readChar == 's' || readChar == 'S')
                    {
                        using (var process = new Process())
                        {
                            process.StartInfo.FileName = @"..\..\..\MiddleServer\bin\Debug\MiddleServer.exe";
                            process.Start();
                        }
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

            DrinkList testDrinkList = new DrinkList();

            Drink chilsung = new Drink("칠성사이다");
            chilsung.addIngredient(new AmountPerDrinks("1.5 칠성사이다 패트병", "1"));
            chilsung.addIngredient(new AmountPerDrinks("탄산 혼합물", "20"));
            chilsung.addIngredient(new AmountPerDrinks("구연산", "30"));
            testDrinkList.Add(chilsung);

            Drink pepsi = new Drink("펩시");
            pepsi.addIngredient(new AmountPerDrinks("1.5L 팹시 패트병", "1"));
            pepsi.addIngredient(new AmountPerDrinks("탄산 혼합물", "15"));
            pepsi.addIngredient(new AmountPerDrinks("혼합제제", "50"));
            testDrinkList.Add(pepsi);

            Drink orange = new Drink("델몬트");
            orange.addIngredient(new AmountPerDrinks("1.5L 주스 패트병", "1"));
            orange.addIngredient(new AmountPerDrinks("정제수", "50"));
            orange.addIngredient(new AmountPerDrinks("오렌지 과즙", "5"));
            testDrinkList.Add(orange);

            Drink water = new Drink("생수");
            water.addIngredient(new AmountPerDrinks("패트병", "1"));
            water.addIngredient(new AmountPerDrinks("물", "20"));
            testDrinkList.Add(water);

            Drink vesselTest = new Drink("패트병");
            vesselTest.addIngredient(new AmountPerDrinks("패트병", "1"));
            testDrinkList.Add(vesselTest);

            js.applyInputData(testDrinkList);
        }
    }
}
