using Microsoft.EntityFrameworkCore;
using StkMS.Data.Models;

namespace StkMS.Data.Contracts
{
    internal interface IStkMSContext
    {
        DbSet<Product> Products { get; set; }
        DbSet<Stock> Stocks { get; set; }
        DbSet<Sale> Sales { get; set; }
        DbSet<SaleItem> SaleItems { get; set; }
        DbSet<Customers> Customers { get; set; }

        int SaveChanges();
    }
}
