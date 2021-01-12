using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasterDataService.Repository.Interfaces
{
    public interface ILogRepository
    {
        Task Log(IConfiguration _configuration, string status, string systemMessage, string eventType);
    }
}
