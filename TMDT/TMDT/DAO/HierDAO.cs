using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace TMDT.DAO
{
    public class HierDAO
    {
        TMDTModel db = null;
        public HierDAO()
        {
            db = new TMDTModel();
        }
        
        public List<Hierarchary> ListAll()
        {
            return db.Hierarcharys.ToList();
        }
        public IEnumerable<SelectListItem> DropdownCategory()
        {
            IEnumerable<SelectListItem> items = db.Hierarcharys.Select(
                b => new SelectListItem { Value = b.id.ToString(), Text = b.Name });
            return items;
        }
        public string HierName(int id)
        {
            return db.Hierarcharys.Find(id).Name;
        }
    }
}