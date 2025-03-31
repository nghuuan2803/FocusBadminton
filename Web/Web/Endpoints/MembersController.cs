using Application.Common.Mappings;
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
        public async Task<ActionResult<IEnumerable<MemberDTO>>> GetMembers(
            [FromQuery] string? fullName = null,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            var query = new GetMembersQuery
            {
                FullName = fullName,
                PageNumber = pageNumber,
                PageSize = pageSize
            };
            var members = await _mediator.Send(query);
            var memberDTOs = members.Select(p => p.ToMemberDTO()).ToList();
            return Ok(memberDTOs);
        }
    }
}
