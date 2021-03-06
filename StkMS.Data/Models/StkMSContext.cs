using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using StkMS.Data.Contracts;

#nullable disable

namespace StkMS.Data.Models
{
    internal class StkMSContext : DbContext, IStkMSContext
    {
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<Stock> Stocks { get; set; }
        public virtual DbSet<Sale> Sales { get; set; }
        public virtual DbSet<SaleItem> SaleItems { get; set; }
        public virtual DbSet<Customers> Customers { get; set; }
        public virtual DbSet<Inventory> Inventory { get; set; }
        public virtual DbSet<InventoryItem> InventoryItems { get; set; }

        public StkMSContext(DbContextOptions<StkMSContext> options, IConfiguration configuration)
            : base(options)
        {
            this.configuration = configuration;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
                optionsBuilder.UseSqlServer(configuration["ConnectionStrings:default"]);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "SQL_Latin1_General_CP1_CI_AS");

            modelBuilder.Entity<Product>(
                entity =>
                {
                    entity.Property(e => e.Code).IsRequired().HasMaxLength(50);
                    entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                    entity.Property(e => e.Unit).IsRequired().HasMaxLength(50);

                    entity.Property(e => e.UnitPrice).HasColumnType("smallmoney");
                });

            modelBuilder.Entity<Stock>(
                entity =>
                {
                    entity.Property(e => e.Quantity).HasColumnType("decimal(18, 2)");

                    entity
                        .HasOne(it => it.Product)
                        .WithMany()
                        .HasForeignKey(d => d.ProductId)
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_Stocks_Products");
                }
            );

            modelBuilder.Entity<Sale>(
                entity => { entity.HasMany(e => e.Items); }
            );

            modelBuilder.Entity<Inventory>(
                entity => { entity.HasMany(e => e.Items); }
            );

            modelBuilder.Entity<InventoryItem>(
                entity =>
                {
                    entity.Property(e => e.OldPrice).HasColumnType("smallmoney");
                    entity.Property(e => e.NewPrice).HasColumnType("smallmoney");
                    entity.Property(e => e.OldQuantity).HasColumnType("decimal(18, 2)");
                    entity.Property(e => e.NewQuantity).HasColumnType("decimal(18, 2)");
                }
            );
        }

        //

        private readonly IConfiguration configuration;
    }
}