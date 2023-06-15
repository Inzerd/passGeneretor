using System;
using System.Collections.Generic;
using System.IO;

namespace passgeneretor.Models
{
  class UserInfo
  {
    public List<string> infoList { get; set; } = new List<string>();

    public UserInfo(string urlUserInfo)
    {
      if (!string.IsNullOrEmpty(urlUserInfo) && File.Exists(urlUserInfo))
      {
        var fileUserInfo = File.ReadAllLines(urlUserInfo);
        foreach (var row in fileUserInfo)
        {
          infoList.Add(row.ReplaceLineEndings());
        }
      }
    }
  }
}
