using System;
namespace userMgrApi.Core.Dtos.General
{
	public class GeneralServiceResponseDto
	{
		public bool isSuccess { get; set; }

		public int StatusCode { get; set; }

		public string Message { get; set; }
	}
}

