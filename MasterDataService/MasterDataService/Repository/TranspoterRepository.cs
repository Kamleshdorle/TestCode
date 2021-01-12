using Dapper;
using MasterDataService.Models;
using MasterDataService.Repository.Interfaces;
using Microsoft.Extensions.Configuration;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace MasterDataService.Repository
{
    public class TranspoterRepository : BaseAsyncRepository, ITranspoterRepository
    {
        private readonly IConfiguration _configuration;
        LogRepository _logRepository = new LogRepository();
        public TranspoterRepository(IConfiguration configuration) : base(configuration)
        {
            _configuration = configuration;
        }

        public async Task<IList<Transpoter>> GetTranspotersByContainerNumber(List<string> containerNumberList)
        {
            foreach (string value in containerNumberList.ToList())
            {
                if (string.IsNullOrEmpty(value))
                {
                    containerNumberList.Remove(value);
                }
            }

            List<Transpoter> ContainerList = new List<Transpoter>();
            string container = string.Join(",", containerNumberList
                                            .Select(x => string.Format("'{0}'", x))); //string.Join(",", trailerList);
            try
            {
                Log.Information("GetTranspotersByContainerNumber Param : " + container);
                using (DbConnection dbConnection = new SqlConnection(_configuration.GetSection("NaviTransSQLServerDBInfo:ReaderConnectionString").Value))
                {
                    await dbConnection.OpenAsync();
                    Log.Information("Db connection open");
                    //Log.Information("Query : select Container as containernumber ,[File No_] as transportnumber from [Archived Shipment Container] where Container IN(" + container + ")");

                    

                    foreach (var item in containerNumberList)
                    {
                        var param = item.Replace("-", "").Replace("/", "").Trim();
                        //var result = await dbConnection.QueryAsync<Transpoter>(@"select Container as containernumber ,[File No_] as transportnumber from [Archived Shipment Container] where Container IN('" + param + "')  order by [timestamp]");
                        Log.Information("Query : select containernumber, transportnumber from (select Container as containernumber ,[File No_] as transportnumber, [timestamp] as 'updateddate' from [Archived Shipment Container] union all select Container as containernumber,[File No_] as transportnumber, [timestamp] as 'updateddate' from[Shipment Container]) t where t.containernumber IN('" + param + "')  order by updateddate desc");
                        _logRepository.Log(_configuration, "passed", "select containernumber, transportnumber from (select Container as containernumber ,[File No_] as transportnumber, [timestamp] as 'updateddate' from [Archived Shipment Container] union all select Container as containernumber,[File No_] as transportnumber, [timestamp] as 'updateddate' from[Shipment Container]) t where t.containernumber IN('" + param + "')  order by updateddate desc", "Info");
                        var result = await dbConnection.QueryAsync<Transpoter>(@"select containernumber, transportnumber from (select Container as containernumber ,[File No_] as transportnumber, [timestamp] as 'updateddate' from [Archived Shipment Container] union all select Container as containernumber,[File No_] as transportnumber, [timestamp] as 'updateddate' from[Shipment Container]) t where t.containernumber IN('" + param + "')  order by updateddate desc");


                        if (result?.FirstOrDefault()?.containernumber != null)
                        {
                            ContainerList.Add(new Transpoter
                            {
                                containernumber = result?.FirstOrDefault()?.containernumber,
                                transportnumber = result?.FirstOrDefault()?.transportnumber
                            });
                        }
                        else
                        {
                            ContainerList.Add(new Transpoter
                            {
                                containernumber = string.Empty,
                                transportnumber = string.Empty
                            });
                        }

                    }


                    //var result = await dbConnection.QueryAsync<Transpoter>(@"select Container as containernumber ,[File No_] as transportnumber from [Archived Shipment Container] where Container IN(" + container + ")");
                    //    ContainerList = result?.ToList();
                    Log.Information("GetTranspotersByContainerNumber Result Count : " + ContainerList.Count.ToString());
                }

            }
            catch (Exception ex)
            {
                Log.Information("Error in GetTranspotersByContainerNumber : " + ex.Message.ToString());
                var errorMessage = string.Format("Error encountered on server. Message:'{0}' when writing an object", ex.Message);
                _logRepository.Log(_configuration, "Failed", errorMessage, "Error");
            }
            return ContainerList;
        }

        public async Task<IList<Trailer>> GetTrailerList(List<string> trailerList)
        {
            foreach (string value in trailerList.ToList())
            {
                if (string.IsNullOrEmpty(value))
                {
                    trailerList.Remove(value);
                }
            }

            List<Trailer> TrailerList = new List<Trailer>();
            //string trailer = string.Join(",", trailerList
            //                                .Select(x => string.Format("'{0}'", x))); //string.Join(",", trailerList);

            try
            {
                Log.Information("GetTrailerList Param : " + trailerList);
                using (DbConnection dbConnection = new SqlConnection(_configuration.GetSection("NaviTransSQLServerDBInfo:ReaderConnectionString").Value))
                {
                    await dbConnection.OpenAsync();
                    //foreach (var item in trailerList)
                    //{
                    //var param = item.Replace("-", "").Replace("/","").Trim();

                    Log.Information("Db connection open");
                    //Log.Information("Query : select [Code Trailer] as trailernumber, Department as department, [Number plate] as numberplate from Trailer where [Number plate] IN(" + trailer + ")");

                    foreach (var item in trailerList)
                    {
                        var result = await dbConnection.QueryAsync<Trailer>(@"select [Code Trailer] as trailernumber, Department as department, [Number plate] as numberplate from Trailer where [Number plate] IN('" + item + "')");

                        if (result?.FirstOrDefault()?.numberplate != null)
                        {
                            TrailerList.Add(new Trailer
                            {
                                department = result?.FirstOrDefault()?.department,
                                numberplate = result?.FirstOrDefault()?.numberplate,
                                trailernumber = result?.FirstOrDefault()?.trailernumber//item
                            });
                        }
                        else
                        {
                            TrailerList.Add(new Trailer
                            {
                                department = string.Empty,
                                numberplate = string.Empty,
                                trailernumber = string.Empty//item
                            });
                        }
                    }
                    Log.Information("GetTrailerList Result Count : " + TrailerList.Count.ToString());

                }

            }
            catch (Exception ex)
            {
                Log.Information("Error in GetTranspotersByContainerNumber : " + ex.Message.ToString());
                var errorMessage = string.Format("Error encountered on server. Message:'{0}' when writing an object", ex.Message);
                _logRepository.Log(_configuration, "Failed", errorMessage, "Error");
            }
            return TrailerList;
        }

        public async Task<IList<Truck>> GetTruckList(List<string> truckList)
        {
            foreach (string value in truckList.ToList())
            {
                if (string.IsNullOrEmpty(value))
                {
                    truckList.Remove(value);
                }
            }

            List<Truck> TruckList = new List<Truck>();
            //string truck = string.Join(",", truckList
            //                                .Select(x => string.Format("'{0}'", x))); //string.Join(",", trailerList);

            try
            {
                Log.Information("GetTruckList Param : " + TruckList);
                using (DbConnection dbConnection = new SqlConnection(_configuration.GetSection("NaviTransSQLServerDBInfo:ReaderConnectionString").Value))
                {
                    await dbConnection.OpenAsync();
                    //foreach (var item in truckList)
                    //{
                    //var param = item.Replace("-", "").Replace("/", "").Trim();
                    Log.Information("Db connection open");
                    //Log.Information("Query : select [Code Vehicle] as trucknumber, Department as department, [Number plate] as numberplate  from Vehicle where [Number plate] IN(" + truck + ")");
                    foreach (var item in truckList)
                    {
                        var result = await dbConnection.QueryAsync<Truck>(@"select [Code Vehicle] as trucknumber, Department as department, [Number plate] as numberplate  from Vehicle where [Number plate] IN('" + item + "')");
                        if (result?.FirstOrDefault()?.numberplate != null)
                        {
                            TruckList.Add(new Truck
                            {
                                department = result?.FirstOrDefault()?.department,
                                numberplate = result?.FirstOrDefault()?.numberplate,
                                trucknumber = result?.FirstOrDefault()?.trucknumber//item
                            });
                        }
                        else
                        {
                            TruckList.Add(new Truck
                            {
                                department = string.Empty,
                                numberplate = string.Empty,
                                trucknumber = string.Empty//item
                            });
                        }
                    }
                    Log.Information("GetTruckList Result Count : " + TruckList.Count.ToString());
                }

            }
            catch (Exception ex)
            {
                Log.Information("Error in GetTranspotersByContainerNumber : " + ex.Message.ToString());
                var errorMessage = string.Format("Error encountered on server. Message:'{0}' when writing an object", ex.Message);
                _logRepository.Log(_configuration, "Failed", errorMessage, "Error");
            }
            return TruckList;
        }
    }
}
