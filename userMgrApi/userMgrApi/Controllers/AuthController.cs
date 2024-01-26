using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using userMgrApi.Core.Constants;
using userMgrApi.Core.Dtos.Auth;
using userMgrApi.Core.Interfaces;

namespace userMgrApi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]

	public class AuthController : ControllerBase
	{
		private readonly IAuthService _authService;

		public AuthController(IAuthService authService)
		{
			_authService = authService;
		}

		//seed roles
		[HttpPost]
		[Route("seed-roles")]
		public async Task<IActionResult> SeedRoles()
		{
			var seedResult = await _authService.SeedRoleAsync();
			return StatusCode(seedResult.StatusCode, seedResult.Message);
		}

		//registration
		[HttpPost]
		[Route("register")]
		public async Task<IActionResult> Register([FromBody] RegisterDto registerDto)
		{
			var registerResult = await _authService.RegisterAsync(registerDto);
			return StatusCode(registerResult.StatusCode, registerResult.Message);
		}

		//login
		[HttpPost]
		[Route("login")]
		public async Task<ActionResult<LoginServiceDto>> Login([FromBody] LoginDto loginDto)
		{
			var loginResult = await _authService.LoginAsync(loginDto);
			if(loginResult is null)
			{
				return Unauthorized("Invalid credentials");
			}

			return Ok(loginResult);
		}

		//update role
		//owner can change everything
		[HttpPost]
		[Route("update-role")]
		[Authorize(Roles = StaticUserRoles.OwnerAdmin)]
		public async Task<IActionResult> UpdateRole([FromBody] UpdateRoleDto updateRoleDto)
		{
			var updateRoleResult = await _authService.UpdateRoleAsync(User, updateRoleDto);

			if(updateRoleResult.isSucceed)
			{
				return Ok(updateRoleResult.Message);
			}
			else
			{
				return StatusCode(updateRoleResult.StatusCode, updateRoleResult.Message);

			}
		}

		//get user record
		[HttpPost]
		[Route("me")]
		public async Task<ActionResult<LoginServiceDto>> Me([FromBody] MeDto token)
		{
			try
			{
				var me = await _authService.MeAsync(token);
				if(me is not null)
				{
					return Ok(me);
				}
				else
				{
					return Unauthorized("Ivalid user token");
				}
			}
			catch (Exception)
			{
                return Unauthorized("Ivalid user token");
            }
		}


		//list all users



	}
}

