using System;
using System.Security.Claims;
using System.Threading.Tasks;
using userMgrApi.Core.Dtos.Auth;
using userMgrApi.Core.Dtos.General;

namespace userMgrApi.Core.Interfaces
{
	public interface IAuthService
	{
		Task<GeneralServiceResponseDto> SeedRoleAsync();

		Task<GeneralServiceResponseDto> RegisterAsync(RegisterDto registerDto);

		Task<LoginServiceDto?> LoginAsync(LoginDto loginDto);

        Task<GeneralServiceResponseDto> UpdateRoleAsync(ClaimsPrincipal User, UpdateRoleDto updateRoleDto);

		Task<LoginServiceDto?> MeAsync(MeDto meDto);

        Task<IEnumerable<UserInfoResult>> GetUsersListAsync();

		Task<UserInfoResult> GetUserDetailsByUserNameAsync(string userName);

		Task<IEnumerable<string>> GetUsernamesListAsync();
    }
}


