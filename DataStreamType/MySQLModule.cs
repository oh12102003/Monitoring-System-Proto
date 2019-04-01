using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Data;

namespace DataStreamType
{
    /// <summary>
    /// [resource class] class for using mysql
    /// > adapter pattern(adaptee)
    /// </summary>
    class MySQLModule : IDataBase
    {
        MySqlConnection connection;

        public MySQLModule() { }

        // db 연결
        override public void connect()
        {
            const string connectionString = "Server=localhost; Database=monitoring; Uid=root; Pwd=1234; Charset=utf8";
            connection = new MySqlConnection(connectionString);

            connection.Open();
        }

        // db 연결 해제
        override public void close()
        {
            connection.Close();
        }

        // 데이터 조회 함수
        public override int inquire(ref DataTable result, string inputQuery, List<MySqlParameter> queryStringData)
        {
            MySqlCommand command = new MySqlCommand(inputQuery, connection);

            foreach (MySqlParameter param in queryStringData)
            {
                command.Parameters.Add(param);
            }

            MySqlDataReader readData = command.ExecuteReader();

            result.Load(readData);
            return result.Rows.Count;
        }

        public override int inquire(string inputQuery, List<MySqlParameter> queryStringData)
        {
            DataTable result = new DataTable();
            MySqlCommand command = new MySqlCommand(inputQuery, connection);

            foreach (MySqlParameter param in queryStringData)
            {
                command.Parameters.Add(param);
            }

            MySqlDataReader readData = command.ExecuteReader();

            result.Load(readData);
            return result.Rows.Count;
        }

        override public int inquire(ref DataTable result, string inputQuery)
        {
            MySqlCommand command = new MySqlCommand(inputQuery, connection);

            MySqlDataReader readData = command.ExecuteReader();

            result.Load(readData);
            return result.Rows.Count;
        }

        // 데이터 추가, 수정, 삭제 함수
        override public int update(string inputQuery, List<MySqlParameter> queryStringData)
        {
            try
            {
                MySqlCommand command = new MySqlCommand(inputQuery, connection);

                foreach (MySqlParameter param in queryStringData)
                {
                    command.Parameters.Add(param);
                }

                return command.ExecuteNonQuery();
            }

            catch
            {
                return -1;
            }
        }
    }
}
