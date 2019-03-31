using MySql.Data.MySqlClient;
using System.Collections.Generic;
using System.Data;

namespace DataStreamType
{
    /// <summary>
    /// [resource abstract class] class for using database
    /// > adapter pattern(adapter)
    /// </summary>
    public abstract class IDataBase
    {
        // connect & close connection
        abstract public void connect();
        abstract public void close();

        // execute query
        abstract public int inquire(ref DataTable result, string inputQuery, List<MySqlParameter> queryStringData);
        abstract public int inquire(string inputQuery, List<MySqlParameter> queryStringData);
        abstract public int update(string inputQuery, List<MySqlParameter> queryStringData);
    }
}
