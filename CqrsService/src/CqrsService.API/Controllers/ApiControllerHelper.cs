using CqrsService.Domain.Configuration.Framework;
using CqrsService.Domain.ErrorResponses;
using Microsoft.AspNetCore.Mvc;

namespace CqrsService.API.Controllers;

public static class ApiControllerHelper
{
    public static async Task<IActionResult> HttpResponse<ResponseType>(Response<ResponseType> response)
    {
        if (response is null)
        {
            return new BadRequestResult();
        }

        if (response.Status == Status.Success)
        {
            if (response.Content is not null)
            {
                return new OkObjectResult(response.Content);
            }
            else
            {
                return new NoContentResult();
            }
        }
        else
        {
            //check for validation error responses
            if (response.ErrorResponse is not null && response.ErrorResponse is DomainValidationErrorResponse)
            {
                return new BadRequestObjectResult(response.ErrorResponse);
            }

            //check for exception error responses
            if (response.ErrorResponse is not null && response.ErrorResponse is SystemErrorResponse)
            {
                return new ObjectResult(response.ErrorResponse) { StatusCode = StatusCodes.Status500InternalServerError };
            }

            return new ObjectResult(null) { StatusCode = StatusCodes.Status500InternalServerError };
        }
    }
}