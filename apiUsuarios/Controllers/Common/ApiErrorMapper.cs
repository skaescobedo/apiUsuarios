using apiUsuarios.DTOs.Common;
using apiUsuarios.Services.Common;
using Microsoft.AspNetCore.Mvc;

namespace apiUsuarios.Controllers.Common
{
    public interface IApiErrorMapper
    {
        ActionResult ToActionResult(ControllerBase controller, ServiceError error);
    }

    public class ApiErrorMapper : IApiErrorMapper
    {
        public ActionResult ToActionResult(ControllerBase controller, ServiceError error)
        {
            var response = new ApiErrorResponse
            {
                Message = error.Message,
                Code = error.Code.ToString()
            };

            return error.Code switch
            {
                ServiceErrorCode.NotFound => controller.NotFound(response),
                ServiceErrorCode.Validation => controller.BadRequest(response),
                ServiceErrorCode.Duplicate => controller.BadRequest(response),
                ServiceErrorCode.Conflict => controller.BadRequest(response),
                _ => controller.BadRequest(response)
            };
        }
    }
}
