using MasterDataService.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MasterDataService.Repository.Interfaces
{
    public interface ICompanyLookupRepository
    {
        Task<IList<Company>> GetCompany();
    }
}
