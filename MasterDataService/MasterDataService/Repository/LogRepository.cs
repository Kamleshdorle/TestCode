using System;
using MasterDataService.Models;
using MasterDataService.Repository.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Threading.Tasks;
namespace MasterDataService.Repository
{
    public class LogRepository : ILogRepository
    {
        public async Task Log(IConfiguration _configuration, string status, string systemMessage, string eventType)
        {
            //...Log Audit Information...//
            AuditLog log = new AuditLog();
            log.logname = "VendorInvoice - MasterData Service";
            log.description = string.Empty;
            log.status = status;
            log.systemmessage = systemMessage;
            log.typeofevent = eventType;
            log.userid = string.Empty;
            log.changeddetails = string.Empty;

            var urlOCR = _configuration.GetSection("AncoTransAPI").GetSection("AuditLogService").Value;
            var client = new HttpClient();

            //AuditLog Service calls
            client.PostAsJsonAsync(urlOCR, log);
        }
    }
}
