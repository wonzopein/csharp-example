using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace csharp_net_dapper.Domain
{
    public class User
    {

        public class Default
        {
            string id;
            string userName;
            int userAge = 0;
            DateTime createDt;
            DateTime updateDt;

            public string Id { get => id; set => id = value; }
            public string UserName { get => userName; set => userName = value; }
            public int UserAge { get => userAge; set => userAge = value; }
            public DateTime CreateDt { get => createDt; set => createDt = value; }
            public DateTime UpdateDt { get => updateDt; set => updateDt = value; }
        }
        
    }
}
