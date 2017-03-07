namespace TMDT
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Order")]
    public partial class Order
    {
        public int OrderID { get; set; }

        public int? AccountID { get; set; }

        public DateTime? CreatedDate { get; set; }

        public int? Quantity { get; set; }

        public decimal? Total { get; set; }

        public virtual Account Account { get; set; }

    }
}
