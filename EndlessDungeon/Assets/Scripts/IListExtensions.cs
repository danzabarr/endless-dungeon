using System.Collections.Generic;
using UnityEngine;

public static class IListExtensions
{
    public static IList<T> Shuffle<T>(this IList<T> ts)
    {
        var count = ts.Count;
        var last = count - 1;
        for (var i = 0; i < last; ++i)
        {
            var r = Random.Range(i, count);
            var tmp = ts[i];
            ts[i] = ts[r];
            ts[r] = tmp;
        }
        return ts;
    }

    public static List<T> ShuffledCopy<T>(this IList<T> ts)
    {
        List<T> copy = new List<T>(ts);
        copy.Shuffle();
        return copy;
    }

    
    public static List<T> DistanceShortlist<T>(this IList<T> list, int minCount, int maxCount, float minDistance) where T : Component
    {
        if (list.Count <= 1) return new List<T>();

        List<T> copy = new List<T>(list);
        List<T> shortList = new List<T>();

        


        T first = copy[Random.Range(0, copy.Count)];
        shortList.Add(first);
        copy.Remove(first);

        for (int i = 0; i < maxCount - 1; i++)
        {
            if (copy.Count <= 0) return shortList;

            T loneliest = default;
            float longestDistance = 0;

            foreach (T s1 in copy)
            {
                float nearestOther = float.MaxValue;
                foreach (T s2 in shortList)
                {
                    if (s1 == s2) continue;
                    float distance = Vector3.Distance(s1.transform.position, s2.transform.position);
                    nearestOther = Mathf.Min(nearestOther, distance);
                }
                if (loneliest == null || nearestOther > longestDistance)
                {
                    loneliest = s1;
                    longestDistance = nearestOther;
                }
            }


            if (loneliest == null) return shortList;
            if (shortList.Count >= minCount && longestDistance < minDistance) return shortList;
            
            copy.Remove(loneliest);
            shortList.Add(loneliest);
        }
        return shortList;
    }
}