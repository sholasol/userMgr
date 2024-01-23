﻿using System;
using System.Security.Claims;
using userMgrApi.Core.Dtos.Log;

namespace userMgrApi.Core.Interfaces
{
	public interface ILogService
	{
		Task SaveNewLog(string UserName, string Description);

		Task<IEnumerable<GetLogDto>> GetLogsAsync();

		Task<IEnumerable<GetLogDto>> GetMyLogsAsync(ClaimsPrincipal User);
	}
}

 