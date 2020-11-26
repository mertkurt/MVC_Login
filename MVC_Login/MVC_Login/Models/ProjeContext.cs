using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace MVC_Login.Models
{
    public class ProjeContext : DbContext
    {
        public ProjeContext()
        {
            Database.Connection.ConnectionString = "Server=DESKTOP-KSAA1RL\\MERTKURT;Database=OrnekLoginDB;UID=sa;PWD=12345678;";

        }

        public DbSet<Kullanici> Kullanici { get; set; }
    }
}