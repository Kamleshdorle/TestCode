using MasterDataService.Models;
using MasterDataService.Repository.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;
using Serilog;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace MasterDataService.Controllers
{
    [Route("api/[controller]")]
    public class MasterDataController : Controller
    {
        private ITranspoterRepository transporterRepository;
        private ICompanyLookupRepository lookupRepository;
        private IVendorRepository vendorRepository;
        private readonly ILogRepository _logRepository;
        private IConfiguration _configuration;

        public MasterDataController(IConfiguration configuration, ITranspoterRepository _transporterRepository, ICompanyLookupRepository _lookupRepository, IVendorRepository _vendorRepository, ILogRepository logRepository)
        {
            transporterRepository = _transporterRepository;
            lookupRepository = _lookupRepository;
            vendorRepository = _vendorRepository;
            _logRepository = logRepository;
            _configuration = configuration;
        }

        /// <summary>
        /// Test API endpoint to check service is running
        /// </summary>
        /// <returns></returns>
        [HttpGet("HealthCheck")]
        public async Task<ActionResult> HealthCheck()
        {
            Log.Information("Calling from MasterData service - HealthCheck");
            BaseResponseStatus responseDetails = new BaseResponseStatus();
            responseDetails.StatusCode = StatusCodes.Status200OK.ToString();
            responseDetails.StatusMessage = string.Format("MasterData service is running");
            responseDetails.ResponseData = null;
            return Ok(responseDetails);
        }


        [HttpGet("TestDatabase")]
        public async Task<ActionResult> TestDatabase()
        {
            var dataList = await vendorRepository.GetDataFromSQLServer();
            BaseResponseStatus responseDetails = new BaseResponseStatus();
            responseDetails.StatusCode = StatusCodes.Status200OK.ToString();
            responseDetails.StatusMessage = string.Format("MasterData service to fetch data from navitans database");
            responseDetails.ResponseData = dataList;
            return Ok(responseDetails);
        }

        /// <summary>
        /// Get all company details
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetCompany")]
        public async Task<ActionResult> GetCompany()
        {
            Log.Information("Calling from MasterData service - GetCompany");
            BaseResponseStatus responseDetails = new BaseResponseStatus();
            try
            {
                //_logRepository.Log(_configuration, "Passed", "MasterDataController-GetCompany - Entering GetCompany", "Info");

                //_logRepository.Log(_configuration, "Passed", "MasterDataController-GetCompany - Calling GetCompany method", "Info");
                var companyList = await lookupRepository.GetCompany();
                //_logRepository.Log(_configuration, "Passed", "MasterDataController-GetCompany - Executed GetCompany method", "Info");

                if (companyList == null)
                {
                    Log.Information("Calling from MasterData service - No company found ");
                    responseDetails.StatusCode = StatusCodes.Status404NotFound.ToString();
                    responseDetails.StatusMessage = responseDetails.StatusMessage;
                    return Ok(responseDetails);
                }
                Log.Information("Calling from MasterData service - Companies found : " + companyList.Count);
                responseDetails.StatusCode = StatusCodes.Status200OK.ToString();
                responseDetails.StatusMessage = string.Format("Get company list");
                responseDetails.ResponseData = companyList;
            }
            catch (System.Exception ex)
            {
                var errorMessage = string.Format("Error encountered on server. Message:'{0}' when writing an object", ex.Message);
                //_logRepository.Log(_configuration, "Failed", errorMessage, "Error");
                //return NotFound();
                Log.Information("Error in MasterData service GetCompany : " + ex.Message);
                responseDetails.StatusCode = StatusCodes.Status500InternalServerError.ToString();
                responseDetails.StatusMessage = ex.Message.ToString();
                responseDetails.ResponseData = null;
            }
            //_logRepository.Log(_configuration, "Passed", "MasterDataController-GetCompany - Executed GetCompany", "Info");
            return Ok(responseDetails);
        }

        /// <summary>
        /// Fetch all transporter details by container number
        /// </summary>
        /// <param name="containerList"></param>
        /// <returns></returns>
        [HttpPost("FetchTransportersList")]
        public async Task<ActionResult> FetchTransportersList([FromBody] List<string> containerList)
        {
            try
            {
                Log.Information("Calling from MasterData service - FetchTransportersList");
                _logRepository.Log(_configuration, "Passed", "MasterDataController-GetTransporters - Entering GetTransporters", "Info");

                _logRepository.Log(_configuration, "Passed", "MasterDataController-GetTransporters - Entering GetTranspotersByContainerNumber method", "Info");

                Log.Information("Before calling - GetTranspotersByContainerNumber");
                var transporterList = await transporterRepository.GetTranspotersByContainerNumber(containerList);
                Log.Information("After calling - GetTranspotersByContainerNumber");

                Log.Information("GetTranspotersByContainerNumber result : " + transporterList.Count.ToString());

                _logRepository.Log(_configuration, "Passed", "MasterDataController-GetTransporters - Executed GetTranspotersByContainerNumber method", "Info");

                BaseResponseStatus responseDetails = new BaseResponseStatus
                {
                    //Add BaseResponseStatus details
                    StatusCode = StatusCodes.Status200OK.ToString(),
                    StatusMessage = string.Format("Get transporter list by container number"),
                    ResponseData = transporterList
                };
                _logRepository.Log(_configuration, "Passed", "MasterDataController-GetTransporters - Executed GetTransporters", "Info");
                return Ok(responseDetails);
            }
            catch (System.Exception ex)
            {
                var errorMessage = string.Format("Error encountered on server. Message:'{0}' when writing an object", ex.Message);
                _logRepository.Log(_configuration, "Failed", errorMessage, "Error");
                return NotFound();
            }
        }

        /// <summary>
        /// Get all vendor information
        /// </summary>
        [HttpGet("GetVendors")]
        public async Task<ActionResult> GetVendors()
        {
            try
            {
                _logRepository.Log(_configuration, "Passed", "MasterDataController-GetVendors - Entering GetVendors", "Info");

                _logRepository.Log(_configuration, "Passed", "MasterDataController-GetVendors - Entering GetDataFromSQLServer method", "Info");
                var isEmp = await vendorRepository.GetDataFromSQLServer();
                _logRepository.Log(_configuration, "Passed", "MasterDataController-GetVendors - Exiting GetDataFromSQLServer method", "Info");

                var vendors = await vendorRepository.GetVendors();
                BaseResponseStatus responseDetails = new BaseResponseStatus
                {
                    //Add BaseResponseStatus details
                    StatusCode = StatusCodes.Status200OK.ToString(),
                    StatusMessage = string.Format("Get vendor information list"),
                    ResponseData = vendors
                };
                _logRepository.Log(_configuration, "Passed", "MasterDataController - GetVendors - Executed GetVendors", "Info");
                return Ok(responseDetails);
            }
            catch (System.Exception ex)
            {
                var errorMessage = string.Format("Error encountered on server. Message:'{0}' when writing an object", ex.Message);
                _logRepository.Log(_configuration, "Failed", errorMessage, "Error");
                return NotFound();
            }
        }

        [HttpPost("FetchTrailerList")]
        public async Task<ActionResult> FetchTrailerList([FromBody] List<string> trailerList)
        {
            try
            {
                _logRepository.Log(_configuration, "Passed", "MasterDataController - FetchTrailerList - Entering FetchTrailerList", "Info");

                _logRepository.Log(_configuration, "Passed", "MasterDataController - FetchTrailerList - Entering GetTrailerList", "Info");
                var resultList = await transporterRepository.GetTrailerList(trailerList);
                _logRepository.Log(_configuration, "Passed", "MasterDataController - FetchTrailerList - Executed GetTrailerList", "Info");

                BaseResponseStatus responseDetails = new BaseResponseStatus
                {
                    //Add BaseResponseStatus details
                    StatusCode = StatusCodes.Status200OK.ToString(),
                    StatusMessage = string.Format("Fetched trailer list"),
                    ResponseData = resultList
                };
                _logRepository.Log(_configuration, "Passed", "MasterDataController - FetchTrailerList - Executed FetchTrailerList", "Info");
                return Ok(responseDetails);
            }
            catch (System.Exception ex)
            {
                var errorMessage = string.Format("Error encountered on server. Message:'{0}' when writing an object", ex.Message);
                _logRepository.Log(_configuration, "Failed", errorMessage, "Error");
                return NotFound();
            }
        }

        [HttpPost("FetchTruckList")]
        public async Task<ActionResult> FetchTruckList([FromBody] List<string> truckList)
        {
            try
            {
                _logRepository.Log(_configuration, "Passed", "MasterDataController - FetchTruckList - Entering FetchTruckList", "Info");

                _logRepository.Log(_configuration, "Passed", "MasterDataController - FetchTruckList - Entering FetchTruckList", "Info");
                var resultList = await transporterRepository.GetTruckList(truckList);
                _logRepository.Log(_configuration, "Passed", "MasterDataController - FetchTruckList - Executed FetchTruckList", "Info");

                BaseResponseStatus responseDetails = new BaseResponseStatus
                {
                    //Add BaseResponseStatus details
                    StatusCode = StatusCodes.Status200OK.ToString(),
                    StatusMessage = string.Format("Fetched trailer list"),
                    ResponseData = resultList
                };
                _logRepository.Log(_configuration, "Passed", "MasterDataController - FetchTruckList - Executed FetchTruckList", "Info");
                return Ok(responseDetails);
            }
            catch (System.Exception ex)
            {
                var errorMessage = string.Format("Error encountered on server. Message:'{0}' when writing an object", ex.Message);
                _logRepository.Log(_configuration, "Failed", errorMessage, "Error");
                return NotFound();
            }
        }
    }
}
