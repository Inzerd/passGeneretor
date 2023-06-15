using System;
using System.Collections.Generic;
using System.Text;

namespace passgeneretor.Models
{
  static class WellKnow
  {
    public static string manual = $"\n\t\tP4ssG3n3r3Th0r Manual:\n" +
      $"\nMandatory:" +
      $"\n\t-u[url]: file info user mandatory" +
      $"\n\t-o[url]: output file" +
      $"\n\t-l [lenght]: password's lenght" +
      $"\n\t-c/-p: at least one of these options" +
      $"\nOptions:" +
      $"\n\t-rule [cns]: rule from generate password, this options generate a regexp used for validate genereted password:" +
      $"\n\t\t c = use Major and Minor Case" +
      $"\n\t\t n = use number" +
      $"\n\t\t s = use special char" +
      $"\n\t-t[url]: file with transformation from char/ string to number/ special char/ string or another transformation string" +
      $"\n\t-c combination: combined any trasfomartion will creating more pass complex" +
      $"\n\t-p permutation: substitute char in term with Trasformation List rule in configuration json file" +
      $"\n\t-h manual: show manual in console";
  }
}
