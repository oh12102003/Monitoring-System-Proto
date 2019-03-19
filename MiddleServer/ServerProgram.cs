using DataStreamType;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;

namespace CometServer_MiddleServer
{
    class ServerProgram
    {
        static void Main(string[] args)
        {
            Server server = new Server();
            server.start();
        }
    }

    // callback을 통해서 계속하여 센서의 소켓 통신 요청을 accept
    class Server
    {
        static List<Sensor> sensorList;
        static Socket serverSocket;
        static JsonStream drinkIO;

        // listening 소켓 생성
        public void start()
        {
            sensorList = new List<Sensor>();
            drinkIO = new JsonStream("drink");

            Console.SetWindowSize(65, 20);

            using (serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp))
            {
                // start the server, and accept sensor, webclient sockets
                serverSocket.Bind(Global.serverAddr);
                serverSocket.Listen(5);
                asyncAccept();

                while (true)
                {
                    // check all of socket connection
                    sensorList.ForEach(sensor =>
                    {
                        if (!sensor.socket.Connected)
                        {
                            sensorList.Remove(sensor);
                            sensor.Dispose();
                        }
                    });

                    try
                    {
                        Console.WriteLine("Connected sensor list");
                        printsensorList();
                    }


                    catch (SocketException e)
                    {
                        Console.WriteLine("Recover the listening socket");
                        Console.WriteLine(e.StackTrace);

                        // recovery
                        serverSocket.Disconnect(true);
                        serverSocket.Bind(Global.serverAddr);
                        serverSocket.Listen(5);

                        asyncAccept();
                    }

                    finally
                    {
                        System.Threading.Thread.Sleep(200);
                        Console.Clear();
                    }
                }
            }
        }

        /// <summary>
        /// 비동기 방식 accept 수행
        /// </summary>
        void asyncAccept()
        {
            serverSocket.BeginAccept(acceptCallback, serverSocket);
        }

        /// <summary>
        /// 비동기 방식 accept 에 대한 콜백 함수
        /// [주의] asyncAccept 의해서만 호출될 수 있도록 할 것!
        /// </summary>
        /// <param name="ar"> endAccept에 대한 값 </param>
        void acceptCallback(IAsyncResult ar)
        {
            try
            {
                Socket sensorSocket = serverSocket.EndAccept(ar);
                Sensor sensorSensor = new Sensor(sensorSocket);
                Sensor.TryParse(sensorSensor.getBuffer(), ref sensorSensor);
                sensorSensor.clear();

                // 새로운 Sensor에 receive 비동기 작동 방식 추가
                asyncReceive(sensorSensor);

                // 서버의 관리 리스트에 append
                sensorList.Add(sensorSensor);
            }

            catch
            {
                Console.WriteLine("Error occured while accept sensor or client");
            }

            finally
            {
                serverSocket.BeginAccept(acceptCallback, serverSocket);
            }
        }


        /// <summary>
        /// 비동기 방식 receive 수행
        /// </summary>
        /// <param name="sensor"> 수신 받을 센서 혹은 클라이언트</param>
        void asyncReceive(Sensor sensor)
        {
            sensor.clear();
            sensor.socket.BeginReceiveFrom(sensor.buffer, 0, 0, 0, ref sensor.whereFrom, receiveCallback, sensor);
        }

        /// <summary>
        /// 비동기 방식 receive 에 대한 콜백 함수
        /// [주의] asyncReceive에 의해서만 호출될 수 있도록 할 것!
        /// </summary>
        /// <param name="ar"> endReceive 대한 값 </param>
        void receiveCallback(IAsyncResult ar)
        {
            Sensor inputSensor = ar.AsyncState as Sensor;

            try
            {
                int dataSize = inputSensor.socket.EndReceiveFrom(ar, ref inputSensor.whereFrom);

                // 데이터를 받았을 시
                if (dataSize > 0)
                {
                    Sensor.TryParse(inputSensor.getBuffer(), ref inputSensor);
                    inputSensor.clear();

                    // material, vessel 센서에 보내는 생산 명령 (무시)
                    if (inputSensor.patternMatching(messageType: "make")) { }

                    // 센서 및 클라이언트 등록에 대한 동작
                    // [sensorType sensorName "register" "register"]
                    else if (inputSensor.patternMatching(messageType: "register"))
                    {
                        bool checker = false;

                        foreach (var sensor in sensorList)
                        {
                            if (sensor.patternMatching(sensorName: inputSensor.sensorName))
                            {
                                checker |= sensor.patternMatching(ipAddr: inputSensor.getIP());
                            }
                        }

                        if (checker)
                        {
                            Console.WriteLine("Duplicate name. Access denied : " + inputSensor.sensorName);
                        }

                        else
                        {
                            Sensor targetSensor = sensorList.Find(sensor =>
                            {
                                return sensor.patternMatching(ipAddr: inputSensor.getIP());
                            });

                            Sensor.TryParse(inputSensor.getBuffer(), ref targetSensor);
                            targetSensor.clear();

                            Console.WriteLine("Register sensor : " + targetSensor.sensorName);
                        }
                    }

                    // only from client!!!
                    // [sensorType 음료수명 "product" 생산개수]
                    // 등록된 웹 클라이언트의 생산 명령에 따른 동작
                    else if (inputSensor.patternMatching(messageType: "product", sensorType: "webClient"))
                    {
                        string drinkName = inputSensor.sensorName;

                        // 등록된 음료수 인지 확인
                        if (!drinkIO.isRegisteredDrink(drinkName))
                        {
                            Console.WriteLine("There is no drink with name " + drinkName);
                        }

                        else
                        {
                            string productNumber = inputSensor.messageValue;

                            // 재료 재고량 확인
                            List<AmountPerDrinks> recipeList = drinkIO.getAmountPerDrinks(drinkName);
                            bool checker = true;

                            foreach (var recipe in recipeList)
                            {
                                // 센서의 값 가져오기
                                Sensor targetSensor = sensorList.Find(sensor => sensor.patternMatching(sensorName: recipe.ingredient));

                                if (targetSensor == null)
                                {
                                    checker = false;
                                }

                                else
                                {
                                    string value = targetSensor.messageValue;
                                    checker &= (double.Parse(value) >= double.Parse(recipe.amount) * double.Parse(productNumber));
                                }
                            }

                            // 모두 충족시
                            if (checker)
                            {
                                foreach(var recipe in recipeList)
                                {
                                    Sensor targetSensor = sensorList.Find(sensor => sensor.patternMatching(sensorName: recipe.ingredient));
                                    asyncSend(targetSensor, "make", drinkName);
                                }
                            }

                            else
                            {
                                Console.WriteLine("Cannot produce the drink as lack of ingredients.");
                            }
                        }
                    }


                    // only from client!!!
                    // [webClient 음료수명 "show" 보여줄 센서]
                    // 등록된 웹 클라이언트에게 센서 현황을 보냄
                    else if (inputSensor.patternMatching(messageType : "show", sensorType : "webClient"))
                    {
                        StringBuilder sb = new StringBuilder();
                        bool hasData = false;

                        if (inputSensor.patternMatching(messageValue : "All"))
                        {
                            sensorList.ForEach(sensor =>
                            {
                                if (sensor.patternMatching(notSensorType: "webClient"))
                                {
                                    if (hasData)
                                    {
                                        sb.Append(", ");
                                    }

                                    hasData = true;
                                    sb.Append(string.Format("{0}~{1}", sensor.sensorName, sensor.messageValue));
                                }
                            });
                        }

                        else
                        {
                            Sensor targetSensor = sensorList.Find(sensor => sensor.patternMatching(sensorName: inputSensor.messageValue));

                            if (targetSensor != null)
                            {
                                hasData = true;
                                sb.AppendLine(string.Format("{0}~{1}", targetSensor.sensorName, targetSensor.messageValue));
                            }
                        }

                        if (!hasData)
                        {
                            sb.AppendLine("No data");
                        }

                        asyncSend(inputSensor, "Respond", sb.ToString());
                    }

                    // only from sensors!!!
                    // [sensorType sensorName "notice" 현재상태]
                    // 각종 센서들이 주기적으로 보내는 현재 상태 값들을 현재 센서 리스트에 반영
                    else if (inputSensor.patternMatching(messageType: "notice", notSensorType : "webClient"))
                    {
                        Sensor targetSensor
                            = sensorList.Find(sensor => sensor.patternMatching(sensorName: inputSensor.sensorName));

                        // just apply
                        targetSensor.clear();
                        Sensor.TryParse(inputSensor.getBuffer(), ref targetSensor);
                    }

                    else
                    {
                        Console.WriteLine("Unknown message type");
                        Console.WriteLine("Message Type : " + inputSensor.messageType);
                        Console.WriteLine(inputSensor.getBuffer());
                    }
                }

                inputSensor.clear();
                inputSensor.socket.BeginReceiveFrom(inputSensor.buffer, 0, inputSensor.length, 0,
                    ref inputSensor.whereFrom, receiveCallback, inputSensor);
            }
            
            catch
            {
                // 소켓 통신에 문제 발생시 소켓 연결 해제
                sensorList.Remove(inputSensor);
                inputSensor.Dispose();
            }
        }

        /// <summary>
        /// 비동기 방식 send 수행
        /// </summary>
        /// <param name="sensor"> 데이터를 보낼 센서</param>
        /// <param name="messageType"> 메시지의 타입</param>
        /// <param name="messageValue"> 메시지의 값 </param>
        void asyncSend(Sensor sensor, string messageType, string messageValue)
        {
            sensor.clear();
            sensor.setBuffer(messageType, messageValue);
            sensor.socket.BeginSendTo(sensor.buffer, 0, sensor.length, 0, sensor.whereFrom, sendCallback, sensor);
        }

        /// <summary>
        /// 비동기 방식 send 에 대한 콜백 함수
        /// [주의] asyncSend에 의해서만 호출될 수 있도록 할 것!
        /// </summary>
        /// <param name="ar"> endAccept에 대한 값 </param>
        static void sendCallback(IAsyncResult ar)
        {
            Sensor sendSensor = ar.AsyncState as Sensor;

            try
            {
                sendSensor.socket.EndSendTo(ar);
                sendSensor.clear();
                Sensor.TryParse(sendSensor.getBuffer(), ref sendSensor);

                Console.WriteLine("{0} _ send to {1}", sendSensor.getBuffer(), sendSensor.whereFrom);
            }

            catch
            {
                sensorList.Remove(sendSensor);
                sendSensor.Dispose();
            }
        }

        /// <summary>
        /// 현재 미들 서버에 연결되어 있는 센서 및 클라이언트를 모두 출력하는 함수
        /// </summary>
        static void printsensorList()
        {
            Console.WriteLine("------------------------------------------");
            Console.WriteLine("Sensor Status");

            if (sensorList.Count == 0)
            {
                Console.WriteLine("Empty");
            }

            else
            {
                sensorList.ForEach(sensor =>
                {
                    Console.WriteLine(string.Format("{0} [{1} : {2}] : -({3}){4}-", sensor.getIP(), sensor.sensorType, sensor.sensorName, sensor.messageType, sensor.messageValue));
                });
            }

            Console.WriteLine("------------------------------------------");
        }
    }
}
