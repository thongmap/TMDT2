namespace TMDT
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Product")]
    public partial class Product
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Product()
        {
            cartitems = new HashSet<cartitem>();
            Comments = new HashSet<Comment>();
            DetailBills = new HashSet<DetailBill>();
        }

        public int ProductID { get; set; }


        public int? CategoryID { get; set; }

        public int? AccountID { get; set; }

        [Required(ErrorMessage = "Yêu cầu nhập tên sản phẩm")]
        [Display(Name = "Tên sản phẩm")]
        [StringLength(100)]
        public string ProductName { get; set; }


        [Required(ErrorMessage = "Yêu cầu nhập thông tin sản phẩm")]
        [Display(Name = "Thông tin sản phẩm")]
        [StringLength(200)]
        public string Info { get; set; }

        [Required(ErrorMessage = "Yêu cầu nhập số lượng sản phẩm")]
        [Display(Name = "Số lượng")]
        [RegularExpression("[1-9]", ErrorMessage = "Số lượng giới hạn từ 1 -> 9")]
        public int? Quantity { get; set; }

        [Required(ErrorMessage = "Yêu cầu nhập giá sản phẩm")]
        [Display(Name = "Giá")]
        [Range(0,2000, ErrorMessage = "Nhập giá từ 0 đến 2000.")]
        [DisplayFormat(DataFormatString = "{0:n0}", ApplyFormatInEditMode = true)]
        public decimal? Price { get; set; }

        //[Required]
        [Display(Name = "Hình")]
        [StringLength(1000)]
        public string Image { get; set; }

        //[Required]
        [Display(Name = "Ngày tạo")]
        [DisplayFormat(DataFormatString = "{0:dd/mm/yyyy}")]
        public DateTime? CreatedDate { get; set; }

        public virtual Account Account { get; set; }

        public virtual Category Category { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<cartitem> cartitems { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Comment> Comments { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<DetailBill> DetailBills { get; set; }
    }
}
