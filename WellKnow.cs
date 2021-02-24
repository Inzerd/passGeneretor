using System;
using System.Collections.Generic;
using System.Text;

namespace passgeneretor
{
	static class WellKnow
	{
		public static string manual = $"\n\t\tManual to execute program:\n" +
			$"\nMandatory:" +
			$"\n\t-u[url]: file info user mandatory" +
			$"\n\t-o[url]: output file" +
			$"\nOptions:" +
			$"\n\t-l [lenght]: password's lenght" +
			$"\n\t-t[url]: file with transformation from char/ string to number/ special char/ string or another transformation string" +
			$"\n\t-c combined: combined any trasfomartion will creating more pass complex" +
			$"\n\t-s specialChar: convert same char in special char and insert special char how delimitator" +
			$"\n\t-h manual: show manual in console";
	}
}
