using System;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace DataStreamType
{


    public class Sensor : IDisposable
    {
        protected const char delimeter = '#';

        public Socket socket { get; private set; }
        public EndPoint whereFrom;

        public string sensorType;
        public string sensorName;
        public string messageType;
        public string messageTarget;
        public string messageValue;

        public byte[] buffer { get; private set; }
        public int length { get { return buffer.Length; } }

        public Sensor(Socket dataSocket, string _sensorName = "unRegistered")
        {
            socket = dataSocket;
            whereFrom = dataSocket.RemoteEndPoint;

            sensorType = "none";
            sensorName = _sensorName;

            messageTarget = "none";
            messageType = "register";

            buffer = new byte[100];
        }

        public static bool TryParse(string str, ref Sensor targetSensor)
        {
            string sType = targetSensor.sensorType;
            string sName = targetSensor.sensorName;
            
            string mType = targetSensor.messageType;
            string mTarget = targetSensor.messageTarget;
            string mValue = targetSensor.messageValue;

            targetSensor.clear();

            try
            {
                string[] parseResult = str.Split(delimeter);

                targetSensor.sensorType = parseResult[0];
                targetSensor.sensorName = parseResult[1];
                targetSensor.setBuffer(parseResult[2], parseResult[3], parseResult[4].TrimEnd('\0'));

                return true;
            }

            catch
            {
                targetSensor.sensorType = sType;
                targetSensor.sensorName = sName;
                targetSensor.setBuffer(mType, mTarget, mValue);
                
                return false;
            }
        }

        public void setBuffer(string type, string target, string value)
        {
            messageType = type;
            messageTarget = target;
            messageValue = value;

            StringBuilder sb = new StringBuilder();
            sb.Append(sensorType); sb.Append(delimeter);
            sb.Append(sensorName); sb.Append(delimeter);
            sb.Append(messageType); sb.Append(delimeter);
            sb.Append(messageTarget); sb.Append(delimeter);
            sb.Append(messageValue);

            buffer = Encoding.Unicode.GetBytes(sb.ToString());
        }

        public string getBuffer()
        {
            return Encoding.Unicode.GetString(buffer);
        }

        /// <summary>
        /// 메시지 패턴 매칭 check 타입과 notCheck 타입이 있으며 check 타입이 있을 시에는 notCheck는 무시
        /// </summary>
        /// <param name="sensorType"> 패턴 매칭을 수행할 센서 타입 값 (기본값 null, 매칭에 포함하지 않음) </param>
        /// <param name="notSensorType"> 해당 데이터가 없는지 패턴 매칭을 수행할 센서 타입 값 (기본값 null, 매칭에 포함하지 않음) </param>
        /// 
        /// <param name="sensorName"> 패턴 매칭을 수행할 센서 이름 값 (기본값 null, 매칭에 포함하지 않음) </param>
        /// <param name="notSensorName"> 해당 데이터가 없는지 패턴 매칭을 수행할 센서 이름 값 (기본값 null, 매칭에 포함하지 않음) </param>
        /// 
        /// <param name="messageType"> 패턴 매칭을 수행할 메시지 타입 값 (기본값 null, 매칭에 포함하지 않음) </param>
        /// <param name="notMessageType"> 해당 데이터가 없는지 패턴 매칭을 수행할 메시지 타입 값 (기본값 null, 매칭에 포함하지 않음) </param>
        /// 
        /// <param name="messageValue"> 패턴 매칭을 수행할 메시지 내용 값 (기본값 null, 매칭에 포함하지 않음) </param>
        /// <param name="notMessageValue"> 해당 데이터가 없는지 패턴 매칭을 수행할 메시지 내용 값 (기본값 null, 매칭에 포함하지 않음) </param>
        /// 
        /// <param name="ipAddr"> 패턴 매칭을 수행할 해당 소켓의 ip:port 값 (기본값 null, 매칭에 포함하지 않음) </param>
        /// <param name="notIpAddr"> 해당 데이터가 없는지 패턴 매칭을 수행할 ip:port 값 (기본값 null, 매칭에 포함하지 않음) </param>
        /// 
        /// <returns> 매칭 되었는지에 대한 true / false </returns>
        public bool patternMatching(string sensorType = null, string notSensorType = null,
            string sensorName = null, string notSensorName = null,
            string messageType = null, string notMessageType = null,
            string messageValue = null, string notMessageValue = null,
            string ipAddr = null, string notIpAddr = null)
        {
            bool result = true;

            // sensor에 대한 체크
            result &= returnCheck(result, this.sensorType, sensorType, notSensorType);
            result &= returnCheck(result, this.sensorName, sensorName, notSensorName);

            // message에 대한 체크
            result &= returnCheck(result, this.messageType, messageType, notMessageType);
            result &= returnCheck(result, this.messageValue, messageValue, notMessageValue);

            // ip 주소에 대한 검수
            result &= returnCheck(result, this.getIP(), ipAddr, notIpAddr);
            return result;
        }

        private bool returnCheck(bool beforeChecker, string checkTarget, string isCheck, string isNotCheck)
        {
            return (isCheck != null) ? isCheck.Equals(checkTarget, StringComparison.OrdinalIgnoreCase) :
                (isNotCheck != null) ? Not(isNotCheck.Equals(checkTarget, StringComparison.OrdinalIgnoreCase)) : beforeChecker;
        }

        private bool Not(bool target)
        {
            return !target;
        }

        override public string ToString()
        {
            return getIP() + " : " + getBuffer();
        }

        public string getIP()
        {
            return whereFrom.ToString();
        }

        public void clear()
        {
            buffer = new byte[1024];
        }

        public void Dispose()
        {
            socket.Dispose();
        }
    }
}
