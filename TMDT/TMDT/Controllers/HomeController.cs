using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TMDT.DAO;
using TMDT.Others;
using TMDT.Models;
using System.Data.Entity;
using PagedList;
using System.Data.Entity.Validation;
using System.Diagnostics;
using TMDT.Logic;
using System.Net.Mail;
using System.Web.Mail;
using Facebook;
using System.Web.Security;

namespace TMDT.Controllers
{
    public class HomeController : Controller
    {
      
        public ActionResult Index()
        {
            ViewBag.New = new ProductDAO().ListNewProduct(12);
            ViewBag.Discount = new ProductDAO().ListDiscount();
            ViewBag.Category = new ProductDAO().ListAllCat();
            ViewBag.Hier = new HierDAO().ListAll();
            return View(new ProductDAO().ListAll());
        }
        public ActionResult GetData()
        {
            TMDTModel db = new TMDTModel();
            var ListCategory = new HierDAO().ListAll();
            ViewBag.Catlist = new CategoryDAO().ListAll();
            ViewBag.HierList = ListCategory;
            return PartialView();
        }
        public ActionResult Details(int id)
        {
            TMDTModel db = new TMDTModel();
            int? idCat = db.Products.Find(id).CategoryID;
            List<Product> Lis = db.Products.Where(s => s.CategoryID == idCat).ToList();
            ViewBag.Same = Lis;
            return View(db.Products.Find(id));
        }

        public ActionResult ReturnSearch(string name, string category, string merchant, string pricetext1, string pricetext2, int? page, string currentFilter, string currentcategory, string currentmerchant, string curpricetext1, string curpricetext2)
        {
            TMDTModel db = new TMDTModel();
            db.Products.Load();
            var product = db.Products.Include(s => s.Category);
            if (name != null)
            {
                page = 1;
            }
            else
            {
                name = currentFilter;
            }
            ViewBag.CurrentFilter = name;
            if (category != null)
            {
                page = 1;
            }
            else
            {
                category = currentcategory;
            }
            ViewBag.CurrentCategory = category;
            if (merchant != null)
            {
                page = 1;
            }
            else
            {
                merchant = currentmerchant;
            }
            ViewBag.CurrentMerchant = merchant;
            if (pricetext1 != null)
            {
                page = 1;
            }
            else
            {
                pricetext1 = curpricetext1;
            }
            ViewBag.CurrentP1 = pricetext1;
            if (pricetext2 != null)
            {
                page = 1;
            }
            else
            {
                pricetext2 = curpricetext2;
            }
            ViewBag.CurrentP2 = pricetext2;
            if (!string.IsNullOrEmpty(name))
                product = product.Where(i => i.ProductName.Contains(name));
            if (!string.IsNullOrEmpty(merchant))
                product = product.Where(i => i.Account.UserName.Contains(merchant));
            if (!String.IsNullOrEmpty(category))
            {
                int lop = Convert.ToInt32(category);
                product = product.Where(i => i.CategoryID == lop);
            }
            if (!String.IsNullOrEmpty(pricetext1))
            {
                int price1 = int.Parse(pricetext1);
                int price2 = int.Parse(pricetext2);
                product = product.Where(i => i.Price >= price1 && i.Price <= price2);
            }
            product = product.OrderBy(i => i.ProductName);
            if (product.Count() > 0)
            {
                foreach (Product item in product)
                {
                    int i = item.Image.IndexOf("|");
                    item.Image = item.Image.Substring(0, i);
                }
            }
            int pageSize = 8;
            int pageNumber = (page ?? 1);
            return View(product.ToPagedList(pageNumber, pageSize));
        }

        [HttpPost]
        public ActionResult Login(string username, string pass)
        {
            if (ModelState.IsValid)
            {
                var dao = new AccountDAO();
                int result = dao.Login(username, Encryptor.MD5Hash(pass));
                if (result == 1)
                {
                    var user = dao.GetUser(username);
                    if (user.Status == "true")
                    {
                        Session["User"] = user;
                        string notice = "";
                        DaysLeft daysleft = dao.CountDayLeft(user.AccountID);
                        Session["DaysLeft"] = daysleft;
                        var cart = ShoppingCart.GetCart(this.HttpContext, null);
                        notice = cart.RemoveSellerItem(user.AccountID);
                        cart.MigrateCart(username);
                        Session.Remove("CartID");
                        if (!String.IsNullOrEmpty(notice))
                            TempData["notice"] = "Bạn không thể mua các sản phẩm: " + notice + " vì bạn bán chúng";
                        Session["CartCount"] = ShoppingCart.GetCart(this.HttpContext, username).GetCount();
                        if (cart.GetCount() > 0)
                            return RedirectToAction("Index", "CartItem");
                        return RedirectToAction("Index");
                    }
                    else
                        ViewBag.Message = "Xin lỗi tài khoản của bạn đã bị khóa. Vì lý do: " + user.LockNote + ". Nếu có thắc mắc, vui lòng liên hệ 11-22-33-44.";
                }
                else
                    ViewBag.Message = "Tài khoản hoặc mật khẩu không đúng. Vui lòng đăng nhập lại!";
            }
            return View();
        }

        [HttpGet]
        public ActionResult Login()
        {
            if (Session["User"] != null)
                return RedirectToAction("Index");
            ViewBag.Message = TempData["Message"];
            return View();
        }

        public ActionResult Logout()
        {
            Session.Remove("User");
            Session.Remove("DaysLeft");
            Session.Remove("CartCount");
            Session.Remove("CartID");
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Register()
        {
            ViewBag.Message = TempData["Message"];
            return View();
        }

        [HttpPost]
        public ActionResult Register(Account model)
        {
            if (ModelState.IsValid)
            {
                var dao = new AccountDAO();
                if (dao.CheckUserName(model.UserName))
                {
                    ViewBag.Message = "Tên đăng nhập đã tồn tại";
                }
                else if (dao.CheckEmail(model.Email))
                {
                    ViewBag.Message = "Email đã tồn tại";
                }
                else
                {
                    var user = new Account();
                    user.UserName = model.UserName;
                    user.Pass = Encryptor.MD5Hash(model.Pass);
                    user.Phone = model.Phone;
                    user.Address = model.Address;
                    user.Email = model.Email;
                    user.Level = 2;
                    user.CreatedDate = DateTime.Now;
                    user.ExpiryDate = DateTime.Now;
                    user.Status = "false";
                    user.LockNote = "Ban chua kich hoat";
                    user.Rating = 0;
                    user.NoRating = 0;
                    var result = dao.Insert(user);
                    if (result > 0)
                    {
                        int userid = dao.GetIdUser(model.UserName);
                        string smtpUserName = "testtmdt123@gmail.com";
                        string smtpPassword = "conheo123";
                        string smtpHost = "smtp.gmail.com";
                        int smtpPort = 25;
                        string emailTo = model.Email;
                        string subject = "Xác minh tài khoản của bạn";
                        string path = Server.MapPath(@"/public/images/facebook.png");
                        //now do the HTML formatting
                        //@Request.Url.GetLeftPart(UriPartial.Authority)
                        string body = string.Format(
                            "<img src=@'/public/images/facebook.png'/><br/>Xin chào {0}.<br/>Cảm ơn bạn đã đăng kí tài khoản tại TATSHOP. Vui lòng nhấn vào đường dẫn bên dưới để hoàn tất đăng kí.<br/><a href=\"{1}\" title=\"Xác nhận tài khoản của bạn\">Xác nhận</a>", model.UserName, Url.Action("ConfirmEmail", "Home", new { Token = userid, Email = model.Email }, Request.Url.Scheme));


                        EmailService service = new EmailService();
                        bool kq = service.Send(smtpUserName, smtpPassword, smtpHost, smtpPort,
                            emailTo, subject, body, path);
                        model = new Account();
                        return RedirectToAction("Confirm", "Home", new { Email = user.Email });
                    }
                    else
                    {
                        ViewBag.Message = "Đăng kí chưa thành công. Vui lòng thử lại.";
                    }
                }
            }
            return View(model);
        }

        public ActionResult ConfirmEmail(string Token, string Email)
        {
            var dao = new AccountDAO();
            Account user = dao.FindById(Int16.Parse(Token));
            if (user != null)
            {
                if (user.Email == Email)
                {
                    dao.UpdateStatusUser(Int16.Parse(Token));
                    TempData["Message"] = "Tài khoản của bạn đã được kích hoạt. Vui lòng đăng nhập tại đây.";
                    return RedirectToAction("Login", "Home");
                }
                else
                {
                    return RedirectToAction("Confirm", "Home", new { Email = user.Email });
                }
            }
            else
            {
                TempData["Message"] = "Tài khoản này không có trong hệ thống. Vui lòng đăng kí tại đây.";
                return RedirectToAction("Register", "Home");
            }
        }
        public ActionResult Confirm(string Email)
        {
            ViewBag.Email = Email;
            return View();
        }

        [HttpGet]
        public ActionResult Upgrade()
        {
            return View();
        }

        public ActionResult Result(string tx, string st, string amt, string cc, string cm, string item_number)
        {
            if (Session["User"] != null)
            {
                var user = Session["User"] as TMDT.Account;
                var dao = new OrderDAO();
                Order order = new Order();
                order.AccountID = user.AccountID;
                order.CreatedDate = DateTime.Now;
                order.Quantity = 1;
                order.Total = 50;
                dao.Insert(order);
                new AccountDAO().UpgradeAccount(user.AccountID);
                user.Level = 1;
                Session.Remove("User");
                Session["User"] = user;
                return RedirectToAction("Sell", "Merchant");
            }
            else
                return RedirectToAction("Login", "Home");
        }

        [ChildActionOnly]
        public ActionResult Menu()
        {
            var model = new HierDAO().ListAll();
            ViewBag.Category = new CategoryDAO().ListAll();
            return PartialView(model);
        }

        public ActionResult Category(int id)
        {
            var model = new ProductDAO().GetProduct(id);
            foreach (var item in model)
            {
                int i = item.Image.IndexOf("|");
                item.Image = item.Image.Substring(0, i);
            }
            ViewBag.CatName = new HierDAO().HierName(id);
            return View(model);
        }

        public JsonResult SendRating(string r, string s, int id, string url)
        {
            TMDTModel db = new TMDTModel();
            int autoId = id;
            Int16 thisVote = 0;
            Int16 sectionId = 0;
            Int16.TryParse(s, out sectionId);
            Int16.TryParse(r, out thisVote);
            if (autoId.Equals(0))
            {
                return Json("Phần đánh giá không thể thực hiện.");
            }

            switch (s)
            {
                case "5":
                    Account user = Session["User"] as Account;
                    var isIt = db.VoteModels.Where(m => m.BuyerID == user.AccountID && m.MerchantID == autoId).FirstOrDefault();
                    if (isIt != null)
                    {
                        // keep the school voting flag to stop voting by this member
                        HttpCookie cookie = new HttpCookie(url, "true");
                        Response.Cookies.Add(cookie);
                        return Json("<br />Bạn đã đánh giá người bán này rồi.");
                    }
                    Account sch = db.Accounts.Where(sc => sc.AccountID == autoId).First();
                    if (!sch.Equals(null))
                    {
                        try
                        {
                            sch.Rating = (double)(thisVote + sch.Rating * sch.NoRating) / (double)(sch.NoRating + 1);
                            sch.NoRating += 1;
                            db.Entry(sch).State = EntityState.Modified;
                            db.SaveChanges();

                            VoteModel vm = new VoteModel()
                            {
                                BuyerID = user.AccountID,
                                MerchantID = autoId,
                                NoVotes = thisVote,
                            };

                            db.VoteModels.Add(vm);
                            db.SaveChanges();
                            HttpCookie cookie = new HttpCookie(url, "true");
                            Response.Cookies.Add(cookie);
                        }
                        catch (DbEntityValidationException e)
                        {
                            foreach (var eve in e.EntityValidationErrors)
                            {
                                Debug.WriteLine("Entity of type \"{0}\" in state \"{1}\" has the following validation errors:",
                                    eve.Entry.Entity.GetType().Name, eve.Entry.State);
                                foreach (var ve in eve.ValidationErrors)
                                {
                                    Debug.WriteLine("- Property: \"{0}\", Error: \"{1}\"",
                                        ve.PropertyName, ve.ErrorMessage);
                                }
                            }
                            throw;
                        }
                    }
                    break;
                default:
                    break;
            }
            return Json("<br />Bạn đã đánh giá " + r + " sao. Cảm ơn bạn.");
        }

        [HttpGet]
        public ActionResult AccountInfo(int id)
        {
            var user = Session["User"] as TMDT.Account;
            if (user == null)
            {
                return RedirectToAction("Login", "Home");
            }
            var model = new AccountDAO().GetUser_ID(id);
            return View(model);
        }

        [HttpGet]
        public ActionResult ChangeInfo(int? id)
        {
            var user = Session["User"] as TMDT.Account;
            if (user == null)
            {
                return RedirectToAction("Login", "Home");
            }
            var model = new AccountDAO().GetUser_ID(id);
            return View(model);
        }

        [HttpPost]
        public ActionResult ChangeInfo(Account acc)
        {
            if (ModelState.IsValid)
            {
                new AccountDAO().UpdateAcc(acc);
                TempData["Message"] = "Cập nhật thông tin tài khoản thành công.";
                return RedirectToAction("AccountInfo", "Home", new { id = acc.AccountID });
            }
            else
            {
                TempData["Message"] = "Cập nhật thất bại. Vui lòng nhập lại.";
                return RedirectToAction("ChangeInfo", "Home", new { id = acc.AccountID });
            }

        }

        [HttpGet]
        public ActionResult ChangePass(int? id)
        {
            var user = Session["User"] as TMDT.Account;
            if (user == null)
            {
                return RedirectToAction("Login", "Home");
            }
            var model = new AccountDAO().GetUser_ID(id);
            return View(model);
        }

        [HttpPost]
        public ActionResult ChangePass(string userid, string pass, string pass3)
        {
            var dao = new AccountDAO();
            if (dao.CheckPass(Int16.Parse(userid), Encryptor.MD5Hash(pass3)))
            {
                dao.ChangePass(Encryptor.MD5Hash(pass), Int16.Parse(userid));
                TempData["Message"] = "Cập nhật mật khẩu thành công.";
                return RedirectToAction("AccountInfo", new { id = Int16.Parse(userid) });
            }
            else
            {
                TempData["Message"] = "Mật khẩu cũ không đúng. Vui lòng nhập lại";
                return RedirectToAction("ChangePass", new { id = Int16.Parse(userid) });
            }

        }
    }
}
