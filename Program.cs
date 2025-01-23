using System;
using System.IO;
using System.Linq;
using passgeneretor.trasformation;
using passgeneretor.Models;
using System.Threading;
using System.Collections.Generic;
using System.Runtime.InteropServices;

namespace passgeneretor
{
  class Program
  {
    private static string urlToUserInfo;
    private static UserInfo userInfo;
    private static PassComposerManager passComposer;
    static void Main(string[] args)
    {
      Console.WriteLine("\n\tWelcome On P4ssG3n3r3Th0r\n");
      Console.WriteLine("To read manual execute program with -h parameter");

      #region read param
      if (args.Length == 0 || (args.Length == 1 && args[0] == "-h"))
      {
        Console.WriteLine(WellKnow.Manual);
        return;
      }

      Console.WriteLine($"{DateTime.Now}: Reading arguments");
      //check mandatory paramenter
      CheckAndSetmandatoryParamenter(args);
      //save parameter			
      CheckAndSetOtherParamenter(args);
      #endregion

      #region start the magic

      var permutationDictionary = new Dictionary<string, List<string>>();

      if (!(passComposer.permutation && passComposer.combination))
      {
        Console.WriteLine($"{DateTime.Now}: check whether the list of term passed meets the password configuration");
        _ = SetDictionaryWithoutPermutation(permutationDictionary);

        foreach (var key in permutationDictionary.Keys)
        {
          passComposer.WritePassword(permutationDictionary[key]);
        }
      }
      else
      {
        int totalList = 0;
        if (passComposer.permutation)
        {
          Console.WriteLine($"{DateTime.Now}: Create a complete list of permutation");
          permutationDictionary = Transformation.GetPermutationDictionary(userInfo.infoList, passComposer);
          totalList = Transformation.NumberOfElementInDictionary;
        }

        Console.WriteLine($"{DateTime.Now}: List of permutation completed");

        if (passComposer.combination)
        {
          if (!passComposer.permutation)
          {
            //if user cannot select permutation option permutationDictionary is empty, 
            //add one key with only one value for any records in user info list and start combinations
            totalList = SetDictionaryWithoutPermutation(permutationDictionary);
          }
          //to be defined: i see that when there are more record inside permutation dictionary the 
          //combination execution time are more expansive, two solution:
          // - try to execute more task in parallel mode to reduce time to execution
          // - inform user for more time.
          if (totalList > 10000)
          {
            Console.WriteLine($"The number of permutation is very high, are you sure you want start terms combinations?");
            Console.WriteLine($"Y/y: to start combination - N/n: to not start combination");
            var userChoice = (char)Console.Read();
            if (userChoice.ToString().ToLower() == "y")
            {
              Transformation.CreateAndWriteListOfCombination(permutationDictionary, passComposer, 0);
            }
            else
            {
              Console.WriteLine($"Good choice, start to write output file with permutation generated");
              foreach (var key in permutationDictionary.Keys)
              {
                passComposer.WritePassword(permutationDictionary[key]);
              }
            }
          }
          else
          {
            Console.WriteLine($"{DateTime.Now}: Start to create combination and write in one or more file.");
            Console.WriteLine("...");
            Console.WriteLine($"Go to make a coffee human, and one for me also, the combinations are coming...");
            Transformation.CreateAndWriteListOfCombination(permutationDictionary, passComposer, 0);
          }
        }
        else
        {
          foreach (var key in permutationDictionary.Keys)
          {
            passComposer.WritePassword(permutationDictionary[key]);
          }
        }
      }
      Console.WriteLine($"{DateTime.Now}: End!!!! maybe XD");
      Thread.Sleep(2000);
      Console.WriteLine($"{DateTime.Now}: OK its a joke this is the END!!!");
      #endregion
    }

    private static int SetDictionaryWithoutPermutation(Dictionary<string, List<string>> permutationDictionary)
    {
      int totalList;
      userInfo.infoList.ForEach(info => permutationDictionary.Add(info, new List<string>() { info }));
      totalList = userInfo.infoList.Count;
      return totalList;
    }

    private static void CheckAndSetmandatoryParamenter(string[] args)
    {
      try
      {
        //user file information to use for permutation and trasformation
        if (!args.Contains("-u"))
        {
          Console.WriteLine($"{DateTime.Now}: Error: Cannot execute program without user information list!");
          return;
        }
        var currentIndex = Array.IndexOf(args, "-u");
        urlToUserInfo = args[++currentIndex];
        Console.WriteLine("Loading user Information...");
        if (string.IsNullOrEmpty(urlToUserInfo) || !File.Exists(urlToUserInfo))
        {
          Console.WriteLine($"{DateTime.Now}: Error: the file: \"{urlToUserInfo}\"  not exist, please check the path!!");
          return;
        }
        userInfo = new UserInfo(urlToUserInfo);

        //output path
        var outputPath = "";
        currentIndex = Array.IndexOf(args, "-o");
        if (currentIndex <= -1)
        {
          outputPath = Path.Combine(Directory.GetCurrentDirectory(), "passGeneraThor_output.txt");
        }
        else
        {
          outputPath = args[++currentIndex];
        }

        //if there aren't any PassComposer setted from user, set 
        passComposer ??= new PassComposerManager();

        passComposer.OutputFilePath = outputPath;
        currentIndex = Array.IndexOf(args, "-l");
        if (currentIndex <= -1)
        {
          throw new ArgumentNullException("password lenght", "-l lenght paramenter is mandatory please added it");
        }
        _ = int.TryParse(args[++currentIndex], out passComposer.minLenght);
      }
      catch 
      {
        Console.WriteLine($"Ops!!!, somethis its go wrong!!! sorry XD");
        throw;
      }
    }

    private static void CheckAndSetOtherParamenter(string[] args)
    {
      for (var iter = 0; iter < args.Length; iter++)
      {
        switch (args[iter])
        {
          case "-rule":
            //set rule for validation password, if there aren't rule explained use only minLenght
            iter++;
            var rule = args[iter];
            passComposer.ruleCaseLetter = rule.Contains('c');
            passComposer.ruleUseNumber = rule.Contains('n');
            passComposer.ruleUseSpecialChar = rule.Contains('s');
            break;
          case "-t":
            Console.WriteLine("Loading transformation information...");
            iter++;
            if (File.Exists(args[iter]))
            {
              passComposer.SetConfiguration(args[iter]);
            }
            break;
          case "-c":
            passComposer.combination = true;
            break;
          case "-p":
            passComposer.permutation = true;
            break;
        }
      }
    }
  }
}
