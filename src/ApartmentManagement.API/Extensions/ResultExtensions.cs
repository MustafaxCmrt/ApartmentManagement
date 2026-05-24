using ApartmentManagement.Application.Common.Models;
using Microsoft.AspNetCore.Mvc;

namespace ApartmentManagement.API.Extensions;

public static class ResultExtensions
{
    public static IActionResult ToActionResult(this Result result)
    {
        if (result.IsSuccess) return new OkResult();
        return MapError(result.Error!);
    }

    public static IActionResult ToActionResult<T>(this Result<T> result)
    {
        if (result.IsSuccess) return new OkObjectResult(result.Value);
        return MapError(result.Error!);
    }

    private static IActionResult MapError(Error error) => error.Code switch
    {
        "NotFound" => new NotFoundObjectResult(error),
        "Validation" => new BadRequestObjectResult(error),
        "Conflict" => new ConflictObjectResult(error),
        "Unauthorized" => new UnauthorizedObjectResult(error),
        "Forbidden" => new ObjectResult(error) { StatusCode = StatusCodes.Status403Forbidden },
        _ => new ObjectResult(error) { StatusCode = StatusCodes.Status500InternalServerError }
    };
}
