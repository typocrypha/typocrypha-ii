using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public static class StringExtensions
{
    /// <summary>
    /// Finds index of first occurence of search character.
    /// </summary>
    /// <param name="str">Original string.</param>
    /// <param name="search">Character to search for.</param>
    /// <param name="startPos">Index to start at.</param>
    /// <param name="escapes">Escape characters.</param>
    /// <returns>Index of first occurence of search term. -1 if not found.</returns>
    public static int IndexOf(this string str, char search, int startPos, char[] escapes)
    {
        for (int i = startPos; i < str.Length; i++)
        {
            char c = str[i];
            if (escapes.Contains(c))
            {
                i++;
            }
            else if (c == search)
            {
                return i;
            }
        }
        return -1;
    }

    /// <summary>
    /// Split a string based on a delimiter.
    /// </summary>
    /// <param name="str">Original string.</param>
    /// <param name="delims">Delimiters to split on.</param>
    /// <param name="escapes">Escape characters.</param>
    /// <returns>Array of split characters.</returns>
    public static string[] Split(this string str, char[] delims, char[] escapes)
    {
        List<string> strl = new List<string>();
        StringBuilder strb = new StringBuilder();
        for (int i = 0; i < str.Length; i++)
        {
            char c = str[i];
            if (escapes.Contains(c))
            {
                strb.Append(str[++i]);
            }
            else if (delims.Contains(c))
            {
                strl.Add(strb.ToString());
                strb.Clear();
            }
            else
            {
                strb.Append(c);
            }
        }
        if (strb.Length > 0) strl.Add(strb.ToString());
        return strl.ToArray();
    }
}
