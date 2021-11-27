using System;
using System.Collections;

public static class ComputeSimilarity
{
    /// <summary>
    /// Calculate percentage similarity of two strings
    /// <param name="source">Source String to Compare with</param>
    /// <param name="target">Targeted String to Compare</param>
    /// <returns>Return Similarity between two strings from 0 to 1.0</returns>
    /// </summary>
    public static double CalculateSimilarity(this string source, string target)
    {
        if ((source == null) || (target == null)) return 0.0;
        if ((source.Length == 0) || (target.Length == 0)) return 0.0;
        if (source == target) return 1.0;

        int stepsToSame = ComputeLevenshteinDistance(source, target);
        return (1.0 - ((double)stepsToSame / (double)Math.Max(source.Length, target.Length)));
    }
    /// <summary>
    /// Returns the number of steps required to transform the source string
    /// into the target string.
    /// </summary>
    static int ComputeLevenshteinDistance(string source, string target)
    {
        if ((source == null) || (target == null)) return 0;
        if ((source.Length == 0) || (target.Length == 0)) return 0;
        if (source == target) return source.Length;

        int sourceWordCount = source.Length;
        int targetWordCount = target.Length;

        // Step 1
        if (sourceWordCount == 0)
            return targetWordCount;

        if (targetWordCount == 0)
            return sourceWordCount;

        int[,] distance = new int[sourceWordCount + 1, targetWordCount + 1];

        // Step 2
        for (int i = 0; i <= sourceWordCount; distance[i, 0] = i++) ;
        for (int j = 0; j <= targetWordCount; distance[0, j] = j++) ;

        for (int i = 1; i <= sourceWordCount; i++)
        {
            for (int j = 1; j <= targetWordCount; j++)
            {
                // Step 3
                int cost = (target[j - 1] == source[i - 1]) ? 0 : 1;

                // Step 4
                distance[i, j] = Math.Min(Math.Min(distance[i - 1, j] + 1, distance[i, j - 1] + 1), distance[i - 1, j - 1] + cost);
            }
        }

        return distance[sourceWordCount, targetWordCount];
    }

    public static int LevenshteinDistance(string src, string dest)
    {
        int[,] d = new int[src.Length + 1, dest.Length + 1];
        int i, j, cost;
        char[] str1 = src.ToCharArray();
        char[] str2 = dest.ToCharArray();

        for (i = 0; i <= str1.Length; i++)
        {
            d[i, 0] = i;
        }
        for (j = 0; j <= str2.Length; j++)
        {
            d[0, j] = j;
        }
        for (i = 1; i <= str1.Length; i++)
        {
            for (j = 1; j <= str2.Length; j++)
            {

                if (str1[i - 1] == str2[j - 1])
                    cost = 0;
                else
                    cost = 1;

                d[i, j] =
                    Math.Min(
                        d[i - 1, j] + 1,              // Deletion
                        Math.Min(
                            d[i, j - 1] + 1,          // Insertion
                            d[i - 1, j - 1] + cost)); // Substitution

                if ((i > 1) && (j > 1) && (str1[i - 1] ==
                    str2[j - 2]) && (str1[i - 2] == str2[j - 1]))
                {
                    d[i, j] = Math.Min(d[i, j], d[i - 2, j - 2] + cost);
                }
            }
        }

        return d[str1.Length, str2.Length];
    }

    public static Hashtable Search(string inputWord, Hashtable originalHashtabs, double threshold)
    {
        Hashtable foundStories = new Hashtable();

        foreach(DictionaryEntry de in originalHashtabs)
        {
            // Calculate the Levenshtein-distance:
            int lavDistance = LevenshteinDistance(inputWord, de.Key.ToString());

            // Length of the longer string:
            int length = Math.Max(inputWord.Length, de.Key.ToString().Length);

            // Calculate the score:
            double score = 1.0 - (double)lavDistance / length;

            if (score > threshold)
                foundStories.Add(de.Key, de.Value);
        }

        return foundStories;
    }
}
