using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;

namespace passgeneretor
{
	class Program
	{
		private static bool useCombinatedString;
		private static bool useSpecialChar;
		private static int passwordLen;
		private static string urlToOutPut;
		private static string urlToUserInfo;
		private static UserInfo userInfo;
		private static PassComposer passComposer;
		static void Main(string[] args)
		{
			Console.WriteLine("\n\tWelcome On P4ssG3n3r3Th0r\n");
			Console.WriteLine("For read manual execute program with -h parameter");
			if (args.Count() <= 1 || args[0] == "-h")
			{
				Console.WriteLine(WellKnow.manual);
				return;
			}
			for (var iter = 0; iter < args.Count(); iter++)
			{
				switch (args[iter])
				{
					case "-h":
						Console.WriteLine("Can't call help menu with other param");
						return;
					case "-u":
						//mandatory
						iter++;
						urlToUserInfo = args[iter];
						if (!File.Exists(urlToUserInfo))
						{
							Console.WriteLine("the file " + urlToUserInfo + " not exist, please check the path!!");
							return;
						}
						break;
					case "-o":
						//mandatory
						iter++;
						var outputPath = args[iter];
						try
						{
							if (!File.Exists(outputPath))
							{
								File.Create(outputPath);
							}
							else
							{
								File.Open(outputPath, FileMode.Append);
							}
						}
						catch (Exception caught)
						{
							Console.WriteLine($"Error: -o param need correct url. Path insert: {args[iter + 1]}" +
								$"\nCheck if you have permision to read and write:\n{caught}");
							//terminare l'applicazione
						}
						urlToOutPut = args[iter++];
						break;
					case "-t":
						Console.WriteLine("Loading transformation information...");
						string transformCharFilePath = "";
						if (File.Exists(args[iter + 1]))
						{
							transformCharFilePath = args[iter++];
						}
						passComposer = new PassComposer(transformCharFilePath);
						break;
					case "-c":
						useCombinatedString = true;
						break;
					case "-s":
						useSpecialChar = true;
						break;
					case "-l":
						int.TryParse(args[iter++], out passwordLen);
						break;
				}
			}
			try
			{
				Console.WriteLine("Loading user Information...");
				userInfo = new UserInfo(urlToUserInfo);

			}
			catch
			{
				Console.WriteLine("user info file is not correct or not exist, please check file!!!");
				return;
			}
			foreach (var token in userInfo.infoList)
			{
				/*Cicla tutte le userInfo
			 per ogni token trovato richiama la funzione che combina una parola con altre "n" parole (parametro passato)
			 la funzione può effettuare:
					- trasformazioni: per ogni parola trovata restituisce l'originale e le sue trasformazioni
					- combinazioni: ogni parola viene divisa in sillabe e composta con le altre (anche ques'ultime eventualmente suddivide)
													questo serve per la creazione di password complesse
			*/
			}
		}
		private List<string> GetStringsTransformed(string token)
		{
			var res = new List<String>();
			foreach (var transf in passComposer.TranformationList)
			{
				whiel
			}
			return res;
		}
	}
}
