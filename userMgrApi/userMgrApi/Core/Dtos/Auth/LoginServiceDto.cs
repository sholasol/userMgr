using System;
namespace userMgrApi.Core.Dtos.Auth
{
	public class LoginServiceDto
	{
		public object NewToken { get; set; }

		//get info to return to the frontend from UserInfoResult
		public UserInfoResult UserInfo { get; set; }
	}
}

