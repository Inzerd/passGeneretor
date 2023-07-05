using System;
using System.IO;
using System.Linq;
using passgeneretor.trasformation;
using passgeneretor.Models;
using System.Threading;
using System.Collections.Generic;

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
      if (args.Count() == 0 || (args.Count() == 1 && args[0] == "-h"))
      {
        Console.WriteLine(WellKnow.Manual);
        return;
      }
      //check mandatory paramenter
      CheckAndSetmandatoryParamenter(args);
      //save parameter			
      CheckAndSetOtherParamenter(args);
      #endregion

      #region start the magic

      var permutationDictionary = new Dictionary<string, List<string>>();

      if (!(passComposer.permutation && passComposer.combination))
      {
        Console.WriteLine("check whether the list of term passed meets the password configuration");
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
          Console.WriteLine($"{DateTime.Now.ToString()}: Create a complete list of permutation");
          permutationDictionary = Transformation.GetPermutationDictionary(userInfo.infoList, passComposer);
          totalList = Transformation.NumberOfElementInDictionary;

        }

        Console.WriteLine($"{DateTime.Now.ToString()}: List of permutation completed");

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
            var userChoice = Console.ReadLine();
            if (userChoice.ToLower() == "y")
            {
              Console.WriteLine($"Go to make a coffee human, and one for me also, the combinations are coming...");
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
            Console.WriteLine($"Start to create combination and write in one or more file.");
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
      Console.WriteLine($"{DateTime.Now.ToString()}: End!!!! maybe XD");
      Thread.Sleep(2000);
      Console.WriteLine($"{DateTime.Now.ToString()}: OK its a joke this is the END!!!");
      #endregion

    }

    private static int SetDictionaryWithoutPermutation(Dictionary<string, List<string>> permutationDictionary)
    {
      int totalList;
      userInfo.infoList.ForEach(info => permutationDictionary.Add(info, new List<string>() { info }));
      totalList = userInfo.infoList.Count();
      return totalList;
    }

    private static void CheckAndSetmandatoryParamenter(string[] args)
    {
      try
      {
        //user file information to use for permutation and trasformation
        if (!args.Contains("-u"))
        {
          Console.WriteLine("Cannot execute program without user information list!");
          return;
        }
        var currentIndex = Array.IndexOf(args, "-u");
        urlToUserInfo = args[++currentIndex];
        Console.WriteLine("Loading user Information...");
        if (string.IsNullOrEmpty(urlToUserInfo) || !File.Exists(urlToUserInfo))
        {
          Console.WriteLine($"the file: \"{urlToUserInfo}\"  not exist, please check the path!!");
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
        if (passComposer == null)
        {
          passComposer = new PassComposerManager();
        }

        passComposer.OutputFilePath = outputPath;
        currentIndex = Array.IndexOf(args, "-l");
        if (currentIndex <= -1)
        {
          throw new ArgumentNullException("-l lenght paramenter is mandatory please added it");
        }
        int.TryParse(args[++currentIndex], out passComposer.minLenght);
      }
      catch (Exception caught)
      {
#if DEBUG
        Console.WriteLine($"Error: {caught}");
#else
        Console.WriteLine($"Ops!!!, somethis its go wrong!!! sorry XD");        
#endif
        throw new Exception();
      }
    }

    private static void CheckAndSetOtherParamenter(string[] args)
    {
      for (var iter = 0; iter < args.Count(); iter++)
      {
        switch (args[iter])
        {
          case "-rule":
            //set rule for validation password, if there aren't rule explained use only minLenght
            iter++;
            var rule = args[iter];
            passComposer.ruleCaseLetter = rule.Contains("c");
            passComposer.ruleUseNumber = rule.Contains("n");
            passComposer.ruleCaseLetter = rule.Contains("s");
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
