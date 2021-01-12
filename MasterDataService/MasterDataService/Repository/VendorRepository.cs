using Dapper;
using MasterDataService.Models;
using MasterDataService.Repository.Interfaces;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace MasterDataService.Repository
{
    public class VendorRepository : BaseAsyncRepository, IVendorRepository
    {
        private IConfiguration _configuration;
        LogRepository _logRepository = new LogRepository();

        public VendorRepository(IConfiguration configuration) : base(configuration)
        {
            _configuration = configuration;
        }

        public async Task<List<string>> GetDataFromSQLServer()
        {
            var data = new List<string>();
            try
            {
                using (DbConnection dbConnection = new SqlConnection(_configuration.GetSection("NaviTransSQLServerDBInfo:ReaderConnectionString").Value))
                {
                    await dbConnection.OpenAsync();
                    data.Add("NaviTransSQLServerDBInfo - Pass");
                }
            }
            catch (Exception ex)
            {
                data.Add("NaviTransSQLServerDBInfo - Failed");
            }

            return data;
        }

        public async Task<IList<Vendor>> GetVendors()
        {
            List<Vendor> LookupList = null;
            try
            {
                if (LookupList is null)
                {
                    using (DbConnection dbConnection = new SqlConnection(_configuration.GetSection("NaviTransSQLServerDBInfo:ReaderConnectionString").Value))
                    {
                        await dbConnection.OpenAsync();
                        var result = await dbConnection.QueryAsync<Vendor>(@"select distinct Vendor.name as name ,Vendor.vendorno as vendorno,Vendor.vatno as vatno , Vendor.vendorgroup from 
                                                                             (select Name as name,[VAT Registration No_] as vatno,No_ as vendorno ,'vendor' as vendorgroup from [Produktion - AncoTrans$Vendor] 
                                                                             union all
                                                                             select Name as name,[VAT Registration No_] as vatno,No_ as vendorno ,'BV' as vendorgroup from [Produktion - AncoTrans BV$Vendor]
                                                                             union all
                                                                             select  Name as name,[VAT Registration No_] as vatno,No_ as vendorno,'AB' as vendorgroup from [Produktion - AncoTrans AB$Vendor] 
                                                                             union all
                                                                             select Name as name,[VAT Registration No_] as vatno,No_ as vendorno ,'AG' as vendorgroup from [Produktion - AncoTrans AG$Vendor]
                                                                             union all
                                                                             select  Name as name,[VAT Registration No_] as vatno,No_ as vendorno ,'BFCTAB' as vendorgroup from [BFCT AB$Vendor]) as Vendor");
                        LookupList = result?.ToList();
                    }
                }
            }
            catch (Exception ex)
            {
                var errorMessage = string.Format("Error encountered on server. Message:'{0}' when writing an object", ex.Message);
                _logRepository.Log(_configuration, "Failed", errorMessage, "Error");
            }
            return LookupList;
        }

    }
}
