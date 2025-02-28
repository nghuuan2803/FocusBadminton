using Shared.Auth;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Interfaces
{
    public interface ILoginStrategy
    {
        // credential ở đây sẽ là authCode đối với Google
        Task<Result<AuthResponse>> LoginAsync(string credential);
    }

}
