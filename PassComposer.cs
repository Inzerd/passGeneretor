using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
namespace passgeneretor
{
  public class PassComposerConfiguration
  {
    //dictionary to manage trasformation from char to many other char example: a-> 4
    public Dictionary<string, List<string>> TranformationList { get; set; }
    // list of string delimitator to use to composePassword
    public List<string> DelimitatorList { get; set; }
    //max number of Combinations 
    public int MaxNumberOfComposition { get; set; }

    public int MaxOutputSize = 31457280;
  }

  public class PassComposerManager
  {
    #region member and construct		
    private PassComposerConfiguration PassComposerConf;

    private Regex _regex = null;

    //next development: public bool syllabe;
    public bool combination;
    public bool permutation;
    public int minLenght;
    public bool ruleCaseLetter;
    public bool ruleUseNumber;
    public bool ruleUseSpecialChar;
    private string _outputFilePath;
    private string _outputFileName;
    private string _outputPath;
    private int fileSuffixNumer = 0;

    public string OutputFilePath
    {
      get
      {
        return _outputFilePath;
      }
      set
      {
        _outputFilePath = value;
        _outputFileName = Path.GetFileNameWithoutExtension(OutputFilePath);
        _outputPath = Path.GetDirectoryName(OutputFilePath);
      }
    }
    #endregion

    #region ctor
    public PassComposerManager()
    {
      var urlFileTransform = Path.Combine(Directory.GetCurrentDirectory(), "config\\passComposerConfiguration.json");
      var jsonString = File.ReadAllText(urlFileTransform);
      PassComposerConf = JsonSerializer.Deserialize<PassComposerConfiguration>(jsonString);
    }
    public void SetConfiguration(string urlFileTransform)
    {
      if (!File.Exists(urlFileTransform))
      {
        Console.WriteLine($"Error: -t param need correct Path. Path insert: {urlFileTransform}");
        //chiudi applicazione
      }
      else
      {
        if (PassComposerConf == null)
        {
          PassComposerConf = new PassComposerConfiguration();
          PassComposerConf.DelimitatorList = GetStandardDeliminatorList();
          PassComposerConf.MaxNumberOfComposition = 3;
        }

        PassComposerConf.TranformationList = new Dictionary<string, List<string>>();
        var charToTrasformList = File.ReadAllLines(urlFileTransform);

        foreach (var trasformation in charToTrasformList)
        {
          if (trasformation.Contains("="))
          {
            var transform = trasformation.Split("=");
            var key = transform[0];
            var values = new List<string>(transform[1].Split(','));
            if (!PassComposerConf.TranformationList.ContainsKey(key))
            {
              PassComposerConf.TranformationList.Add(key, values);
            }
            values.ForEach(value =>
            {
              if (!PassComposerConf.TranformationList[key].Contains(value))
              {
                PassComposerConf.TranformationList[key].Add(value);
              }
            });
          }
        }
      }
    }
    #endregion

    #region property
    public List<string> GetPassDelimitator
    {
      get
      {
        if (PassComposerConf != null)
        {
          return PassComposerConf.DelimitatorList;
        }
        return null;
      }
    }

    public IEnumerable<string> TransformationKeyList
    {
      get
      {
        if (PassComposerConf != null)
        {
          return PassComposerConf.TranformationList.Keys;
        }
        return null;
      }
    }

    public IEnumerable<string> TrasformationValueList(string key)
    {
      if (PassComposerConf?.TranformationList != null)
      {
        return PassComposerConf.TranformationList[key];
      }
      return null;
    }

    public int MaxNumberOfComposition
    {
      get
      {
        return PassComposerConf.MaxNumberOfComposition;
      }
    }

    public Regex PasswordRegex
    {
      get
      {
        if (_regex != null)
        {
          return _regex;
        }
        var regexString = @$"^"; //start of line
        string compositionString = string.Empty;
        if (ruleCaseLetter)
        {
          // positivelockhead almost one of a-z and one of A-Z char
          regexString += @"(?=.*[a-z])(?=.*[A-Z])";
          compositionString = "a-zA-Z";
        }
        if (ruleUseNumber)
        {
          //positive lockehead almost one of digit
          regexString += @"(?=.*\d)";
          compositionString += @"\d";
        }
        if (ruleUseSpecialChar)
        {
          //positive lockhead almost one of the special char
          var specialChar = string.Empty;
          GetStandardSpecialChar().ForEach(delegate (string term) { specialChar += @$"{term}"; });
          regexString += @$"(?=.[{specialChar}])";
          compositionString += $"{specialChar}";
        }
        if (!string.IsNullOrEmpty(compositionString))
        {
          //almost one of minLenght times
          regexString += $"[{compositionString}]";
        }
        regexString += @$"{{{minLenght},}}"; //add min lengh passowrd check and close line
        _regex = new Regex(regexString);
        return _regex;
      }
    }
    #endregion

    #region public methods
    public void WritePassword(List<string> passwordList)
    {
      if (!passwordList.Any())
      {
        Console.WriteLine("cannot write empty password list in output file");
        return;
      }
      try
      {
        if (!File.Exists(OutputFilePath))
        {
          File.WriteAllLines(OutputFilePath, passwordList);
        }
        else
        {
          var outputFileInfo = new FileInfo(OutputFilePath);
          if (outputFileInfo.Length >= PassComposerConf.MaxOutputSize)
          {
            fileSuffixNumer++;
            OutputFilePath = Path.Combine(_outputPath, $"{_outputFileName}_{fileSuffixNumer}.txt");
            File.WriteAllLines(OutputFilePath, passwordList);
          }
          else
          {
            File.AppendAllLines(OutputFilePath, passwordList);
          }

        }
      }
      catch (Exception caught)
      {
        Console.WriteLine($"ERROR: during write output file: {caught}");
      }
    }

    public bool CheckValidityPassword(string passowordToCheck)
    {
      var result = PasswordRegex.IsMatch(passowordToCheck);
      if (!result)
      {
        Console.WriteLine($"Password: {passowordToCheck}, not pass Regex check");
      }
      return result;
    }
    #endregion

    #region Private Function
    private List<string> GetStandardDeliminatorList()
    {
      return new List<string> { "_", "-" };
    }

    private List<string> GetStandardSpecialChar()
    {
      return new List<string> {"!","\\","\"","£","$","%","&","/",
      "(",")","=","?","^","[","]","{","}","ç","@","°","#",",",
      ";",".",":","-","_","*","€","|","<",">",};
    }

    #endregion
  }
}
