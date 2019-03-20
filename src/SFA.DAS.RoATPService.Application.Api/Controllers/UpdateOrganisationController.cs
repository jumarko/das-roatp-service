﻿namespace SFA.DAS.RoATPService.Application.Api.Controllers
{
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using SFA.DAS.RoATPService.Application.Api.Middleware;
    using Swashbuckle.AspNetCore.SwaggerGen;
    using System.Collections.Generic;
    using System.Net;
    using MediatR;
    using Microsoft.Extensions.Logging;
    using RoATPService.Api.Types.Models;
    using System.Threading.Tasks;

    [Authorize(Roles = "RoATPServiceInternalAPI")]
    [Route("api/v1/[controller]")]
    public class UpdateOrganisationController : Controller
    {
        private readonly ILogger<UpdateOrganisationController> _logger;
        private readonly IMediator _mediator;

        public UpdateOrganisationController(ILogger<UpdateOrganisationController> logger, IMediator mediator)
        {
            _logger = logger;
            _mediator = mediator;
        }

        [HttpPost("legalName")]
        [SwaggerResponse((int) HttpStatusCode.OK, Type = typeof(bool))]
        [SwaggerResponse((int) HttpStatusCode.BadRequest, typeof(IDictionary<string, string>))]
        [SwaggerResponse((int) HttpStatusCode.InternalServerError, Type = typeof(ApiResponse))]
        [Route("update")]
        public async Task<IActionResult> UpdateLegalName([FromBody] UpdateOrganisationLegalNameRequest updateLegalNameRequest)
        {
            bool result = await _mediator.Send(updateLegalNameRequest);

            return Ok(result);
        }
    }
}
