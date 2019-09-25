using Dapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace csharp_net_dapper.Dao
{
    public abstract class IDao
    {
        string _connectionString = "";

        public string ConnectionString { get => _connectionString; }

        protected IDao(string connectionString)
        {
            //
            //  Dapper 설정
            //  Object Setter/Getter = Database Columns
            //  Class.UserName = SELECT USER_NAME FROM
            DefaultTypeMap.MatchNamesWithUnderscores = true;
            _connectionString = connectionString;
        }

        
    }
}
