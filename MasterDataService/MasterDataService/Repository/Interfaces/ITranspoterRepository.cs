using MasterDataService.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MasterDataService.Repository.Interfaces
{
    public interface ITranspoterRepository
    {
        Task<IList<Transpoter>> GetTranspotersByContainerNumber(List<string> containerList);
        Task<IList<Trailer>> GetTrailerList(List<string> trailerList);
        Task<IList<Truck>> GetTruckList(List<string> truckList);
    }
}
