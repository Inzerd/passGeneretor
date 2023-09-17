using System;
using System.Collections.Generic;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using System.Linq;
using System.Text;

namespace passgeneretor.trasformation
{
  public static class Transformation
  {
    private static int _numberOfElementInDictionary = 0;

    //private static bool printProgess = true;
    #region permutation methods
    public static int NumberOfElementInDictionary
    {
      get
      {
        return _numberOfElementInDictionary;
      }
    }

    /// <summary>
    /// Gived a list of terms return a dictionry with key the terms records and value a list 
    /// of any term permutation with configuration file rule
    /// </summary>
    /// <param name="terms">list of user info</param>
    /// <param name="passComposer"></param>
    /// <param name="numberOfElementInDictionary">total count for any list inside dictionary</param>
    /// <returns></returns>
    public static Dictionary<string, List<string>> GetPermutationDictionary(List<string> terms, PassComposerManager passComposer)
    {
      _numberOfElementInDictionary = 0;
      var result = new ConcurrentDictionary<string, List<string>>();
      Parallel.ForEach(terms,
      new ParallelOptions { MaxDegreeOfParallelism = passComposer.MaxDegreeOfParallelism },
      (termToPermutation) =>
      {
        var value = GetListPermutationed(new List<string>() { termToPermutation }, passComposer);

        if (value.Any())
        {
          _numberOfElementInDictionary += value.Count;
          result.TryAdd(termToPermutation, value.Distinct().ToList());
        }
      });
      return result.ToDictionary(item => item.Key,
      item => item.Value,
      result.Comparer);
    }

    /// <summary>
    /// gived a list of terms and passcomposer object return all possible 
    /// </summary>
    /// <param name="terms">list of term to permutation</param>
    /// <param name="passComposer">object that contain a dictionary to key and value for permutation action</param>
    /// <param name="addTermNotPermuted">allow to add term not permuted in result list</param>
    /// <returns>return the list of permutation</returns>
    private static List<string> GetListPermutationed(List<string> terms,
    PassComposerManager passComposer, bool addTermNotPermuted = true)
    {
      var results = new List<string>();
      foreach (var term in terms)
      {
        if (addTermNotPermuted)
        {
          results.Add(term);
        }
        var partialResult = new List<string>();
        foreach (var substitute in passComposer.TransformationKeyList)
        {
          //for any term find in item any matchecs and save position 
          var arrayOfTermMatch = new List<int>();
          for (int index = 0; index <= term.Length; index++)
          {
            index = term.IndexOf(substitute, index);
            //first round or next cannot found any matches exit from this cycle
            if (index == -1)
            {
              break;
            }
            arrayOfTermMatch.Add(index);
          }
          //if there aren't match break from foreach and pass to another term
          if (!arrayOfTermMatch.Any())
          {
            continue;
          }

          //there are almost one char to permutation
          var listOfPermutation = passComposer.TrasformationValueList(substitute);
          foreach (var permutation in listOfPermutation)
          {
            //permuted only one char
            var stringBuilder = new StringBuilder(term);
            foreach (var index in arrayOfTermMatch)
            {
              var stringBuilded = stringBuilder.Replace(substitute, permutation, index, 1).ToString();
              if (!partialResult.Contains(stringBuilded))
              {
                partialResult.Add(stringBuilded);
              }
              stringBuilder.Clear();
              stringBuilder.Append(term);
            }

            //permuted all char
            var allPermutationString = stringBuilder.Replace(substitute, permutation).ToString();
            if (!partialResult.Contains(allPermutationString))
            {
              partialResult.Add(allPermutationString);
            }
          }
        }
        if (partialResult.Any())
        {
          results.AddRange(partialResult);
          results.AddRange(GetListPermutationed(partialResult, passComposer, false));
        }
      }
      return results;
    }

    #endregion

    #region combination methods    
    /// <summary>
    /// GIved a dictionary key: string, value: list of string  combination any term in dictionary and write in output file the 
    /// password. 
    /// Il metodo cicla tutte le chiavi del dizionario, 
    /// per ogni ciclo di chiave esegue un altro ciclo per tutti i termini della lista di stringhe, salvando di volta in volta i termini
    /// in previousLIst,
    /// Una volta raggiunta la condizione maxNumberOfCombination, combina i termini della previousList e scrive le password che
    /// rispettano le regole impostate
    /// </summary>
    /// <param name="permutation"></param>
    /// <param name="passComposer"></param>
    /// <param name="startIndex"></param>
    /// <param name="previousList"></param>
    /// <returns></returns>
    public static void CreateAndWriteListOfCombination(Dictionary<string, List<string>> permutation,
    PassComposerManager passComposer, int startIndex = 0, List<string> previousList = null)
    {
      var keyDictionaryCount = permutation.Keys.Count;
      var maxNumberOfCombination = passComposer.MaxNumberOfComposition;

      //foreach any dictionary key dictionary and start create the list of term to pass a composition function
      for (var iter = startIndex; iter < keyDictionaryCount; iter++)
      {
        //if there is main cycle skip, the combination are alredy inside output.
        if (startIndex == 0 && iter == keyDictionaryCount - maxNumberOfCombination + 1)
        {
          continue;
        }
        //else there aren't in main foreach add previus terms fixxed         
        var CombinationList = new List<string>();
        if (previousList != null)
        {
          CombinationList.AddRange(previousList);
        }

        //get the terms list associated to the key selected
        var FixxedDictKey = permutation.Keys.ToArray()[iter];
        if (startIndex == 0)
        {
          Console.WriteLine("");
          Console.WriteLine($"{DateTime.Now}: start to compose and write any password for term: {FixxedDictKey}:");
        }
        if (!permutation.TryGetValue(FixxedDictKey, out var listOfValuesByKey))
        {
          Console.WriteLine($"{DateTime.Now}: Error during processing, for term \"{FixxedDictKey}\", list of permutation for create combination list; this term will skipped");
        }

        //foreach the list and check if start combination and write password procedure
        foreach (var value in listOfValuesByKey)
        {
          if (CombinationList.Count == maxNumberOfCombination - 1)
          {
            //if condition maxNumberOfCombination is valid start combination and write password procedure
            var tempList = new List<string>(CombinationList)
            {
              value
            };
            Console.Write(new char[' '], 0, 100);
            //Console.Write($"\r==>start to compose all combination with terms combination: {tempList[0]}");
            Console.Write($"\r\t==>start to check and write terms combination: [{string.Join(",", tempList)}]");
            var partialCombinationList = GetCombinationParallel(tempList,
              0,
              CombinationList.Count,
              passComposer,
              true);
            if (partialCombinationList.Any())
            {
              passComposer.WritePassword(partialCombinationList);
            }
          }
          else
          {
            //else save the list 
            var tempList = new List<string>(CombinationList)
            {
              value
            };
            CreateAndWriteListOfCombination(permutation, passComposer, iter + 1, tempList);
          }
        }
      }
    }

    /// <summary>
    /// compose all combinations from all term in list terms, using all deliminator specify in passComposer
    /// return a list of password filter from parameter passed by user 
    /// </summary>
    /// <param name="terms">list fo term to combinate</param>
    /// <param name="startIndex">index to start combination</param>
    /// <param name="endIndex">last index for terms</param>
    /// <param name="passComposer">configuration of pass composer object</param>
    /// <returns>return a list of password filterd by validity check</returns>
    /// <exception cref="ArgumentException"></exception>
    private static List<string> GetCombinationParallel(List<string> terms,
    int startIndex,
    int endIndex,
    PassComposerManager passComposer,
    bool enableParallelization = false)
    {
      //var result = new List<string>();
      var result = new ConcurrentBag<string>();
      if (!terms.Any())
      {
        throw new ArgumentException($"{nameof(GetCombinationParallel)}: terms is empty");
      }
      //there are only tow term in list to combine, return partial list
      if (terms.Count == 2)
      {
        return GetCombination(terms[0], terms[1], passComposer.GetPassDelimitator);
      }

      //foreach any terms and fix one term and swap other,   
      if (enableParallelization)
      {
        Parallel.For(startIndex, endIndex + 1,
        new ParallelOptions { MaxDegreeOfParallelism = passComposer.MaxDegreeOfParallelism },
        (iterFixxed) =>
        {
          GetCombination(terms, endIndex, passComposer, iterFixxed, result);
        });
      }
      else
      {
        for (var iterFixxed = startIndex; iterFixxed <= endIndex; iterFixxed++)
        {
          GetCombination(terms, endIndex, passComposer, iterFixxed, result);
        }
      }
      return result.Where(password => passComposer.CheckValidityPassword(password)).ToList();
    }

    private static void GetCombination(
      List<string> terms,
      int endIndex,
      PassComposerManager passComposer,
      int iterFixxed,
      ConcurrentBag<string> result)
    {
      var termsFixxed = terms[iterFixxed];
      var cloneList = terms.GetRange(0, terms.Count);
      cloneList.RemoveAt(iterFixxed);
      var partialResult = GetCombinationParallel(cloneList, iterFixxed + 1, endIndex, passComposer);
      foreach (var partTerms in partialResult)
      {
        result.Add($"{termsFixxed}{partTerms}");
        foreach (var delimitator in passComposer.GetPassDelimitator)
        {
          result.Add($"{termsFixxed}{delimitator}{partTerms}");
        }
      }
    }

    /// <summary>
    /// gived two terms compose all combination with delimitator in pass configuration object
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name="passDelimitator"></param>
    /// <returns></returns>
    private static List<string> GetCombination(string a, string b, List<string> passDelimitator)
    {
      var result = new List<string>() { a + b, b + a };
      foreach (var delimitator in passDelimitator)
      {
        result.AddRange(new string[] { $"{a}{delimitator}{b}", $"{b}{delimitator}{a}" });
      }
      return result;
    }

  }
  #endregion
}