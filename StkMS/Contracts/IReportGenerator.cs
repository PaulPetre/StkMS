using System.Collections.Generic;
using StkMS.DomainModels;

namespace StkMS.Contracts
{
    public interface IReportGenerator
    {
        byte[] Generate(IEnumerable<ProductStock> inventory);
    }
}