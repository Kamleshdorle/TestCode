namespace MasterDataService.Models
{
    public class BaseResponseStatus
    {
        /// <summary>
        /// Status Code for the response
        /// </summary>
        public string StatusCode { get; set; }
        /// <summary>
        /// Status Message for the response
        /// </summary>
        public string StatusMessage { get; set; }

        /// <summary>
        /// Response Data to be shared
        /// </summary>
        public object ResponseData { get; set; }

        public object ResponseDataCount { get; set; }
    }
}