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
            if(Session["User"]==null)
            {
                TempData["Message"] = "Vui lòng đăng nhập để thực hiện thanh toán";
                return RedirectToAction("Login", "Home");
            }
            Account user = Session["User"] as Account;    
            var cart = ShoppingCart.GetCart(this.HttpContext,user.UserName);

            var viewModel = new ShoppingCartViewModel
            {
                CartItems = cart.GetCartItems(),
                CartTotal = cart.GetTotal()
            };
            if(viewModel.CartItems.Count==0)
                return RedirectToAction("Index", "Home");
            ViewBag.Item = viewModel.CartItems;
            Bill bill = new Bill()
            {
                AccountID = user.AccountID,
                ShipName = user.UserName,
                ShipMobile = user.Phone,
                ShipAddress = user.Address,
                ShipEmail = user.Email,
                SumMoney = viewModel.CartTotal
            };
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
                var cart = ShoppingCart.GetCart(this.HttpContext, user.UserName);
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
                ShoppingCart.GetCart(this.HttpContext, user.UserName).EmptyCart();
                Session.Remove("CartCount");

                new BillDAO().Insert(bill);

                return RedirectToAction("Order");
            }
            return View();
        }

        [HttpGet]
        public ActionResult Order(int? index, int?  limitTime, int? status)
        {
            var user = Session["User"] as TMDT.Account;
            if (user == null)
                return RedirectToAction("Login", "Home");
            var model=new BillDAO().ViewOrder(index,limitTime,status,user.AccountID);           
            return View(model);
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