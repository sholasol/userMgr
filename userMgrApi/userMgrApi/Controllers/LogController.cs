using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using userMgrApi.Core.Constants;
using userMgrApi.Core.Dtos.Log;
using userMgrApi.Core.Interfaces;

namespace userMgrApi.Controllers
{
	[Route("api/[controller]")]
	[ApiController]

	public class LogController : ControllerBase
	{
		private readonly ILogService _logService;

		public LogController(ILogService logService)
		{
			_logService = logService;
		}

		[HttpGet]
		[Authorize(Roles = StaticUserRoles.OwnerAdmin)]
		public async Task<ActionResult<IEnumerable<GetLogDto>>> GetLogs()
		{
			var logs = await _logService.GetLogsAsync();
			return Ok(logs);
		}

		[HttpGet]
		[Route("mine")]
		[Authorize]
		public async Task<ActionResult<IEnumerable<GetLogDto>>> GetMyLogs()
		{
			var logs = await _logService.GetMyLogsAsync(User);
			return Ok(logs);
		}

	}
}

