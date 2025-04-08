using Application.Common.Mappings;
using Domain.Repositories;
using Shared.Members;

namespace Application.Features.Members.Queries
{
    public class GetMembersQuery : IRequest<IEnumerable<MemberDTO>>
    {
    }
    public class GetMembersQueryHandler : IRequestHandler<GetMembersQuery, IEnumerable<MemberDTO>>
    {
        private readonly IRepository<Member> _memberRepository;
        public GetMembersQueryHandler(IRepository<Member> memberRepository)
        {
            _memberRepository = memberRepository;
        }
        public async Task<IEnumerable<MemberDTO>> Handle(GetMembersQuery request, CancellationToken cancellationToken)
        {
            var members = await _memberRepository.GetAllAsync(null,cancellationToken);
            return members.Select(m => m.ToMemberDTO());
        }
    }
}
