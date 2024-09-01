using UnityEngine;

public static class EasingFunctions
{
    public static float EaseInSine(float x) => 1 - Mathf.Cos((x * Mathf.PI) / 2);

    public static float InverseEaseInSine(float y) => (2 * Mathf.Acos(1 - y)) / Mathf.PI;

    public static float EaseOutSine(float x) => Mathf.Sin((x * Mathf.PI) / 2);

    public static float InverseEaseOutSine(float y) => Mathf.Asin(y) * 2 / Mathf.PI;



    public static float EaseInCubic(float x) => x * x * x;

    public static float InverseEaseInCubic(float y) => Mathf.Pow(y, 1f / 3f);

    public static float EaseOutCubic(float x) => 1 - Mathf.Pow(1 - x, 3);

    public static float InverseEaseOutCubic(float x) => 1 - Mathf.Pow(1 - x, 1f / 3f);



    public static float EaseInQuint(float x) => x * x * x * x * x;

    public static float InverseEaseInQuint(float y) => Mathf.Pow(y, 1f / 5f);

    public static float EaseOutQuint(float x) => 1 - Mathf.Pow(1 - x, 5);

    public static float InverseEaseOutQuint(float y) => 1 - Mathf.Pow(1 - y, 1f / 5f);



    public static float EaseInCirc(float x) => 1 - Mathf.Sqrt(1 - x * x);

    public static float InverseEaseInCirc(float y) => Mathf.Sqrt(1 - (1 - y) * (1 - y));

    public static float EaseOutCirc(float x) => Mathf.Sqrt(1 - Mathf.Pow(x - 1, 2));

    public static float InverseEaseOutCirc(float y) => 1 - Mathf.Sqrt(1 - y * y);



    public static float EaseInQuad(float x) => x * x;

    public static float InverseEaseInQuad(float y) => Mathf.Sqrt(y);

    public static float EaseOutQuad(float x) => 1 - (1 - x) * (1 - x);

    public static float InverseEaseOutQuad(float y) => 1 - Mathf.Sqrt(1 - y);



    public static float EaseInQuart(float x) => x * x * x * x;

    public static float InverseEaseInQuart(float y) => Mathf.Pow(y, 1f / 4f);

    public static float EaseOutQuart(float x) => 1 - Mathf.Pow(1 - x, 4);

    public static float InverseEaseOutQuart(float y) => 1 - Mathf.Pow(1 - y, 1f / 4f);



    public static float EaseInExpo(float x) => x == 0 ? 0 : Mathf.Pow(2, 10 * x - 10);

    public static float InverseEaseInExpo(float y) => y == 0 ? 0 : (Mathf.Log(y) / Mathf.Log(2) + 10) / 10;

    public static float EaseOutExpo(float x) => x == 1 ? 1 : 1 - Mathf.Pow(2, -10 * x);

    public static float InverseEaseOutExpo(float y) => y == 1 ? 1 : -Mathf.Log(1 - y) / (10 * Mathf.Log(2));
}
