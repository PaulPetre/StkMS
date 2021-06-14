using Microsoft.EntityFrameworkCore;
using StkMS.Data.Models;

namespace StkMS.Data.Contracts
{
    public interface IStkMSContext
    {
        DbSet<Product> Products { get; set; }
        DbSet<Stock> Stocks { get; set; }

        int SaveChanges();
    }
}