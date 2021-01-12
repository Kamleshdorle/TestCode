using Dapper;
using MasterDataService.Models;
using MasterDataService.Repository.Interfaces;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace MasterDataService.Repository
{
    public class CompanyRepository : BaseAsyncRepository, ICompanyLookupRepository
    {
        private readonly IConfiguration _configuration;
        LogRepository _logRepository = new LogRepository();
        public CompanyRepository(IConfiguration configuration) : base(configuration)
        {
            _configuration = configuration;
        }


        public async Task<IList<Company>> GetCompany()
        {
            List<Company> companyList = null;

            if (companyList is null)
            {
                using (DbConnection dbConnection = new SqlConnection(_configuration.GetSection("NaviTransSQLServerDBInfo:ReaderConnectionString").Value))
                {
                    await dbConnection.OpenAsync();
                    var result = await dbConnection.QueryAsync<Company>(@"select Name as name,Address as address,'vendor' as vendorgroup from dbo.[Produktion - AncoTrans$Company Information] union all
                                                                          select Name as name,Address as address,'BV' as vendorgroup from dbo.[Produktion - AncoTrans BV$Company Information] union all
                                                                          select Name as name,Address as address,'AG' as vendorgroup from dbo.[Produktion - AncoTrans AG$Company Information] union all
                                                                          select Name as name,Address as address,'AB' as vendorgroup from dbo.[Produktion - AncoTrans AB$Company Information] union all
                                                                          select Name as name,Address as address,'BFCTAB' as vendorgroup from dbo.[BFCT AB$Company Information]"); //select  * from dbo.Company


                    companyList = result?.ToList();
                }
            }
            return companyList;
        }
    }
}
