using csharp_net_dapper.Dao;
using csharp_net_dapper.Domain;
using Dapper;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace csharp_net_dapper.Dao
{
    public class UserDao : IDao
    {
        public UserDao(string connectionString) : base(connectionString)
        {
        }

        public int Insert(User.Default user)
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                return conn.Execute(@"INSERT INTO TB_USER(ID,  USER_NAME, USER_AGE, CREATE_DT, UPDATE_DT )
                                      VALUES             (@Id, @UserName, @UserAge, @CreateDt, @UpdateDt )", user);
            }
        }

        public int InsertCustomMapping(User.Default user)
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                return conn.Execute(@"INSERT INTO TB_USER(ID,  USER_NAME, USER_AGE, CREATE_DT, UPDATE_DT )
                                      VALUES             (@Id, @Name, @Age, @CDate, @UDate )"
                                    , new { Id=user.Id, Name=user.UserName, Age=user.UserAge, CDate=user.CreateDt, UDate=user.UpdateDt });
            }
        }

        public int Update(User.Default user)
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                return conn.Execute(@"UPDATE    TB_USER
                                      SET       USER_NAME   = @UserName,
                                                USER_AGE    = @UserAge,
                                                UPDATE_DT   = @UpdateDt
                                      WHERE     ID = @Id
                                    ", user);
            }
        }

        public User.Default Select(User.Default user)
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                return conn.QuerySingle<User.Default>("SELECT * FROM TB_USER WHERE ID=@Id", user);
            }
        }

        public List<User.Default> Select()
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                return conn.Query<User.Default>("SELECT * FROM TB_USER").ToList();
            }
        }

        public int Delete()
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                return conn.Execute("DELETE FROM TB_USER");
            }
        }

        public int Delete(User.Default user)
        {
            using (var conn = new SqlConnection(ConnectionString))
            {
                return conn.Execute("DELETE FROM TB_USER WHERE ID=@Id", user);
            }
        }

        
    }
}
