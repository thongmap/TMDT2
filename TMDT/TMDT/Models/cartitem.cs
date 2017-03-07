namespace TMDT
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    [Table("cartitem")]
    public partial class cartitem
    {
        [Key]
        public string ItemID { get; set; }
        public string CartID { get; set; }
        public int Quantity { get; set; }
        [DisplayFormat(DataFormatString = "{0:dd/mm/yyyy}")]
        public System.DateTime DateCreated { get; set; }
        public int ProductID { get; set; }


        public virtual Product Product { get; set; }
    }
}