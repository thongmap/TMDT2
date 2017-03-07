using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace TMDT.DAO
{
    public class ProductDAO
    {
        TMDTModel db = null;
        public ProductDAO()
        {
            db = new TMDTModel();
        }
        public void Create(Product product)
        {
            db.Products.Add(product);
            db.SaveChanges();
        }

        public List<Product> ListAll(int id)
        {
            return db.Products.Where(x => x.AccountID == id).ToList();
        }

        public List<Product> ListAll()
        {
            return db.Products.ToList();
        }
        public List<Category> ListAllCat()
        {
            return db.Categories.ToList();
        }
        public Product CTSP(int id)
        {
            return db.Products.Find(id);
        }
        public List<Product> ListCategory(int CategoryID, int NoProductShow)
        {
            List<Product> list = db.Products.Where(x => x.CategoryID == CategoryID).Take(NoProductShow).ToList();
            foreach (var l in list)
            {
                int i = l.Image.IndexOf("|");
                l.Image = l.Image.Substring(0, i);
            }
            return list;
        }
        public List<Product> GetProduct(int categoryID)
        {
            return db.Products.Where(x => x.CategoryID == categoryID).ToList();
        }

        public IEnumerable<Account> GetMerchant_ProductID(int productid)
        {
            var model = db.Products.Where(x => x.ProductID == productid).Select(x => x.Account).AsEnumerable();
            return model;
        }

        public List<Product> ListNewProduct(int top)
        {
            List<Product> list = db.Products.OrderByDescending(x => x.CreatedDate).Take(top).ToList();
            foreach (var l in list)
            {
                int i = l.Image.IndexOf("|");
                l.Image = l.Image.Substring(0, i);
            }
            return list;
        }
    }
}