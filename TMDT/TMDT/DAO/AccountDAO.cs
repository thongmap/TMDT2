using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TMDT.Models;
namespace TMDT.DAO
{
    public class AccountDAO
    {
        TMDTModel db = null;
        public AccountDAO()
        {
            db = new TMDTModel();
        }
        public int Login(string username, string password)
        {
            var result = db.Accounts.SingleOrDefault(x => x.UserName == username && x.Pass == password);
            if (result == null)
                return 0;
            else
                return 1;
        }

        public Account GetUser(string username)
        {
            return db.Accounts.SingleOrDefault(x => x.UserName == username);
        }

        public int GetIdUser(string username)
        {
            var user = db.Accounts.SingleOrDefault(x => x.UserName == username);
            int id = user.AccountID;
            return id;
        }

        public bool CheckUserName(string username)
        {
            return db.Accounts.Count(x => x.UserName == username) > 0;
        }
        public bool CheckEmail(string email)
        {
            return db.Accounts.Count(x => x.Email == email) > 0;
        }

        public long Insert(Account entity)
        {
            db.Accounts.Add(entity);
            db.SaveChanges();
            return entity.AccountID;
        }

        public Account FindById(int id)
        {
            return db.Accounts.Find(id);
        }

        public void UpdateStatusUser(int id)
        {
            var user = db.Accounts.Find(id);
            user.Status = "true";
            db.SaveChanges();
        }

        public void UpgradeAccount(int id)
        {
            var user = db.Accounts.Find(id);
            user.Level = 1;
            user.ExpiryDate = DateTime.Now.AddDays(30);
            db.SaveChanges();
        }

        public void UpdateExpiryDate(int id)
        {
            var user = db.Accounts.Find(id);
            if (user.ExpiryDate > DateTime.Now)
                user.ExpiryDate = user.ExpiryDate.AddDays(30);
            else
                user.ExpiryDate = DateTime.Now.AddDays(30);
            db.SaveChanges();
        }

        public DaysLeft CountDayLeft(int id)
        {
            var user = db.Accounts.Find(id);
            DaysLeft daysleft = new DaysLeft();
            TimeSpan days = user.ExpiryDate.Subtract(DateTime.Now);
            daysleft.Days = days.Days + 1;
            TimeSpan timepast = DateTime.Now - user.CreatedDate;
            int timepast1 = timepast.Days;
            int percent = timepast1 * 100 / 30;
            daysleft.Percent = percent;
            return daysleft;
        }

        public Account GetUser_ID(int? id)
        {
            var model = db.Accounts.Find(id);
            return model;
        }

        public void UpdateAcc(Account acc)
        {
            var model = db.Accounts.Find(acc.AccountID);
            model.Phone = acc.Phone;
            model.Email = acc.Email;
            model.Address = acc.Address;
            db.SaveChanges();
        }

        public bool CheckPass(int userid, string pass)
        {
            var user=db.Accounts.Find(userid);
            if (user.Pass == pass)
            {
                return true;
            }
            else
                return false;
        }

        public void ChangePass(string newpass, int id)
        {
            var user = db.Accounts.Find(id);
            user.Pass = newpass;
            db.SaveChanges();
        }
        public void DownGradeAcc(int? id)
        {
            Account original = db.Accounts.Find(id);
            if (original != null)
            {
                original.Level = 2;
                db.SaveChanges();
            }
        }   
    }
}