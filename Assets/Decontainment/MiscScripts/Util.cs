using UnityEngine;

public static class Util
{
    public static float Distance(Vector2 v1, Vector2 v2)
    {
        return (v1 - v2).magnitude;
    }

    public static Color ModifyAlpha(Color color, float alpha)
    {
        color.a = alpha;
        return color;
    }
}