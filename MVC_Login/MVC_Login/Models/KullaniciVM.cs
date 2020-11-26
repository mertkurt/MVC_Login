using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace MVC_Login.Models
{
    public class KullaniciVM
    {
        [
           EmailAddress(ErrorMessage = "E-Posta formatında giriş yapınız"),
           Required(ErrorMessage = "E-Posta Giriniz"),
           DisplayName("E-posta")
       ]
        public string Email { get; set; }

        [
            Required(ErrorMessage = "Parola Giriniz"),
            DisplayName("Parola")
        ]
        public string Sifre { get; set; }

        [DisplayName("Beni Hatırla")]
        public bool BeniHatirla { get; set; }
    }
}