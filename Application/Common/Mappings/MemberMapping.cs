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
                PhoneNumber = member.PhoneNumber ?? member.Account.PhoneNumber,
                Contributed = member.Contributed,
                Email = member.Email ?? member.Account.Email,
                AccountId = member.AccountId,
                CurrentTeamId = member.CurrentTeamId,
                JoinedTeamAt = member.JoinedTeamAt,
                Gender = member.Gender,
                DoB = member.DoB,
                Address = member.Address,
                Avatar = member.Account?.Avatar,
                Point = member.Account?.PersonalPoints ?? 0,
            };
        }
    }
}
