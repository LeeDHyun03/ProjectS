using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

using Random = UnityEngine.Random;
public static class Extension
{
    public static List<T> Shuffle<T>(this List<T> origin)
    {
        List<T> shuffled = new(origin);
        for (int i = 0; i < shuffled.Count; i++)
        {
            int r = Random.Range(i, shuffled.Count);
            (shuffled[i], shuffled[r]) = (shuffled[r], shuffled[i]);
        }

        return shuffled;
    }
}
