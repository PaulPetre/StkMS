using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StkMS.Shared.Models;
using StkMS.Shared.Responses;

namespace StkMS.Client.Services.Interfaces
{
    public interface IAuthenticationService
    {

        Task<ApiResponse> RegisterUserAsync(RegisterRequest model);

        // TODO: Migrate login to IAuthenticationService 

    }
}
