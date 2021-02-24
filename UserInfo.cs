using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace passgeneretor
{
	class UserInfo
	{
		public Dictionary<string, string> infoList { get; set; } = new Dictionary<string, string>();

		public UserInfo(string urlUserInfo)
		{
			if (!string.IsNullOrEmpty(urlUserInfo) && File.Exists(urlUserInfo))
			{
				var fileUserInfo = File.ReadAllLines(urlUserInfo);
				foreach (var row in fileUserInfo)
				{
					var info = row.Split(':');
					infoList[info[0].Trim()] = info[1].Trim();
				}
			}
		}

	}
}
