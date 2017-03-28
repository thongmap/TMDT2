using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using TMDT.DAO;
using TMDT.Logic;

namespace TMDT.Controllers
{
    public class CartItemController : Controller
    {
        private TMDTModel storeDB = new TMDTModel();

        public ActionResult GetRating(string id)
        {
            TMDTModel db = new TMDTModel();
            int ids = int.Parse(id.Substring(0, id.IndexOf(",")));
            return View(db.Accounts.Find(ids));
        }

        [HttpGet]
        public ActionResult Index()
        {
            var cart = new ShoppingCart();
            if (Session["User"] == null)
                cart = ShoppingCart.GetCart(this.HttpContext, null);
            else
            {
                Account user = Session["User"] as Account;
                cart = ShoppingCart.GetCart(this.HttpContext, user.UserName);
            }
            var viewModel = new ShoppingCartViewModel
            {
                CartItems = cart.GetCartItems(),
                CartTotal = cart.GetTotal()
            };
            foreach (var item in viewModel.CartItems)
            {
                int i = item.Product.Image.IndexOf("|");
                item.Product.Image = item.Product.Image.Substring(0, i);
            }
            return View(viewModel);
        }

        public ActionResult AddToCart(int id)
        {
            var addedAlbum = storeDB.Products
                .Single(album => album.ProductID == id);
            if (addedAlbum.Account.Status.Equals("false"))
                {
                TempData["notice"] = "Người bán đã bị khóa, không thể thêm sản phẩm";

                return RedirectToAction("Index");
            }
            if (addedAlbum.Quantity > 0)
            {
                var cart = new ShoppingCart();
                if (Session["User"] == null)
                    cart = ShoppingCart.GetCart(this.HttpContext, null);
                else
                {
                    Account user = Session["User"] as Account;
                    if (user.AccountID == addedAlbum.AccountID)
                    {
                        TempData["notice"] = "Không thể thêm sản phẩm";

                        return RedirectToAction("Index");
                    }
                    cart = ShoppingCart.GetCart(this.HttpContext, user.UserName);
                }
                TempData["Notice"]=cart.AddToCart(addedAlbum);
                Session["CartCount"] = cart.GetCount();
                return RedirectToAction("Index");
            }
            else
            {
                TempData["Notice"] = "Số lượng sản phẩm đã hết ";
                return RedirectToAction("Index");
            }

        }

        [HttpPost]
        public ActionResult RemoveFromCart(int id)
        {
            var cart = new ShoppingCart() ;
            if (Session["User"] == null)
                cart = ShoppingCart.GetCart(this.HttpContext,null);
            else
            {
                Account user = Session["User"] as Account;
                cart = ShoppingCart.GetCart(this.HttpContext, user.UserName);
            }  
            cart.RemoveFromCart(id);
            Session["CartCount"] = cart.GetCount();
            return RedirectToAction("Index");
        }
        public ActionResult Update(FormCollection fc)
        {
            string[] quantities = fc.GetValues("quantity");
            List<cartitem> cart;
            if (Session["User"] == null)
                cart = ShoppingCart.GetCart(this.HttpContext, null).GetCartItems();
            else
            {
                Account user = Session["User"] as Account;
                cart = ShoppingCart.GetCart(this.HttpContext, user.UserName).GetCartItems();
            }
            for (int i = 0; i < cart.Count(); i++)
            {
                cart[i].Quantity = Convert.ToInt32(quantities[i]);
            }
            ViewBag.Quantity = cart;
            var cart1 = new ShoppingCart();
            if (Session["User"] == null)
                cart1 = ShoppingCart.GetCart(this.HttpContext, null);
            else
            {
                Account user = Session["User"] as Account;
                cart1 = ShoppingCart.GetCart(this.HttpContext, user.UserName);
            }
            TempData["Notice"]= cart1.UpdateCart(cart);
            Session["CartCount"] = cart1.GetCount();
            return RedirectToAction("Index");
        }

        [ChildActionOnly]
        public ActionResult CartSummary()
        {
            var cart = new ShoppingCart();
            if (Session["User"] == null)
                cart = ShoppingCart.GetCart(this.HttpContext, null);
            else
            {
                Account user = Session["User"] as Account;
                cart = ShoppingCart.GetCart(this.HttpContext, user.UserName);
            }
            ViewData["CartCount"] = cart.GetCount();
            return PartialView("CartSummary");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                storeDB.Dispose();
            }
            base.Dispose(disposing);
        }

        [HttpGet]
        public ActionResult Payment()
        {
            var cart = ShoppingCart.GetCart(this.HttpContext, "");
            Account user = new Account();
            if (Session["User"] != null)
            {
                user = Session["User"] as Account;
                cart = ShoppingCart.GetCart(this.HttpContext, user.UserName);
            }
            var viewModel = new ShoppingCartViewModel
            {
                CartItems = cart.GetCartItems(),
                CartTotal = cart.GetTotal()
            };
            if(viewModel.CartItems.Count==0)
                return RedirectToAction("Index", "Home");
            ViewBag.Item = viewModel.CartItems;
            string s = "";
            foreach (cartitem item in viewModel.CartItems)
                s += item.Product.ProductName + "|";
            Bill bill = new Bill();
            if (user != null)
            {
                bill = new Bill()
                {
                    AccountID = user.AccountID,
                    ShipName = user.UserName,
                    ShipMobile = user.Phone,
                    ShipAddress = user.Address,
                    ShipEmail = user.Email,
                    SumMoney = viewModel.CartTotal
                };
            }
            else
            {
                Random rnd = new Random();
                bill = new Bill()
                {
                    AccountID = rnd.Next(),
                    ShipName = "",
                    ShipMobile = "",
                    ShipAddress = "",
                    ShipEmail = "",
                    SumMoney = viewModel.CartTotal
                };
            }
            ViewBag.Name = s;
            return View(bill);
        }

        [HttpPost]
        public ActionResult Payment(Bill bill)
        {
            if (ModelState.IsValid)
            {
                bill.CreatedDate = DateTime.Now;
                bill.Status = "Chưa hoàn tất";
                var user = Session["User"] as TMDT.Account;
                var cart = ShoppingCart.GetCart(this.HttpContext,"");
                if (user != null)
                {
                    cart = ShoppingCart.GetCart(this.HttpContext, user.UserName);
                }
                foreach (var item in cart.GetCartItems())
                {
                    bill.DetailBills.Add(new TMDT.DetailBill()
                    {
                        AccountID = item.Product.AccountID,
                        ProductID = item.ProductID,
                        Quantity = item.Quantity,
                        Price = item.Product.Price
                    });
                }
                cart.EmptyCart();
                Session.Remove("CartCount");

                new BillDAO().Insert(bill);

                return RedirectToAction("Order");
            }
            return View(bill);
        }
        [HttpGet]
        public ActionResult OnlinePay(string buyerinfo)
        {
            Bill bill = new Bill();
            string[] info = buyerinfo.Split(new Char[] { '|' });
            bill.ShipName = info[0];
            bill.ShipMobile = info[1];
            bill.ShipAddress = info[2];
            bill.ShipEmail = info[3];
            bill.SumMoney = Decimal.Parse(info[4]);
            bill.CreatedDate = DateTime.Now;
            bill.Status = "Chưa hoàn tất";
            var user = Session["User"] as TMDT.Account;
            var cart = ShoppingCart.GetCart(this.HttpContext, "");
            if (user != null)
            {
                cart = ShoppingCart.GetCart(this.HttpContext, user.UserName);
                bill.AccountID = user.AccountID;
            }
            foreach (var item in cart.GetCartItems())
            {
                bill.DetailBills.Add(new TMDT.DetailBill()
                {
                    AccountID = item.Product.AccountID,
                    ProductID = item.ProductID,
                    Quantity = item.Quantity,
                    Price = item.Product.Price
                });
            }

            cart.EmptyCart();
            Session.Remove("CartCount");

            new BillDAO().Insert(bill);
            ViewBag.Detail = new BillDAO().GetDetailBill_BillID(bill.BillID);
            return View(bill);
        }
        public ActionResult OnlinePayment(string info,string name)
        {
           
            Response.Redirect("http://sandbox.nganluong.vn:8088/nl30/button_payment.php?receiver=thongmap1995@yahoo.com&product_name="+name+"&price=3000&return_url=http://localhost:3133/CartItem/OnlinePay?buyerinfo=" + info + "&comments=thongiòiẻuheui");
            
            return View();
        }
        [HttpGet]
        public ActionResult Order(int? index, int?  limitTime, int? status,string email)
        {
            if (String.IsNullOrEmpty(email))
            {
                var user = Session["User"] as TMDT.Account;
                if (user == null)
                    return RedirectToAction("Login", "Home");
                var model = new BillDAO().ViewOrder(index, limitTime, status, user.AccountID);
                return View(model);
            }
            else
            {
                var model = new BillDAO().ViewOrder(index, limitTime, status, email);
                return View(model);
            }
        }  

        [HttpGet]
        public ActionResult OrderDetail(int id)
        {
            var user = Session["User"] as TMDT.Account;
            if (user == null)
                return RedirectToAction("Login", "Home");
            var model = new BillDAO().GetDetailBill_BillID(id);           
            return View(model);
        }

        public JsonResult ChangeSttBill(string id)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            int billid = Int16.Parse(serializer.Deserialize<string>(id));
            try
            {
                new BillDAO().ChangeStatus(billid);
                return Json(new
                {
                    status = true
                });
            }
            catch
            {
                return Json(new
                {
                    status = false
                });
            }
        }

        public ActionResult Order(List<Bill> bill)
        {
            return View();
        }

        public ActionResult Sort(int? sort, int? date, int? sum)
        {
            var user = Session["User"] as TMDT.Account;
            if (user == null)
                return RedirectToAction("Login", "Home");
            List<Bill> listbill = new BillDAO().Sort(sort, date, sum, user.AccountID);
            return View("Order", listbill);
        }
    }
}