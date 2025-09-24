using Microsoft.AspNetCore.Mvc;

namespace CRM_Homestay.App.Attributes
{
    /// <summary>
    /// InternalServerErrorObjectResult
    /// </summary>
    public class InternalServerErrorObjectResult : ObjectResult
    {
        /// <summary>
        /// InternalServerErrorObjectResult
        /// </summary>
        /// <param name="error"></param>
        public InternalServerErrorObjectResult(object error)
            : base(error)
        {
            StatusCode = StatusCodes.Status500InternalServerError;
        }
    }
}