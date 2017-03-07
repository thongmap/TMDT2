namespace TMDT
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class TMDTModel : DbContext
    {
        public TMDTModel()
            : base("name=E_Models")
        {
        }

        public virtual DbSet<Account> Accounts { get; set; }
        public virtual DbSet<Bill> Bills { get; set; }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Comment> Comments { get; set; }
        public virtual DbSet<DetailBill> DetailBills { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<sysdiagram> sysdiagrams { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<cartitem> ShoppingCartItems { get; set; }
        public virtual DbSet<VoteModel> VoteModels { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Bill>()
                .Property(e => e.SumMoney)
                .HasPrecision(18, 0);

            modelBuilder.Entity<DetailBill>()
                .Property(e => e.Price)
                .HasPrecision(18, 0);

            modelBuilder.Entity<Product>()
                .Property(e => e.Price)
                .HasPrecision(18, 0);
            
            modelBuilder.Entity<Order>()
                .Property(e => e.Total)
                .HasPrecision(18, 0);
        }
    }
}
