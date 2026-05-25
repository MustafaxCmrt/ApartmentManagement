using ApartmentManagement.API.Controllers.Common;
using ApartmentManagement.API.Extensions;
using ApartmentManagement.Application.Features.Search.Queries.GlobalSearch;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ApartmentManagement.API.Controllers;

[Route("api/v{version:apiVersion}/search")]
[Authorize(Roles = "TenantAdmin")]
public class SearchController : BaseController
{
    [HttpGet]
    public async Task<IActionResult> Search([FromQuery] GlobalSearchQuery query, CancellationToken ct)
        => (await Sender.Send(query, ct)).ToActionResult();
}
