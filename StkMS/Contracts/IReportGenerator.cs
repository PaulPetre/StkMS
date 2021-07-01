using System.Collections.Generic;
using StkMS.Library.Models;
using StkMS.ViewModels;

namespace StkMS.Contracts
{
    public interface IReportGenerator
    {
        byte[] GenerateInventory(IEnumerable<ProductStock> inventory);
        byte[] GenerateSaleReport(SaleDetailsViewModel saleDetails);
    }
}