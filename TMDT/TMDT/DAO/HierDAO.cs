using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

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
    }
}