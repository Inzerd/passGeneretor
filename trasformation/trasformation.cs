using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace passgeneretor.trasformation
{
  public static class Transformation
  {
    #region permutation methods
    public static Dictionary<string, List<string>> GetPermutationDictionary(List<string> terms, PassComposerManager passComposer, out int numberOfElementInDictionary)
    {
      numberOfElementInDictionary = 0;
      var result = new Dictionary<string, List<string>>();
      foreach (var termsToPermutation in terms)
      {
        var value = GetListPermutationed(new List<string>() { termsToPermutation }, passComposer);

        if (value.Any())
        {
          numberOfElementInDictionary += value.Count();
          result.Add(termsToPermutation, value.Distinct().ToList());
        }
      }
      return result;
    }

    /// <summary>
    /// gived a list of terms and passcomposer object return all possible 
    /// </summary>
    /// <param name="terms">list of term to permutation</param>
    /// <param name="passComposer">object that contain a dictionary to key and value for permutation action</param>
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
    /// GIved a dictionary key: string value list o string combination any term in dictionary and write in output file the 
    /// password
    /// </summary>
    /// <param name="permutation"></param>
    /// <param name="passComposer"></param>
    /// <param name="startIndex"></param>
    /// <param name="previousList"></param>
    /// <returns></returns>
    public static void WriteListOfCombination(Dictionary<string, List<string>> permutation,
    PassComposerManager passComposer, int startIndex = 0, List<string> previousList = null)
    {
      var keyDictionaryCount = permutation.Keys.Count();
      var maxNumberOfCombination = passComposer.MaxNumberOfComposition;
      //cicliamo le chiavi del dizionario e iniziamo a comporre le liste da passare alla funzione che genera le composizioni 
      //ad ogni ciclo viene passata la lista al passComposer che le scrive nel file

      for (var iter = startIndex; iter < keyDictionaryCount; iter++)
      {
        //if there is main cycle skip, the combination are alredy inside output.
        if (startIndex == 0 && iter == keyDictionaryCount - maxNumberOfCombination + 1)
        {
          continue;
        }
        //non siamo nella prima chiave del dizionatio quindi aggiungiamo i termini fixxati precedentemente 
        var CombinationList = new List<string>();
        if (previousList != null)
        {
          CombinationList.AddRange(previousList);
        }

        //recuperiamo la lista di termini da combinare tra loro
        var FixxedDictKey = permutation.Keys.ToArray()[iter];
        if (!permutation.TryGetValue(FixxedDictKey, out var listOFixxedDictKey))
        {
          Console.WriteLine($"Error during processing, for term \"{FixxedDictKey}\", list of permutation for create combination list; this term will skipped");
        }

        //cicliamo la lista e verifichiamo se abbiamo raggiunto il limite oppure no
        foreach (var termFixxed in listOFixxedDictKey)
        {
          if (CombinationList.Count() == maxNumberOfCombination - 1)
          {
            //se abbiamo raggiunto il limite di maxNumberOfCombination cicliamo tutte le lista rimaste nel dizionario
            //le componiamo e le passiamo alla procedura che scrivera sul file
            var tempList = new List<string>(CombinationList);
            tempList.Add(termFixxed);
            var partialCombinationList = GetCombination(tempList,
            0,
            CombinationList.Count(),
            passComposer);
            //TODO: data la lista di permutazioni lanciamo la procedura di verifica password e scrittura su file.
            passComposer.WritePassword(partialCombinationList);
          }
          else
          {
            //non abbiamo ancora raggiunto il limite esterno prepariamo al previous list e richiamiamo se stessa
            var tempList = new List<string>(CombinationList);
            tempList.Add(termFixxed);
            WriteListOfCombination(permutation, passComposer, iter + 1, tempList);
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
    private static List<string> GetCombination(List<string> terms, int startIndex, int endIndex, PassComposerManager passComposer)
    {
      var result = new List<string>();
      if (!terms.Any())
      {
        throw new ArgumentException($"{nameof(GetCombination)}: terms is empty");
      }
      //there are only tow term in list to combine, return partial list
      if (terms.Count() == 2)
      {
        return GetCombination(terms[0], terms[1], passComposer.GetPassDelimitator);
      }

      //foreach any terms and fix one term and swap other,
      for (var iterFixxed = startIndex; iterFixxed <= endIndex; iterFixxed++)
      {
        var termsFixxed = terms[iterFixxed];
        var cloneList = terms.GetRange(0, terms.Count());
        cloneList.RemoveAt(iterFixxed);
        var partialResult = GetCombination(cloneList, iterFixxed + 1, endIndex, passComposer);
        foreach (var partTerms in partialResult)
        {
          result.Add($"{termsFixxed}{partTerms}");
          foreach (var delimitator in passComposer.GetPassDelimitator)
          {
            result.Add($"{termsFixxed}{delimitator}{partTerms}");
          }
        }
      }
      return result.Where(password => passComposer.CheckValidityPassowrd(password)).ToList();
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

    #region utilsMethods
    #endregion
  }
  #endregion
}