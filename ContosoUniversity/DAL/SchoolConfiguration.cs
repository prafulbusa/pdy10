using System;
using System.Data.Entity;
using System.Data.Entity.SqlServer;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ContosoUniversity.DAL
{
    // EF automatically will run this code because it derives from 
    // DbConfiguration
    public class SchoolConfiguration : DbConfiguration
    {
        public SchoolConfiguration()
        {
            SetExecutionStrategy("System.Data.SqlClient", 
                () => new SqlAzureExecutionStrategy());

        }
    }
}