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
    public int MaxDegreeOfParallelism { get; set; }

    public int MaxOutputSize = 314572800;
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

    public int MaxDegreeOfParallelism
    {
      get
      {
        return PassComposerConf.MaxDegreeOfParallelism;
      }
    }
    public Regex PasswordRegex
    {
      get
      {
        var specialChar = @"<!@#$%^&€*()\-_+""\[\]\\=?{}ç@°#,;.:*€|<>\/";
        string compositionString = @$"a-zA-Z0-9{specialChar}";
        if (_regex != null)
        {
          return _regex;
        }
        var regexString = "^"; //start of line

        if (ruleCaseLetter)
        {
          // positivelockhead almost one of a-z and one of A-Z char
          regexString += "(?=.*[a-z])(?=.*[A-Z])";
          //compositionString = "a-zA-Z";
        }
        if (ruleUseNumber)
        {
          //positive lockehead almost one of digit
          regexString += "(?=.*[0-9])";
          //compositionString += "0-9";
        }
        if (ruleUseSpecialChar)
        {
          //positive lockhead almost one of the special char
          regexString += @$"(?=.*[{specialChar}])";
          //compositionString += $"{specialChar}";
        }
        if (!string.IsNullOrEmpty(compositionString))
        {
          //almost one of minLenght times
          regexString += $"[{compositionString}]";
        }
        regexString += $"{{{minLenght},}}$"; //add min lengh passowrd check and close line
        _regex = new Regex(regexString);
        return _regex;
      }
    }

    public string OutputFilePath
    {
      get
      {
        return _outputFilePath;
      }
      set
      {
        _outputFilePath = value;
        if (string.IsNullOrEmpty(_outputFileName))
        {
          _outputFileName = Path.GetFileNameWithoutExtension(OutputFilePath);
        }
        if (string.IsNullOrEmpty(_outputPath))
        {
          _outputPath = Path.GetDirectoryName(OutputFilePath);
        }
      }
    }
    #endregion

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
        Console.WriteLine($"{DateTime.Now}: ERROR: -t param need correct Path. Path insert: {urlFileTransform}");
        //chiudi applicazione
      }
      else
      {
        PassComposerConf ??= new PassComposerConfiguration
        {
          DelimitatorList = GetStandardDeliminatorList(),
          MaxNumberOfComposition = 3
        };

        PassComposerConf.TranformationList = new Dictionary<string, List<string>>();
        var charToTrasformList = File.ReadAllLines(urlFileTransform);

        foreach (var trasformation in charToTrasformList)
        {
          if (trasformation.Contains('='))
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


    #region public methods
    public void WritePassword(List<string> passwordList)
    {
      if (!passwordList.Any())
      {
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
        Console.WriteLine($"{DateTime.Now}: ERROR: during write output file: {caught}");
      }
    }

    public bool CheckValidityPassword(string passowordToCheck)
    {
      return PasswordRegex.IsMatch(passowordToCheck);
      /* if (!result)
      {
        Console.WriteLine($"{DateTime.Now}: Password: {passowordToCheck}, not pass Regex check");
      }
      return result; */
    }
    #endregion

    #region Private Function
    private static List<string> GetStandardDeliminatorList()
    {
      return new List<string> { "_", "-" };
    }

    private List<string> GetStandardSpecialChar()
    {
      return new List<string> {"!",@"\", @"""","£","$","%","&","/",
      "(",")","=","?",@"\]","^", @"\[","{","}","ç","@","°","#",",",
      ";",".",":",@"\-","_","*","€","|","<",">",};
    }

    #endregion
  }
}
