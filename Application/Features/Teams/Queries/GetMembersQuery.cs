using Domain.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Teams.Queries
{
    public class GetMembersQuery : IRequest<IEnumerable<Member>>
    {
        // Có thể thêm điều kiện lọc
        public string? FullName { get; set; }
        public string? PhoneNumber { get; set; }
    }
    public class GetMembersQueryHandler : IRequestHandler<GetMembersQuery, IEnumerable<Member>>
    {
        private readonly IRepository<Member> _memberRepository;

        public GetMembersQueryHandler(IRepository<Member> memberRepository)
        {
            _memberRepository = memberRepository;
        }

        public async Task<IEnumerable<Member>> Handle(GetMembersQuery request, CancellationToken cancellationToken)
        {
            // Lấy tất cả members
            var members = await _memberRepository.GetAllAsync(cancellationToken: cancellationToken);

            // Áp dụng lọc nếu có
            if (!string.IsNullOrEmpty(request.FullName))
            {
                members = members.Where(m => m.FullName != null && m.FullName.Contains(request.FullName)).ToList();
            }
            if (!string.IsNullOrEmpty(request.PhoneNumber))
            {
                members = members.Where(m => m.PhoneNumber != null && m.PhoneNumber.Contains(request.PhoneNumber)).ToList();
            }

            return members;
        }
    }
}
