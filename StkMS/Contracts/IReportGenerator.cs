using System.Collections.Generic;
using StkMS.Library.Models;

namespace StkMS.Contracts
{
    public interface IReportGenerator
    {
        byte[] Generate(IEnumerable<ProductStock> inventory);
    }
}