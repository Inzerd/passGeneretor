using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace passgeneretor
{
	class PassComposer
	{
		#region member and construct
		public List<(string, string)> TranformationList { get; set; }

		public PassComposer(string urlFileTransform)
		{
			if (!File.Exists(urlFileTransform))
			{
				urlFileTransform = Path.Combine(Directory.GetCurrentDirectory(), "config\\charToTransform.txt");
			}
			var fileTransform = File.ReadAllLines(urlFileTransform);
			foreach (var row in fileTransform)
			{
				var splitRow = row.Split(';');
				TranformationList.Add((splitRow[0], splitRow[1]));
			}
		}
		#endregion

		#region public Function
		public string TransNumber(string strToTrans)
		{
			return null;
		}
		#endregion

		#region Privete Function
		#endregion
	}
}
