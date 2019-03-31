namespace DataStreamType
{
    /// <summary>
    /// [resource enum] enum for flag about database module type
    /// </summary>
    public enum dataBaseType
    {
        MySql = 1
    }

    /// <summary>
    /// [resource class] class for create database modules
    /// > factory method pattern, singleton pattern
    /// </summary>
    public class DataBaseModuleFactory
    {
        private static DataBaseModuleFactory instance = null;

        private DataBaseModuleFactory() { }

        public static DataBaseModuleFactory getInstance()
        {
            if (instance == null)
                instance = new DataBaseModuleFactory();

            return instance;
        }

        public IDataBase getDataBaseModule(dataBaseType type)
        {
            switch (type)
            {
                case dataBaseType.MySql:
                    return new MySQLModule();

                default:
                    return null;
            }
        }
    }
}