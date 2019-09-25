using csharp_net_dapper.Dao;
using csharp_net_dapper.Domain;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace csharp_net_dapper
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : Window
    {
        UserDao userDao = new UserDao("Data Source=127.0.0.1,1433;Initial Catalog=eics;User ID=eics;Password=eics");

        public MainWindow()
        {
            InitializeComponent();

            DapperTest();
        }

        private void DapperTest()
        {
            TestInsert();
            TestUpdate();
            TestSelect();
            TestDeleteOne();
            TestDeleteAll();
        }

        private void TestDeleteAll()
        {
            int countDeleteAll = userDao.Delete();
            Console.WriteLine($"Return countDeleteAll : {countDeleteAll}");
        }

        private void TestDeleteOne()
        {
            User.Default user = new User.Default()
            {
                Id = "TestUserId1"
            };

            int countDeleteOne = userDao.Delete(user);
            Console.WriteLine($"Return countDeleteOne : {countDeleteOne}");
        }

        private void TestInsert()
        {
            User.Default user1 = new User.Default()
            {
                Id = "TestUserId1",
                UserName = "TestUserName1",
                UserAge = 30,
                CreateDt = DateTime.Now,
                UpdateDt = DateTime.Now
            };

            int countInsert1 = userDao.Insert(user1);
            Console.WriteLine($"Return countInsert1 : {countInsert1}");

            User.Default user2 = new User.Default()
            {
                Id = "TestUserId2",
                UserName = "TestUserName2",
                UserAge = 30,
                CreateDt = DateTime.Now,
                UpdateDt = DateTime.Now
            };

            int countInsert2 = userDao.InsertCustomMapping(user2);
            Console.WriteLine($"Return countInsert2 : {countInsert2}");

            User.Default user3 = new User.Default()
            {
                Id = "TestUserId3",
                UserName = "TestUserName3",
                UserAge = 24,
                CreateDt = DateTime.Now,
                UpdateDt = DateTime.Now
            };

            int countInsert3 = userDao.InsertCustomMapping(user3);
            Console.WriteLine($"Return countInsert3 : {countInsert3}");
        }

        private void TestUpdate()
        {
            User.Default user = new User.Default()
            {
                Id = "TestUserId1",
                UserName = "TestUserName1Update",
                UserAge = 40,
                UpdateDt = DateTime.Now
            };

            int countUpdate1 = userDao.Update(user);
            Console.WriteLine($"Return countUpdate1 : {countUpdate1}");
        }

        private void TestSelect()
        {
            User.Default user = new User.Default()
            {
                Id = "TestUserId1"
            };

            User.Default selectedUser = userDao.Select(user);
            Console.WriteLine($"selectedUser, id={selectedUser.Id}, name={selectedUser.UserName}, age={selectedUser.UserAge}");

            List<User.Default> selectedUserList = userDao.Select();
            Console.WriteLine($"selectedUserList, count={selectedUserList.Count}");
        }
    }
}
