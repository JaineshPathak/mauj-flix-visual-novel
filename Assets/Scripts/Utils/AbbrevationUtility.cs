using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class AbbrevationUtility
{
    private static readonly SortedDictionary<long, string> abbrevations = new SortedDictionary<long, string>
     {
         {1000, "K" },
         {1000000, "M" },
         {1000000000, "B" },
         {1000000000000, "T" },
         {1000000000000000, "Q" }
     };

    public static string AbbreviateNumber(float number)
    {
        for (int i = abbrevations.Count - 1; i >= 0; i--)
        {
            KeyValuePair<long, string> pair = abbrevations.ElementAt(i);
            if (Mathf.Abs(number) >= pair.Key)
            {
                int roundedNumber = Mathf.FloorToInt(number / pair.Key);
                return roundedNumber.ToString() + pair.Value;
            }
        }
        return number.ToString("F0");
    }
}