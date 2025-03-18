using Shared.Members;

namespace Application.Common.Mappings
{
    public static class MemberMapping
    {
        public static MemberDTO ToMemberDTO(this Member member)
        {
            return new MemberDTO
            {
                Id = member.Id,
                FullName = member.FullName,
                PhoneNumber = member.PhoneNumber,
                Contributed = member.Contributed,
                Email = member.Email,
                AccountId = member.AccountId,
                CurrentTeamId = member.CurrentTeamId,
                JoinedTeamAt = member.JoinedTeamAt,
                Gender = member.Gender,
                DoB = member.DoB,
                Address = member.Address
            };
        }
    }
}
