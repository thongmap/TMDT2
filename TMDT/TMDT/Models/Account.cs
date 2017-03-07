namespace TMDT
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Account")]
    public partial class Account
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Account()
        {
            Bills = new HashSet<Bill>();
            Categories = new HashSet<Category>();
            DetailBills = new HashSet<DetailBill>();
            Products = new HashSet<Product>();
            Orders = new HashSet<Order>();
        }

        public int AccountID { get; set; }

        [Required(ErrorMessage = "Yêu cầu nhập tên đăng nhập")]
        [StringLength(50)]
        [Display(Name = "Tên tài khoản")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Yêu cầu nhập mật khẩu")]
        [StringLength(50,MinimumLength=6,ErrorMessage="Độ dài mật khẩu ít nhất 6 kí tự")]
        [Display(Name = "Mật khẩu")]
        public string Pass { get; set; }

        //[RegularExpression(@"(\\d{10})|(\\d{11})")]
        [Required(ErrorMessage = "Yêu cầu nhập số điện thoại")]
        [StringLength(50)]
        [Display(Name = "Điện thoại")]
        public string Phone { get; set; }

        //[RegularExpression(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*")]
        [Required(ErrorMessage = "Yêu cầu nhập email")]
        [StringLength(50)]
        public string Email { get; set; }

        [Required(ErrorMessage = "Yêu cầu nhập địa chỉ")]
        [StringLength(50)]
        [Display(Name = "Địa chỉ")]
        public string Address { get; set; }

        public int? Level { get; set; }

        [StringLength(50)]
        public string Status { get; set; }

        [Display(Name = "Ngày hết hạn")]
        public DateTime ExpiryDate { get; set; }

        [Display(Name = "Ngày đăng kí")]
        public DateTime CreatedDate { get; set; }

        public double? Rating { get; set; }

        public int? NoRating { get; set; }

        public string LockNote { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Bill> Bills { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Category> Categories { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DetailBill> DetailBills { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Product> Products { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Order> Orders { get; set; }
    }
}
