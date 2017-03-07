using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TMDT.Logic
{
    public class ShoppingCartViewModel
    {
        public List<cartitem> CartItems { get; set; }
        public decimal CartTotal { get; set; }
    }
}