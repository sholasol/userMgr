using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using userMgrApi.Core.Constants;
using userMgrApi.Core.Dtos.Auth;
using userMgrApi.Core.Dtos.General;
using userMgrApi.Core.Entities;
using userMgrApi.Core.Interfaces;

namespace userMgrApi.Core.Services
{
	public class AuthService : IAuthService
	{
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogService _logService;
        private readonly IConfiguration _configuration;


		public AuthService(
            UserManager<ApplicationUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ILogService logService,
            IConfiguration configuration
            )
		{
            _userManager = userManager;
            _roleManager = roleManager;
            _logService = logService;
            _configuration = configuration;
		}




        public async Task<GeneralServiceResponseDto> SeedRoleAsync()
        {
            bool isOwnerRoleExist = await _roleManager.RoleExistsAsync(StaticUserRoles.OWNER);

            bool isAdminRoleExist = await _roleManager.RoleExistsAsync(StaticUserRoles.ADMIN);

            bool isManagerRoleExist = await _roleManager.RoleExistsAsync(StaticUserRoles.MANAGER);

            bool isUserRoleExist = await _roleManager.RoleExistsAsync(StaticUserRoles.USER);


            if(isOwnerRoleExist && isAdminRoleExist && isManagerRoleExist && isUserRoleExist)
            {
                return new GeneralServiceResponseDto()
                {
                    isSucceed = true,
                    StatusCode = 200,
                    Message = "Role seeding is already done"
                };
                
            }

            await _roleManager.CreateAsync(new IdentityRole(StaticUserRoles.OWNER));

            await _roleManager.CreateAsync(new IdentityRole(StaticUserRoles.ADMIN));

            await _roleManager.CreateAsync(new IdentityRole(StaticUserRoles.MANAGER));

            await _roleManager.CreateAsync(new IdentityRole(StaticUserRoles.USER));

            return new GeneralServiceResponseDto()
            {
                isSucceed = true,
                StatusCode = 201,
                Message = "Role seeding done successfully"
            };
        }



        public async Task<GeneralServiceResponseDto> RegisterAsync(RegisterDto registerDto)
        {
            var isUserExist = await _userManager.FindByNameAsync(registerDto.UserName);

            if(isUserExist is not null)
                return new GeneralServiceResponseDto()
                {
                    isSucceed = false,
                    StatusCode = 409,
                    Message = "Username already exists"
                };

            ApplicationUser newUser = new ApplicationUser()
            {
                FirstName = registerDto.FirstName,
                LastName = registerDto.LastName,
                Email = registerDto.Email,
                UserName = registerDto.UserName,
                Address = registerDto.Address,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            var createUserResult = await _userManager.CreateAsync(newUser, registerDto.Password);

            if (!createUserResult.Succeeded)
            {
                var erroString = "User registration failed";
                foreach (var error in createUserResult.Errors)
                {
                    erroString += "#" + error.Description;
                }
                return new GeneralServiceResponseDto()
                {
                    isSucceed = false,
                    StatusCode = 400,
                    Message = erroString
                };
            }
            //add a default USER role

            await _userManager.AddToRoleAsync(newUser, StaticUserRoles.USER);

            await _logService.SaveNewLog(newUser.UserName, "Registered to the website");

            return new GeneralServiceResponseDto()
            {
                isSucceed = true,
                StatusCode = 201,
                Message = "Username registration successfully"
            };


        }


        public async Task<LoginServiceDto?> LoginAsync(LoginDto loginDto)
        {
            //find if user exists
            var user = await _userManager.FindByNameAsync(loginDto.UserName);

            if (user is null)
                return null;

            //check user password
            var isPasswordCorrect = await _userManager.CheckPasswordAsync(user, loginDto.Password);
            if (!isPasswordCorrect)
                return null;

            //return token to frontend
            var newToken = await GenerateJWTTokenAsync(user);
            var roles = await _userManager.GetRolesAsync(user);
            var userInfo = GenerateUserInfoObject(user, roles);

            await _logService.SaveNewLog(user.UserName, "New Login");

            return new LoginServiceDto()
            {
                NewToken = newToken,
                UserInfo = userInfo
            };

        }


        public async Task<GeneralServiceResponseDto> UpdateRoleAsync(ClaimsPrincipal User, UpdateRoleDto updateRoleDto)
        {
            var user = await _userManager.FindByNameAsync(updateRoleDto.UserName);

            if (user is null)
                return new GeneralServiceResponseDto()
                {
                    isSucceed = false,
                    StatusCode = 404,
                    Message = "Invalid username"
                };

            var userRoles = await _userManager.GetRolesAsync(user);

            //owner and admin
            if(User.IsInRole(StaticUserRoles.ADMIN))
            {
                //user admin
                if(updateRoleDto.NewRole ==RoleType.USER || updateRoleDto.NewRole == RoleType.MANAGER)
                {
                    //admin can change the role of everyone except owners and admins
                    if(userRoles.Any(q=>q.Equals(StaticUserRoles.OWNER) || q.Equals(StaticUserRoles.ADMIN)))
                    {
                        return new GeneralServiceResponseDto()
                        {
                            isSucceed = false,
                            StatusCode = 403,
                            Message = "You are not allowed to change this role"
                        };
                    }
                    else
                    {
                        await _userManager.RemoveFromRolesAsync(user, userRoles);
                        await _userManager.AddToRoleAsync(user, updateRoleDto.NewRole.ToString());
                        await _logService.SaveNewLog(user.UserName, "User roles updated");

                        return new GeneralServiceResponseDto()
                        {
                            isSucceed = true,
                            StatusCode = 200,
                            Message = "User role updated successfully"
                        };
                    }
                }
                else return new GeneralServiceResponseDto()
                {
                    isSucceed = false,
                    StatusCode = 403,
                    Message = "You are not allowed to change role for this user"
                };
            }
            else
            {
                //user is owner
                if(userRoles.Any(q=>q.Equals(StaticUserRoles.OWNER)))
                {
                    return new GeneralServiceResponseDto()
                    {
                        isSucceed = false,
                        StatusCode = 403,
                        Message = "You are not allowed to change this user role username"
                    };
                }
                else
                {
                    await _userManager.RemoveFromRolesAsync(user, userRoles);
                    await _userManager.AddToRoleAsync(user, updateRoleDto.NewRole.ToString());
                    await _logService.SaveNewLog(user.UserName, "User role updated successfully");

                    return new GeneralServiceResponseDto()
                    {
                        isSucceed = true,
                        StatusCode = 200,
                        Message = "User role updated successfully"
                    };
                }
            }
        }


        public async Task<LoginServiceDto?> MeAsync(MeDto meDto)
        {
            ClaimsPrincipal handler = new JwtSecurityTokenHandler().ValidateToken(meDto.Token, new TokenValidationParameters()
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidIssuer = _configuration["JWT:ValidIssuer"],
                ValidAudience = _configuration["JWT:ValidAudience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]))
            }, out SecurityToken securityToken);

            string decodedUserName = handler.Claims.First(q => q.Type == ClaimTypes.Name).Value;

            if (decodedUserName is null)
                return null;

            var user = await _userManager.FindByNameAsync(decodedUserName);
            if (user is null)
                return null;

            var newToken = await GenerateJWTTokenAsync(user);
            var roles = await _userManager.GetRolesAsync(user);
            var userInfo = GenerateUserInfoObject(user, roles);

            await _logService.SaveNewLog(user.UserName, "New Token Generated");

            return new LoginServiceDto()
            {
                NewToken = newToken,
                UserInfo = userInfo
            };
        }


        public async Task<IEnumerable<UserInfoResult>> GetUsersListAsync()
        {
            var users = await _userManager.Users.ToListAsync();

            List<UserInfoResult> userInfoResults = new List<UserInfoResult>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                var userInfo = GenerateUserInfoObject(user, roles);

                userInfoResults.Add(userInfo);
            }

            return userInfoResults;
        }


        public async Task<UserInfoResult> GetUserDetailsByUserNameAsync(string userName)
        {
            var user = await _userManager.FindByNameAsync(userName);

            if (user is null)
                return null;

            var roles = await _userManager.GetRolesAsync(user);
            var userInfo = GenerateUserInfoObject(user, roles);

            return userInfo;
        }


        public async Task<IEnumerable<string>> GetUsernamesListAsync()
        {
            var userNames = await _userManager.Users
                .Select(q => q.UserName).ToListAsync();

            return userNames;
        }



        //Generate User Token

        private async Task<string> GenerateJWTTokenAsync(ApplicationUser user)
        {
            var userRoles = await _userManager.GetRolesAsync(user);

            var authClaims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.UserName),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim("FirstName", user.FirstName),
                new Claim("LastName", user.LastName),
            };

            foreach (var userRole in userRoles)
            {
                authClaims.Add(new Claim(ClaimTypes.Role, userRole));
            }

            var authSecret = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["JWT:Secret"]));

            var signingCredentials = new SigningCredentials(authSecret, SecurityAlgorithms.HmacSha256);

            var tokenObject = new JwtSecurityToken(
                issuer: _configuration["JWT:ValidIssuer"],
                audience: _configuration["JWT:ValidAudience"],
                notBefore: DateTime.Now,
                expires: DateTime.Now.AddHours(3),
                claims: authClaims,
                signingCredentials: signingCredentials
                );

            string token = new JwtSecurityTokenHandler().WriteToken(tokenObject);

            return token;
        }


        //generate user info
        private UserInfoResult GenerateUserInfoObject(ApplicationUser user, IEnumerable<string> Roles)
        {
            return new UserInfoResult()
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                UserName = user.UserName,
                Email = user.Email,
                CreatedAt = user.CreatedAt,
                Roles = Roles
            };
        }
    }
}

