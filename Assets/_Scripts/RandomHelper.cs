using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomHelper
{
    public static float RandomValue(float x)
    {
        return (Mathf.PerlinNoise(x, GameManager.Instance.Seed));
    }

    public static float RandomRange(float x, float minInclusive, float maxExclusive)
    {
        return RandomValue(x) * (maxExclusive - minInclusive) + minInclusive;
    }

    public static List<T> Shuffle<T>(float x, List<T> list)
    {
        List<T> elements = new List<T>(list);
        List<T> result = new List<T>();
        int i = 0;
        while(elements.Count > 0)
        {
            T element = elements[Mathf.FloorToInt(RandomRange(x * 123456 + GameManager.Instance.Seed + 0.5f + i, 0, elements.Count))];
            elements.Remove(element);
            result.Add(element);
            i++;
        }
        return result;
    }
}
