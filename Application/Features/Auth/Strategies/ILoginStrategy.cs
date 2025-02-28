using Shared.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Features.Auth.Strategies
{
    public interface ILoginStrategy
    {
        // Credential với google là : Authcode
        // ....       với password là : email / password
        Task<Result<AuthResponse>> LoginAsync(string credential);
    }
}
