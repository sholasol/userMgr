﻿using System;
namespace userMgrApi.Core.Dtos.Message
{
	public class CreateMessageDto
	{
		public string ReceiverUserName { get; set; }

		public string Text { get; set; }
	}
}

