namespace TMDT
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("DetailBill")]
    public partial class DetailBill
    {
        public int DetailBillID { get; set; }

        public int? BillID { get; set; }

        public int? ProductID { get; set; }

        public int? AccountID { get; set; }

        public decimal? Price { get; set; }

        public int? Quantity { get; set; }

        public virtual Account Account { get; set; }

        public virtual Bill Bill { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual Product Product { get; set; }
    }
}
