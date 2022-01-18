using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HelperFunctions
{
    public static RectTransform RectTranform(this GameObject transform)
    {
        return transform.GetComponent<RectTransform>();

    }

    public static int SortByScore(Tile p1, Tile p2)
    {
        return int.Parse(p2.textLabel.text).CompareTo(int.Parse(p1.textLabel.text));
    }
}

public static class IListExtensions
{
    /// <summary>
    /// Shuffles the element order of the specified list.
    /// </summary>
    public static void Shuffle<T>(this IList<T> ts)
    {
        var count = ts.Count;
        var last = count - 1;
        for (var i = 0; i < last; ++i)
        {
            var r = UnityEngine.Random.Range(i, count);
            var tmp = ts[i];
            ts[i] = ts[r];
            ts[r] = tmp;
        }
    }

   
}