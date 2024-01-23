using System;
using System.Security.Claims;
using userMgrApi.Core.DbContext;
using userMgrApi.Core.Dtos.Log;
using userMgrApi.Core.Interfaces;

namespace userMgrApi.Core.Services
{
	public class LogService : ILogService
	{
        private readonly ApplicationDbContext _context;
		public LogService(ApplicationDbContext context)
		{
            _context = context;
		}

        public Task SaveNewLog(string UserName, string Description)
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<GetLogDto>> GetLogsAsync()
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<GetLogDto>> GetMyLogsAsync(ClaimsPrincipal User)
        {
            throw new NotImplementedException();
        }
    }
}

