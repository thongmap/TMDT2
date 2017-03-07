namespace TMDT
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Bill")]
    public partial class Bill
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Bill()
        {
            DetailBills = new HashSet<DetailBill>();
        }

        public int BillID { get; set; }

        public int? AccountID { get; set; }

        [Display(Name = "Thành tiền")]
        public decimal? SumMoney { get; set; }


        public DateTime CreatedDate { get; set; }

        [Required(ErrorMessage = "Yêu cầu nhập họ và tên người nhận")]
        [Display(Name = "Họ và tên")]
        [StringLength(50)]
        public string ShipName { get; set; }

        [Required(ErrorMessage = "Yêu cầu nhập số điện thoại")]
        [Display(Name = "Số điện thoại")]
        [StringLength(50)]
        [RegularExpression("^0[1-9]{9,10}", ErrorMessage = "Nhập chính xác số điện thoại")]
        public string ShipMobile { get; set; }

        [Required(ErrorMessage = "Yêu cầu nhập địa chỉ")]
        [Display(Name = "Địa chỉ")]
        [StringLength(100)]
        public string ShipAddress { get; set; }

        [Required(ErrorMessage = "Yêu cầu nhập email")]
        [Display(Name = "Email")]
        [StringLength(50)]
        [EmailAddress(ErrorMessage = "Xin nhập chính xác e-mail")]
        public string ShipEmail { get; set; }

        [StringLength(50)]
        public string Status { get; set; }

        public virtual Account Account { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DetailBill> DetailBills { get; set; }
    }
}
