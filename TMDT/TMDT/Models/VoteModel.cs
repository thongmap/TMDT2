namespace TMDT
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("VoteModel")]
    public partial class VoteModel
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public VoteModel()
        {
            
        }
        public int? BuyerID { get; set; }

        [Key]
        public int? Id { get; set; }
      
        public int? MerchantID { get; set; }

        public int? NoVotes { get; set; }

     
    }
}
