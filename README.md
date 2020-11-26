Arkadaşlar merhaba,

Bu yazımda sizlere CodeFirst mantığıyla bir kullanıcı tablosu yaratıp basit form yönetimi ile kullanıcı giriş sistemi anlatıp yazdığım kodu adım adım paylaşmaya çalışacağım.

Bir web sitesi tasarlarken muhakkak bir kullanıcı giriş sistemine ihtiyaç duyuluyor. Bu sistemi basit ama kullanışlı bir yöntemle sizlere aktarmaya çalışacağım.

İlk olarak Visual Studio üzerinden boş bir Asp.Net MVC projesi başlatıyorum. Boş projemiz açıldıktan sonra Model klasörüne hemen 'Kullanici' simli bir class oluşturup propertylerini aşağıdaki gibi tanımlıyorum.


    public class Kullanici
    {
	public int ID { get; set; }
        public string AdiSoyadi { get; set; }
        public string Sifre { get; set; }
        public string Email { get; set; }
    }
Kullanici isimli class'ımızı oluşturduktan sonra Model klasörünün içerisine tekrar yeni bir class ekleyip ismini ProjeContext olarak belirliyoruz. Bu class'ın içerisinde CodeFirst ile veri tabanı bağlantımız için bilgileri ve oluşturduğumuz Kullanici isimli class'ımızın tablo olarak veri tabanına kayıt edilmesi için dbSet propertysini yazacağız.

ProjeContext isimli class'ımızın içeriğinide aşağıdaki gibi kodluyoruz.


    public class ProjeContext : DbContext
    {
        public ProjeContext()
        {
            Database.Connection.ConnectionString = "Server=DESKTOP-KSAA1RL\\MERTKURT;Database=s;UID=sa;PWD=12345678;";

        }

        public DbSet Kullanici { get; set; }
    }
Yukarıda paylaştığım kodu sayfanıza eklediğinizde hata alırsanız öncelikle projenizde Entity Framework dll lerinin ekli olduğundan emin olun. Eğer ekli değilse Manage Nuget Packages yardımıyla projenize EntityFramework dll lerini ekleyebilirsiniz.

Entity Framework dll lerini tamamladıktan sonra ProjeContext isimli class'ımıza kalıtım alarak veritabanı işlemlerini gerçekleştirmesi için verdiğimiz Entity Framework içerisinden gelen 'DbContext' class'ımızı eklemeyi unutmuyoruz.

DbContext class'ınıda ekledikten sonra hazırladığımız ProjeContext isimli class'ın yapıcı metodunu(Constructor) yazarak veri tabanı bağlantımız için ConnectionString'imizi tanımlıyoruz.

Burda sizlerde kendi bilgisayarınızda kullandığınız MsSQL server bilgilerini yazarak projemizin start edildiğinde direk çalışmasını sağlayabilirsiniz.

ProjeContext isimli class'ımızda son olarak makalemizin başında oluşturduğumuz Kullanici class ının veritabanında tablo olarak kullanılması için DbSet propertysini yazıp class daki kod geliştirmemizi tamamlıyoruz.

Model klasörüne son olarak kullanıcı giriş işlemi yaparken oluşturacağımız view da kullanmak için KullaniciVM (Kullanıcı View Model) isimli bir class daha tanımlıyoruz. İçerisinide aşağıdaki gibi kodluyoruz.


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
Yukarıdaki View Modeli adından da anlaşılabileceği gibi View için model olması ve Viewdan giriş yaparken hata mesajlarını düzgün bir şekilde ekrana getirmek için kodluyoruz. Model klasöründeki bütün geliştirmelerimiz bitti artık Controller klasörüne Controller'larımızı ekleyerek devam edeceğiz.

İlk ekleyeceğimiz Controller 'KullaniciGiris' isimli olacak ve kullanıcı giriş işlemlerini yönettiğimiz bölüm olacaktır. Kodlamasınıda aşağıdaki gibi yapıyoruz.


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
Kullanıcı giriş işlemlerimizi yapmak için Controller'ımız içerisine Giris ve Cikis isimli ActionResultlarımızı yazıyoruz. Giriş işleminde kullanmak için ActionResultımızı HttpGet ve HttpPost olarak 2 farklı şekilde yazıyoruz.

ActionResult lar içinde kullandığımız FormsAuthentication yapısı için kütüphanelerimiz arasına 'System.Web.Security' kütüphanesini ekliyoruz. Bu kütüphane sayesinde cookie ile sisteme giriş yapan kullanıcının bilgilerini tutuyor olacağız.

Ayrıca Model klasörüne oluşturduğumuz ProjeContext class ı sayesinde veri tabanında kayıtlı olan kullanıcılarımızın eşleşmelerini kontrol ediyor olacağız.

Model klasörüne hazırladığımız bir diğer class'ımız olan KullaniciVM ile Giris ActionResult'ımızın HttpPost kısmında 'ValidateAntiForgeryToken' attributesini kullanarak oluşturduğumuz modeldeki hata mesajlarını View'ımızda gösteriyor olacağız.

Yukarıda oluşturduğumuz Controller da Giris ActionResult'ında yönlendirme olarak kullandığımız Home Controller'ı şimdi tanımlıyoruz. HomeController içerisinide aşağıdaki gibi kodluyoruz.


    [Authorize]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {

            ViewBag.CurrentUser = User.Identity.Name;

            return View();
        }
    }
HomeController içinde Index isimli olarak kodladığımız ActionResult içerisinde sisteme giriş yapan kullanıcının bilgisini alıp ViewBag yardımıyla View'a yönlendirip Viewda gösterimini sağlıyoruz.

Birde bu Controller'ı tanımlarken unutmadan attribute olarak [Authorize]'ı ekliyoruzki otomatik olarak sisteme kullanıcının giriş yapıp yapmadığını kontrol etmemizi sağlıyoruz.

 

Controller kodlamalarımızda bittiğine göre sırada Viewlarımızı kodlamak kaldı. İlk olarak KullaniciGirisController'ımızın içerisindeki Giris View ımızı yaratıp aşağıdaki gibi kodluyoruz.


@{
    Layout = null;
}

@model KullaniciVM

<!DOCTYPE html>

<html>
<head>
    <meta name="viewport" content="width=device-width" />
    <title>Login</title>
    <link href="~/Content/bootstrap.min.css" rel="stylesheet" />
    <script src="~/Scripts/bootstrap.min.js"></script>
    <script src="~/Scripts/jquery-1.10.2.min.js"></script>
</head>
<body>
    <div class="container">
        <div class="col-md-4  col-md-offset-4">

            <div class="panel panel-primary">
                <div class="panel-heading">
                    Login Paneli
                </div>
                <div class="panel-body">
                    @using (Html.BeginForm())
                    {
                        @Html.AntiForgeryToken()

                        <div class="input-group">
                            @Html.TextBoxFor(x => x.Email, new { @placeholder = "E-Posta adresiniz", @class = "form-control" })
                            <div class="input-group-addon">
                                <i class="glyphicon glyphicon-user"></i>
                            </div>
                        </div>
                        <span class="text-danger">
                            @Html.ValidationMessageFor(x => x.Email)
                        </span>

                        <br />

                        <div class="input-group">
                            @Html.TextBoxFor(x => x.Sifre, new { @placeholder = "Parola giriniz", @class = "form-control" })
                            @Html.ValidationMessageFor(x => x.Sifre)
                            <div class="input-group-addon">
                                <i class="glyphicon glyphicon-lock"></i>
                            </div>
                        </div>
                        <span class="text-danger">
                            @Html.ValidationMessageFor(x => x.Email)
                            @ViewBag.HataMesaji
                        </span>
                        <br />
                        <input class="btn btn-success pull-right" type="submit" name="name" value="Giriş" />
                    }
                </div>
            </div>
        </div>
    </div>
</body>
</html>
 
View'ı kodlarken model olarak KullaniciVM'i kulllandığımız hata mesajlarını ekranda göstermemizi sağlamak için 'ValidationMessageFor' helperlarını kullanmayı unutmuyoruz.

KullaniciGirisController içerisindeki Cikis ActionResult'u için bir View tasarlamıyoruz. Zaten kullanıcı çıkış işlemi yaptığımızda Yazdığımız kod otomatik olarak bizi giriş sayfasına yönlendireceği için bir view ihtiyacımız olmuyor.

Bir diğer Controller'ımız olan HomeController içerisindeki Index ActionResult'u için ise aşağıdaki gibi bir kodlama yapıyoruz.


@{
    Layout = null;
}

<!DOCTYPE html>

<html>
<head>
    <meta name="viewport" content="width=device-width" />
    <title>Index</title>
    <link href="~/Content/bootstrap.min.css" rel="stylesheet" />
    <script src="~/Scripts/bootstrap.min.js"></script>
    <script src="~/Scripts/jquery-1.10.2.min.js"></script>
</head>
<body>

    Hoşgeldiniz  @ViewBag.GirisYapanKullanici

    <a class="btn btn-primary" href="/KullaniciGiris/Cikis">Çıkış Yap!</a>

</body>
</html>


HomeController'ı kodlarken Index ActionResult'ı içerisinde tanımladığımız viewbag i ekranda göstermek için body html etiketlerimiz arasına yazıyoruz. Böylelikle sisteme giriş yapan kullanıcı bilgisini View ile ekranımızda göstermiş olacağız. Birde sistemden çıkış yapmak için bir link bağlantısı koyup KullaniciGirisController'ımızdaki Cikis ActionResult'umuzu çağırıyoruz.

Kodlama işlemimizin büyük bir kısmını tamamladık aslında. Sadece sisteme giriş yaparken olduki HomeController içerisindeki Index View'ını başlatarak ekranı açtığımızda sisteme giriş yapan bir kullanıcı yoksa sayfanın otomatik olarak sisteme giriş yapmamızı sağlayacak olan KullaniciGirisController içerisindeki Giris isimli ActionResult'a yönlenmemizi sağlayacak olan kod eklentimizi yapmaya. Bu eklentiyi projemizin Web.config dosyasına aşağıdaki kodu ekleyerek yapıyor olacağız.

<authentication mode="Forms">
      <forms loginUrl="~/KullaniciGiris/Giris" slidingExpiration="true" timeout="2880"> 
        
      </forms>
    </authentication>
Yukarıdaki Web.config kodumuzda kullandığımız değerler bize sistemde kullanıcı ne kadar süre hareket etmeden kalırsa sistemdne çıksın ve sisteme giriş yapılmadığında hangi adrese yönlendirme olsun bilgilerini gösteriyor.

Projemizdeki kod geliştirmemiz artık bitti. Unutmadan ProjeContext class'ımızdaki connectionstring i düzenlerseniz sistemi çalıştırdığınızda bir sorun olmadan sizlerdede çalışması gerekmektedir. Olduki çalışmadı örneğe ulaşmak için aşağıdaki link bağlantılarını kullanabilirsiniz.

Birde unutmadan projeyi ilkkez çalıştırdığınızda sisteme giriş yapmayı birkez deneyin ve hemen sonrasında veritabanını açıp oluşan tabloya bir elle bir kullanıcı ekleyip sonra o eklediğiniz kullanıcı ile sisteme girmeyi deneyin. Bu işlemi yapmazsanız sisteme girecek bir kullanıcı bulamadığınız için kullanıcı giriş işlemini hiçbir şekilde çalıştıramazsınız.
