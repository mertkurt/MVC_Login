using MVC_Login.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace MVC_Login.Controllers
{
    public class KullaniciGirisController : Controller
    {
        [HttpGet]
        public ActionResult Giris()
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Home");
            }

            return View();
        }
        
        [HttpPost, ValidateAntiForgeryToken]
        public ActionResult Giris(KullaniciVM model)
        {
            if (ModelState.IsValid)
            {
                using (ProjeContext context = new ProjeContext())
                {
                    var user = context.Kullanici.FirstOrDefault(x => x.Email == model.Email && x.Sifre == model.Sifre);

                    if (user != null)
                    {
                        FormsAuthentication.SetAuthCookie(user.AdiSoyadi, true);

                        return RedirectToAction("Index", "Home");
                    }
                }
            }

            ViewBag.HataMesaji = "Email veya Parola yanlış. Lütfen Doğru bilgilerle tekrar giriş işlemi deneyiniz.";

            return View();
        }

        public ActionResult Cikis()
        {
            FormsAuthentication.SignOut();

            return RedirectToAction("Giris", "KullaniciGiris");
        }
    }
}