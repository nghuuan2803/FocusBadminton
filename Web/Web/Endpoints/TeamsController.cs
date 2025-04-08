using Domain.Entities;
using Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Shared.Teams;

namespace Web.Endpoints
{
    [Route("api/[controller]")]
    [ApiController]
    public class TeamsController : ControllerBase
    {
        private readonly AppDbContext _dbContext;
        private readonly DbSet<Team> _teams;
        private readonly DbSet<Member> _member;

        public TeamsController(AppDbContext dbContext)
        {
            _dbContext = dbContext;
            _teams = dbContext.Teams;
            _member = dbContext.Members;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var teams = await _teams
                .ToListAsync();
            return Ok(teams);
        }


        [HttpGet("{id}")]
        public IActionResult Get(int id)
        {
            var team = _teams
                .Include(t => t.Leader)
                .Include(t => t.Members)
                .FirstOrDefault(t => t.Id == id);
            return Ok(team);
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] CreateTeamRequest request)
        {
            var leader = await _member.FindAsync(request.LeaderId);
            if (leader == null)
            {
                return BadRequest("Leader not found");
            }
            if (leader.CurrentTeamId != null)
            {
                return BadRequest("Leader already in a team");
            }
            if (request.LeaderId == 0)
            {
                return BadRequest("LeaderId is required");
            }
            var team = new Team
            {
                Name = request.Name,
                LeaderId = request.LeaderId,
                Image = request.Image,
                TeamPoints = 0,
                RewardPoints = 0
            };
            leader.JoinedTeamAt = DateTimeOffset.UtcNow;
            await _teams.AddAsync(team);
            await _dbContext.SaveChangesAsync();
            _member.Update(leader);
            leader.CurrentTeamId = team.Id;
            await _dbContext.SaveChangesAsync();
            return Ok(team.Id);
        }

        [HttpPost("members")]
        public async Task<IActionResult> AddMember(AddMembersRequest request)
        {
            var team = await _teams
                .Include(t => t.Leader)
                .FirstOrDefaultAsync(t => t.Id == request.TeamId);
            if (team == null)
            {
                return BadRequest("Team not found");
            }
            var leader = await _member
                .FirstOrDefaultAsync(m => m.Id == request.LeaderId);
            if (leader == null)
            {
                return BadRequest("Leader not found");
            }
            if (leader.CurrentTeamId != team.Id)
            {
                return BadRequest("Leader not in this team");
            }

            foreach (var member in request.Members)
            {
                var newMember = await _member
                    .FirstOrDefaultAsync(m => m.FullName == member.Name ||
                    m.PhoneNumber == member.PhoneNumber ||
                    m.Email == member.Email);

                if (newMember != null)
                {
                    if (newMember.CurrentTeamId != null)
                    {
                        return BadRequest("Member already in a team");
                    }
                    newMember.CurrentTeamId = team.Id;
                    newMember.JoinedTeamAt = DateTimeOffset.UtcNow;
                    newMember.UpdatedAt = DateTimeOffset.UtcNow;
                    newMember.UpdatedBy = team.Leader.FullName;
                    _member.Update(newMember);
                    continue;
                }
                newMember = new Member
                {
                    FullName = member.Name,
                    Email = member.Email,
                    PhoneNumber = member.PhoneNumber,
                    CreatedAt = DateTimeOffset.UtcNow,
                    CreatedBy = team.Leader.FullName,
                    CurrentTeamId = team.Id,
                    JoinedTeamAt = DateTimeOffset.UtcNow
                };
                await _member.AddAsync(newMember);
            }
            await _dbContext.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("remove-member")]
        public async Task<IActionResult> RemoveMember([FromBody] RemoveMemberRequest request)
        {
            var team = await _teams
                .Include(t => t.Leader)
                .FirstOrDefaultAsync(t => t.Id == request.TeamId);
            if (team == null)
            {
                return BadRequest("Team not found");
            }
            if (team.LeaderId != request.LeaderId)
            {
                return BadRequest("Only the team leader can remove members");
            }
            var member = await _member
                .Include(m => m.CurrentTeam)
                .FirstOrDefaultAsync(m => m.Id == request.MemberId);
            if (member == null)
            {
                return BadRequest("Member not found");
            }
            if (member.CurrentTeamId == null || member.CurrentTeamId != request.TeamId)
            {
                return BadRequest("Member not in a team");
            }
            member.CurrentTeamId = null;
            member.JoinedTeamAt = null;
            member.UpdatedAt = DateTimeOffset.UtcNow;
            member.UpdatedBy = team.Leader.FullName;
            _member.Update(member);
            await _dbContext.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("resign")]
        public async Task<IActionResult> Resign()
        {
            return Ok();
        }

        [HttpPost("quit")]
        public async Task<IActionResult> Quit([FromBody] QuitTeamRequest request)
        {
            var member = await _member
                .Include(m => m.CurrentTeam)
                .FirstOrDefaultAsync(m => m.Id == request.MemberId);
            if (member == null)
            {
                return BadRequest("Member not found");
            }
            if (member.CurrentTeamId == null)
            {
                return BadRequest("Member not in a team");
            }
            if (member.CurrentTeam.Id != request.TeamId)
            {
                return BadRequest("Member not in this team");
            }
            if (member.CurrentTeam.LeaderId == member.Id)
            {
                return BadRequest("Leader cannot quit the team");
            }
            member.OldTeam = member.CurrentTeamId;
            member.CurrentTeamId = null;
            member.JoinedTeamAt = null;
            member.UpdatedAt = DateTimeOffset.UtcNow;
            member.UpdatedBy = member.FullName;
            _member.Update(member);
            await _dbContext.SaveChangesAsync();
            return Ok();
        }

        [HttpPost("update-contribute")]
        public async Task<IActionResult> UpdateBudget([FromBody] UpdateTeamContributeRequest request)
        {
            var team = await _teams
                .Include(t => t.Leader)
                .FirstOrDefaultAsync(t => t.Id == request.TeamId);
            if (team == null)
            {
                return BadRequest("Team not found");
            }
            if (team.LeaderId != request.LeaderId)
            {
                return BadRequest("Only the team leader can update budget");
            }

            var contributes = _dbContext.ContributionHistories;
            foreach (var member in request.MemberContributes)
            {
                var teamMember = await _member
                    .FirstOrDefaultAsync(m => m.Id == member.Key);
                if (teamMember == null)
                {
                    return BadRequest("Member not found");
                }
                if (teamMember.CurrentTeamId != request.TeamId)
                {
                    return BadRequest("Member not in this team");
                }

                var contribute = new ContributionHistory
                {
                    MemberId = teamMember.Id,
                    TeamId = team.Id,
                    Amount = member.Value,
                    Date = DateTimeOffset.UtcNow
                };
                await contributes.AddAsync(contribute);
            }
            _teams.Update(team);
            await _dbContext.SaveChangesAsync();
            return Ok();
        }

        [HttpGet("members/{teamId}")]
        public async Task<IActionResult> GetMember(int teamId)
        {
            var team = await _teams
                .Include(t => t.Members)
                .FirstOrDefaultAsync(t => t.Id == teamId);

            var members = await _member.Where(p=>p.CurrentTeamId==teamId).ToListAsync();

            team.Members = members;
            if (team == null)
            {
                return BadRequest("Team not found");
            }
            var contributes = await _dbContext.ContributionHistories
                .Where(c => c.TeamId == teamId)
                .ToListAsync();
            List<TeamMemberDTO> membersDtos = new();
            foreach (var member in team.Members)
            {
                var memberContributes = contributes
                    .Where(c => c.MemberId == member.Id)
                    .OrderByDescending(c => c.Date)
                    .FirstOrDefault();

                var memberDto = new TeamMemberDTO
                {
                    Id = member.Id,
                    Name = member.FullName,
                    PhoneNumber = member.PhoneNumber,
                    Email = member.Email,
                    IsLeader = member.Id == team.LeaderId,
                };
                memberDto.CurrentContribute = memberContributes?.Amount ?? 0;
                membersDtos.Add(memberDto);
            }
            return Ok(membersDtos);
        }
    }
}
