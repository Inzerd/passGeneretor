using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace passgeneretor.Models
{
  static class WellKnow
  {
    private static StringBuilder _manual;

    public static string Manual
    {
      get
      {
        if (_manual == null || string.IsNullOrEmpty(_manual.ToString()))
        {
          _manual = new StringBuilder();
          var fileManual = File.ReadAllLines(Path.Combine(Environment.CurrentDirectory, "README.md"));
          foreach (var row in fileManual)
          {
            _manual.AppendLine(row.ReplaceLineEndings());
          }
          return _manual.ToString();
        }
        return _manual.ToString();
      }
    }
  }
}
