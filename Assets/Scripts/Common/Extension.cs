using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

namespace Extension
{
    public static class Extension 
    {
        public static T GetRandom<T>(this List<T> source)
        {
            return source[UnityEngine.Random.Range(0, source.Count)];
        }

        public static float3 ToFloat3(this Vector3 source)
        {
            return new float3(source.x, source.y, source.z);
        }

        public static Vector3 ToVector3(this float3 source)
        {
            return new Vector3(source.x, source.y, source.z);
        }
    }

}
