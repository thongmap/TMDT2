using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TMDT.DAO;
using TMDT.Models;
using TMDT.Others;
namespace TMDT.Controllers
{
    public class AdminController : Controller
    {
        public ActionResult Index()
        {
            var admin = Session["Admin"] as TMDT.Account;
            if (admin == null)
                return RedirectToAction("Login", "Admin");
            return View();
        }
        public ActionResult ListDonHang(int? year,int? month,int? currentyear,int? currentmonth,int page = 1, int pageSize = 10)
        {
            var admin = Session["Admin"] as TMDT.Account;
            if (admin == null)
                return RedirectToAction("Login", "Admin");
            decimal? i = 0;
            if (year !=  null)
            {
                page = 1;
            }
            else
            {
                year = currentyear;
            }
            ViewBag.CurrentFilterY = year;

            if ( month != null)
            {
                page = 1;
            }
            else
            {
                month = currentmonth;
            }
            ViewBag.CurrentFilterM = month;
            var model = new AdminDAO().ListAllDonHang(year,month,page, pageSize);
            var model12 = new AdminDAO().ListAllDonHang(year,month);

            foreach (var item in model12)
            {
                i+= item.Total;
            }
            ViewBag.Total = i;
            return View(model);
        }
        public ActionResult GetRating(string m)
        {
            return PartialView("~/Views/CartItem/_VoteNow.cshtml", m);
        }
        public ActionResult ListRating(int? search, int page = 1, int pageSize = 10)
        {
            var admin = Session["Admin"] as TMDT.Account;
            if (admin == null)
                return RedirectToAction("Login", "Admin");
            var model = new AdminDAO().ListRating(page, pageSize);
            if(search!=null)
            {
                if(search!=-1)
                    model = new AdminDAO().FilterRating(search, page, pageSize);
            }
            ViewBag.HighRating = new AdminDAO().ListOptionalRating(1, page, pageSize);
            ViewBag.LowRating = new AdminDAO().ListOptionalRating(2, page, pageSize);

            return View(model);
        }
        public ActionResult ListHoaDon(int page = 1, int pageSize = 10)
        {
            var admin = Session["Admin"] as TMDT.Account;
            if (admin == null)
                return RedirectToAction("Login", "Admin");

            var model = new AdminDAO().ListAllHoaDon(page, pageSize);

            return View(model);
        }
        public ActionResult DetailsHoaDon(int id, int page = 1, int pageSize = 10)
        {
            var admin = Session["Admin"] as TMDT.Account;
            if (admin == null)
                return RedirectToAction("Login", "Admin");
            var model = new AdminDAO().ListChiTietHoaDon(id, page, pageSize);

            return View(model);
        }
        [HttpPost]
        public ActionResult Login(string username, string pass)
        {
            if (ModelState.IsValid)
            {
                var dao = new AccountDAO();
                int result = dao.Login(username, pass);
                if (result == 1)
                {
                    var admin = dao.GetUser(username);
                    if (admin.Level == 0)
                    {
                        Session["Admin"] = admin;
                        return RedirectToAction("Users");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Bạn không có quyền truy cập!");
                    }
                }
                else
                    ModelState.AddModelError("", "Tài khoản hoặc mật khẩu không đúng. Vui lòng đăng nhập lại!");
            }
            return View();
        }

        [HttpGet]
        public ActionResult Login()
        {
            var admin = Session["Admin"] as TMDT.Account;
            if (admin != null)
                return RedirectToAction("Index", "Admin");
            return View();
        }

        public ActionResult Logout()
        {
            Session.Remove("Admin");
            return RedirectToAction("Login");
        }

        public ActionResult Users(string name,string currentFilter,int page = 1, int pageSize = 10)
        {
            var admin = Session["Admin"] as TMDT.Account;
            if (admin == null)
                return RedirectToAction("Login", "Admin");
            var model = new AdminDAO().ListAllUser(page, pageSize);
            if (name != null)
            {
                page = 1;
            }
            else
            {
                name = currentFilter;
            }
            ViewBag.CurrentFilter = name;

            if (!String.IsNullOrEmpty(name))
                model = new AdminDAO().ListNUser(name, page, pageSize);
            return View(model);
        }

        public ActionResult LockUser(int AccountID, string LockNote)
        {
            var admin = Session["Admin"] as TMDT.Account;
            if (admin == null)
                return RedirectToAction("Login", "Admin");
            if (String.IsNullOrEmpty(LockNote))
            {
                TempData["Notice"] = "Không được bỏ trống lí do khóa";
                return RedirectToAction("LockReason", new { id =AccountID });
            }
            new AdminDAO().LockUser(AccountID, LockNote);
            return View("AddUserNotification");
        }

        public ActionResult ResetPass(int id)
        {
            var admin = Session["Admin"] as TMDT.Account;
            if (admin == null)
                return RedirectToAction("Login", "Admin");
            new AdminDAO().ResetPass(id);
            string smtpUserName = "testtmdt123@gmail.com";
            string smtpPassword = "conheo123";
            string smtpHost = "smtp.gmail.com";
            int smtpPort = 25;
            string emailTo = new AdminDAO().getemail(id);
            string subject = "Mật khẩu của bạn đã được khôi phục";
            string body = string.Format(
                "<img src='images / feature - pic1.jpg' alt='Banner' /><br/>Xin chào {0}.<br/>Bạn hoặc ai đó đã khôi phục lại mật khẩu của bạn. Mật khẩu hiện tại là 123456.", new AdminDAO().getname(id), Url.Action("ConfirmEmail", "Home", new
                {
                    Token = id,
                    Email = new AdminDAO().getemail(id)
                }, Request.Url.Scheme));

            EmailService service = new EmailService();

            bool kq = service.Send(smtpUserName, smtpPassword, smtpHost, smtpPort,
                emailTo, subject, body);
            return View("AddUserNotification");
        }

        [HttpGet]
        public ActionResult AddUser()
        {
            var admin = Session["Admin"] as TMDT.Account;
            if (admin == null)
                return RedirectToAction("Login", "Admin");
            return View();
        }
       
        public ActionResult AddUser(Account model)
        {
            var admin = Session["Admin"] as TMDT.Account;
            if (admin == null)
                return RedirectToAction("Login", "Admin");
                var dao = new AccountDAO();
                if (dao.CheckUserName(model.UserName))
                {
                    ViewBag.Message = "Tên đăng nhập đã tồn tại";
                }
                else
                {
                    var user = new Account();
                    user.UserName = model.UserName;
                    user.Pass = Encryptor.MD5Hash(model.Pass);
                    user.Phone = model.Phone;
                    user.Address = model.Address;
                    user.Email = model.Email;
                    user.Level = model.Level;
                    user.CreatedDate = DateTime.Now;
                    user.ExpiryDate = DateTime.Now;
                    user.Status = "true";
                user.NoRating = 0;
                user.Rating = 0;
                    var result = dao.Insert(user);
            }
            return View("AddUserNotification");
        }

        public ActionResult MerchantUpgrade()
        {
            return View();
        }
        public ActionResult LockReason(int id)
        {
            var admin = Session["Admin"] as TMDT.Account;
            if (admin == null)
                return RedirectToAction("Login", "Admin");
            return View(new AdminDAO().ListUser(id));
        }
        public ActionResult CanhBao(int id)
        {
            var admin = Session["Admin"] as TMDT.Account;
            if (admin == null)
                return RedirectToAction("Login", "Admin");
            string smtpUserName = "testtmdt123@gmail.com";
            string smtpPassword = "conheo123";
            string smtpHost = "smtp.gmail.com";
            int smtpPort = 25;
            string emailTo = new AdminDAO().getemail(id);
            string subject = "Bạn có điểm đánh giá quá thấp";
            string body = string.Format(
                "<br/>Xin chào {0}.<br/>Điểm đánh giá của bạn hiện tại là " + new AdminDAO().getrating(id) + ", số điểm này là quá thấp, điều này có thể dẫn đến đơn hàng của bạn.", new AdminDAO().getname(id), Url.Action("ConfirmEmail", "Home", new
                {
                    Token = id,
                    Email = new AdminDAO().getemail(id)
                }, Request.Url.Scheme));

            EmailService service = new EmailService();

            bool kq = service.Send(smtpUserName, smtpPassword, smtpHost, smtpPort,
                emailTo, subject, body);
            return View("AddUserNotification");
        }
    }
}
