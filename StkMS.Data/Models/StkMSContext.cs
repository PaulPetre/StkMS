using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using StkMS.Data.Contracts;

#nullable disable

namespace StkMS.Data.Models
{
    public class StkMSContext : DbContext, IStkMSContext
    {
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<Stock> Stocks { get; set; }

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
                    entity.Property(e => e.Code)
                        .IsRequired()
                        .HasMaxLength(50);

                    entity.Property(e => e.Name)
                        .IsRequired()
                        .HasMaxLength(100);

                    entity.Property(e => e.Unit)
                        .IsRequired()
                        .HasMaxLength(50);

                    entity.Property(e => e.UnitPrice).HasColumnType("smallmoney");
                });

            modelBuilder.Entity<Stock>(
                entity =>
                {
                    entity.Property(e => e.Quantity).HasColumnType("decimal(10, 0)");

                    entity
                        .HasOne(d => d.Product)
                        .WithMany()
                        .HasForeignKey(d => d.ProductId)
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK_Stocks_Products");
                }
            );
        }

        //

        private readonly IConfiguration configuration;
    }
}