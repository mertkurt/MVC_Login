using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVC_Login.Models
{
    public class Kullanici
    {
        public int ID { get; set; }
        public string AdiSoyadi { get; set; }
        public string Sifre { get; set; }
        public string Email { get; set; }
    }
}