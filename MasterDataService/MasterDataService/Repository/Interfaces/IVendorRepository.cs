using MasterDataService.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MasterDataService.Repository.Interfaces
{
    public interface IVendorRepository
    {
        Task<IList<Vendor>> GetVendors();
        Task<List<string>> GetDataFromSQLServer();
    }
}
