using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PagedList;
using TMDT.Others;
namespace TMDT.DAO
{
    public class AdminDAO
    {
        TMDTModel db = null;
        public AdminDAO()
        {
            db = new TMDTModel();
        }
        public IEnumerable<Account> ListAllUser(int page, int pageSize)
        {
            var model = db.Accounts.Where(x => x.AccountID != 0).OrderByDescending(x => x.CreatedDate).ToPagedList(page, pageSize);
            return model;
        }
        public IEnumerable<Account> ListNUser(string name,int page, int pageSize)
        {
            var model = db.Accounts.Where(x => x.AccountID != 0 && x.UserName.Contains(name)).OrderByDescending(x => x.CreatedDate).ToPagedList(page, pageSize);
            return model;
        }
        public Account ListUser(int id)
        {
            var model = db.Accounts.Find(id);
            return model;
        }
        public IEnumerable<Order> ListAllDonHang(int? year,int? month,int page, int pageSize)
        {

            var model = db.Orders.OrderBy(s => s.CreatedDate).ToPagedList(page, pageSize);
            if(year != -1 && year != null)
                model = db.Orders.Where(s => s.CreatedDate.Value.Year == year).OrderBy(s => s.CreatedDate).ToPagedList(page, pageSize);
            if (month != -1 && month != null)
                model = db.Orders.Where(s => s.CreatedDate.Value.Month == month).OrderBy(s => s.CreatedDate).ToPagedList(page, pageSize);
            return model;
        }
        public IEnumerable<Order> ListAllDonHang(int? year,int?month)
        {
            IEnumerable<Order> model = db.Orders.OrderBy(s=>s.CreatedDate);
                if (year != -1 && year != null)
                model = db.Orders.Where(s => s.CreatedDate.Value.Year == year);
            if (month != -1 && month != null)
                model = db.Orders.Where(s => s.CreatedDate.Value.Month == month);

            return model;
        }
        public IEnumerable<Bill> ListAllHoaDon(int page, int pageSize)
        {
            var model = db.Bills.OrderBy(s => s.CreatedDate).ToPagedList(page, pageSize);
            return model;
        }
        public IEnumerable<DetailBill> ListChiTietHoaDon(int id, int page, int pageSize)
        {
            var model = db.DetailBills.Where(s => s.BillID == id).OrderBy(s => s.AccountID).ToPagedList(page, pageSize);
            return model;
        }
        public IEnumerable<Account> ListRating(int page, int pageSize)
        {
            var model = db.Accounts.OrderByDescending(s => s.Rating).ToPagedList(page, pageSize);
            return model;
        }
        public IEnumerable<Account> ListOptionalRating(int isit, int page, int pageSize)
        {
            var model = db.Accounts.OrderBy(s => s.Rating).ToPagedList(page, pageSize);
            if (isit == 1)
                model = db.Accounts.Where(s=>s.NoRating > 0).OrderByDescending(s => s.Rating).Take(5).ToPagedList(page, pageSize);
            else
                model = db.Accounts.Where(s => s.NoRating > 0).OrderBy(s => s.Rating).Take(5).ToPagedList(page, pageSize);
            return model;
        }
        public void LockUser(int id,string reason)
        {
            var user = db.Accounts.Find(id);
            if (user.Status == "false")
            {
                user.Status = "true";
                user.LockNote = reason;
            }
            else
            {
                user.Status = "false";
                user.LockNote = reason;
            }
            db.SaveChanges();
        }
        public void ResetPass(int id)
        {
            var user = db.Accounts.Find(id);
            user.Pass = Encryptor.MD5Hash("123456");
            db.SaveChanges();
        }
        public string getemail(int id)
        {
            var user = db.Accounts.Find(id);
            string email = user.Email;
            return email;
        }
        public string getname(int id)
        {
            var user = db.Accounts.Find(id);
            string name = user.UserName;
            return name;
        }

        public IEnumerable<Account> FilterRating(int? search, int page, int pageSize)
        {
            var model = db.Accounts.Where(s => s.Rating >=(search-1) && s.Rating<=search).OrderByDescending(s=>s.Rating).ToPagedList(page, pageSize);
            return model;
        }
        public string getrating (int id)
        {
            var user = db.Accounts.Find(id);
            string temp = user.Rating.ToString();
            return temp.Substring(0, 3);
        }
    }
}