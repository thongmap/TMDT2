namespace TMDT
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Comment")]
    public partial class Comment
    {
        public int CommentID { get; set; }

        public int? ProductID { get; set; }

        public int? AccountID { get; set; }

        [Column("Comment")]
        [StringLength(200)]
        public string Comment1 { get; set; }

        public virtual Product Product { get; set; }
    }
}
