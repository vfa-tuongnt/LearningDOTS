using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Extension
{
    public static class Extension 
    {
        public static T GetRandom<T>(this List<T> source)
        {
            return source[UnityEngine.Random.Range(0, source.Count)];
        }
    }

}
