using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
namespace TMDT.DAO
{
    public class OrderDAO
    {
        TMDTModel db = null;
        public OrderDAO()
        {
            db = new TMDTModel();
        }
        public bool Insert(Order order)
        {
            try
            {
                db.Orders.Add(order);
                db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}