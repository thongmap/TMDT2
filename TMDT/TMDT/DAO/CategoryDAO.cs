using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
namespace TMDT.DAO
{
    public class CategoryDAO
    {
        TMDTModel db = null;
        public CategoryDAO()
        {
            db = new TMDTModel();
        }
        public List<Category> ListAll()
        {
            return db.Categories.ToList();
        }

        public IEnumerable<SelectListItem> DropdownCategory()
        {
            IEnumerable<SelectListItem> items = db.Categories.Select(
                b => new SelectListItem { Value = b.CategoryID.ToString(), Text = b.CategoryName });
            return items;
        }
    }
}