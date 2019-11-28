using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Vector2Extensions 
{
    public static float Base(this Vector2 vec)
    {
        return vec.x;
    }
    public static float Variable(this Vector2 vec)
    {
        return vec.y;
    }
    public static float Min(this Vector2 vec)
    {
        return vec.x;
    }
    public static float Max(this Vector2 vec)
    {
        return vec.x + vec.y;
    }
    public static float Average(this Vector2 vec)
    {
        return vec.x + vec.y / 2;
    }
    public static float Roll(this Vector2 vec)
    {
        return vec.x + vec.y * Random.value;
    }

    public static Vector2 Min(this Vector2 vec, Vector2 other)
    {
        return new Vector2(Mathf.Min(vec.x, other.x), Mathf.Min(vec.y, other.y));
    }

    public static Vector2 Max(this Vector2 vec, Vector2 other)
    {
        return new Vector2(Mathf.Max(vec.x, other.x), Mathf.Max(vec.y, other.y));
    }

    public static float Cross(this Vector2 vec, Vector2 other)
    {
        return (vec.x * other.y) - (vec.y * other.x);
    }

    public static float Dot(this Vector2 vec, Vector2 other)
    {
        return Vector2.Dot(vec, other);
    }
}
