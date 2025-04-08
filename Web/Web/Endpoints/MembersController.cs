using Application.Common.Mappings;
using Application.Features.Members.Queries;
using Application.Features.Teams.Queries;
using AutoMapper.Execution;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared.Members;

namespace Web.Endpoints
{
    [Route("api/[controller]")]
    [ApiController]
    public class MembersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public MembersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: api/members
        [HttpGet]
        public async Task<ActionResult<IEnumerable<MemberDTO>>> GetMembers()
        {
            var members = await _mediator.Send(new GetMembersQuery());
            return Ok(members);
        }
    }
}
