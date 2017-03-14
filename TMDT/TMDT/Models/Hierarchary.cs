namespace TMDT
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("Hierarchary")]
    public partial class Hierarchary
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Hierarchary()
        {
            
        }
        public int id { get; set; }
        public string Name { get; set; }
        public int CategoryID { get; set; }

        public virtual Category Category { get; set; }
     

    

    

       
    }
}
