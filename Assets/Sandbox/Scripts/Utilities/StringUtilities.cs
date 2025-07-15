using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Contains string utility methods (that don't belong as a string extension)
/// </summary>
public static class StringUtilities
{
    /// <summary>
    /// Return the ordinal suffic (st, nd) for a given number.
    /// https://stackoverflow.com/questions/69262/is-there-an-easy-way-in-net-to-get-st-nd-rd-and-th-endings-for-number
    /// </summary>
    public static string GetOrdinalSuffix(int num)
    {
        string number = num.ToString();
        if (number.EndsWith("11")) return "th";
        if (number.EndsWith("12")) return "th";
        if (number.EndsWith("13")) return "th";
        if (number.EndsWith("1")) return "st";
        if (number.EndsWith("2")) return "nd";
        if (number.EndsWith("3")) return "rd";
        return "th";
    }
}
