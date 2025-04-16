using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MySqlConnector;

namespace Library_Manager.Data
{
    public class Database
    {
        "ConnectionStrings": {
    "MariaDB": "Server=yourserver;Database=yourdb;User=youruser;Password=yourpassword;"
}

        public MySqlConnection GetConnection()
        {
            return new MySqlConnection(_connectionString);
        }
    }
}
