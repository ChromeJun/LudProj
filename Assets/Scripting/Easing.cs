using UnityEngine;

public static class Easing
{
    const float pi = Mathf.PI;
    const float c1 = 1.70158f;
    const float c2 = c1 * 1.525f;
    const float c3 = c1 + 1f;
    const float c4 = (2f * pi) / 3f;
    const float c5 = (2f * pi) / 4.5f;
    const float c6 = 11.125f;

    const float n1 = 7.5625f;
    const float d1 = 2.75f;

    /// <summary>
    /// Eases the in out quadratically.
    /// </summary>

    #region Quadratic

    public static float EaseInOutQuad(float x)
    {
        if (x <= 0f) return 0f; if (x >= 1f) return 1f; // Clamp 0-1
        return (x < 0.5f) ? 2 * x * x : 1f - (Mathf.Pow((-2f * x + 2), 2)) / 2f;
    }

    public static float EaseInQuad(float x)
    {
        if (x <= 0f) return 0f; if (x >= 1f) return 1f; // Clamp 0-1
        return x * x;
    }

    public static float EaseOutQuad(float x)
    {
        if (x <= 0f) return 0f; if (x >= 1f) return 1f; // Clamp 0-1
        return 1 - (1 - x) * (1 - x);
    }

    #endregion




    /// <summary>
    /// Eases the in out cubically.
    /// </summary>

    #region Cubic

    public static float EaseInOutCubic(float x)
    {
        if (x <= 0f) return 0f; if (x >= 1f) return 1f; // Clamp 0-1
        return (x < 0.5f) ? 4f * x * x * x : 1f - Mathf.Pow(-2f * x + 2, 3) / 2f;
    }

    public static float EaseInCubic(float x)
    {
        if (x <= 0f) return 0f; if (x >= 1f) return 1f; // Clamp 0-1
        return x * x * x;
    }

    public static float EaseOutCubic(float x)
    {
        if (x <= 0f) return 0f; if (x >= 1f) return 1f; // Clamp 0-1
        return 1f - Mathf.Pow(1f - x, 3f);
    }

    #endregion




    /// <summary>
    /// Eases the in out circularly?.
    /// </summary>

    #region Circ

    public static float EaseInOutCirc(float x)
    {
        if (x <= 0f) return 0f; if (x >= 1f) return 1f; // Clamp 0-1
        return x < 0.5f ? (1f - Mathf.Sqrt(1f - Mathf.Pow(2f * x, 2f))) / 2f : (Mathf.Sqrt(1f - Mathf.Pow(-2f * x + 2f, 2f)) + 1f) / 2f;
    }

    public static float EaseInCirc(float x)
    {
        if (x <= 0f) return 0f; if (x >= 1f) return 1f; // Clamp 0-1
        return 1 - Mathf.Sqrt(1f - Mathf.Pow(x, 2));
    }

    public static float EaseOutCirc(float x)
    {
        if (x <= 0f) return 0f; if (x >= 1f) return 1f; // Clamp 0-1
        return Mathf.Sqrt(1f - Mathf.Pow(x - 1f, 2f));
    }

    #endregion




    /// <summary>
    /// Eases the in out with a pull back.
    /// </summary>

    #region Back

    public static float EaseInOutBack(float x)
    {
        if (x <= 0f) return 0f; if (x >= 1f) return 1f; // Clamp 0-1
        return x < 0.5f ? (Mathf.Pow(2f * x, 2f) * ((c2 + 1f) * 2f * x - c2)) / 2f : (Mathf.Pow(2f * x - 2f, 2f) * ((c2 + 1f) * (x * 2f - 2f) + c2) + 2f) / 2f;
    }

    public static float EaseInBack(float x)
    {
        if (x <= 0f) return 0f; if (x >= 1f) return 1f; // Clamp 0-1
        return c3 * x * x * x - c1 * x * x;
    }

    public static float EaseOutBack(float x)
    {
        if (x <= 0f) return 0f; if (x >= 1f) return 1f; // Clamp 0-1
        return 1f + c3 * Mathf.Pow(x - 1f, 3f) + c1 * Mathf.Pow(x - 1f, 2f);
    }

    #endregion




    /// <summary>
    /// Eases the in out with an elastic end.
    /// </summary>

    #region Elastic

    public static float EaseInOutElastic(float x)
    {
        if (x <= 0f) return 0f; if (x >= 1f) return 1f; // Clamp 0-1
        return (x < 0.5f) ? -(Mathf.Pow(2f, 20f * x - 10f) * Mathf.Sin((20f * x - c6) * c5)) / 2f : Mathf.Pow(2f, -20f * x + 10f) * Mathf.Sin((20f * x - c6) * c5) / 2f + 1f;
    }

    public static float EaseInElastic(float x)
    {
        if (x <= 0f) return 0f; if (x >= 1f) return 1f; // Clamp 0-1
        return -Mathf.Pow(2f, 10f * x - 10f) * Mathf.Sin((x * 10f - 10.75f) * c4);
    }

    public static float EaseOutElastic(float x)
    {
        if (x <= 0f) return 0f; if (x >= 1f) return 1f; // Clamp 0-1
        return Mathf.Pow(2f, -10f * x) * Mathf.Sin((x * 10f - 0.75f) * c4) + 1f;
    }

    #endregion


    /// <summary>
    /// Eases the in out with bounce.
    /// </summary>

    #region Elastic

    public static float EaseInOutBounce(float x)
    {
        if (x <= 0f) return 0f; if (x >= 1f) return 1f; // Clamp 0-1
        return x < 0.5f ? (1f - BounceOut(1f - 2f * x)) / 2f : (1f + BounceOut(2f * x - 1f)) / 2f;
    }

    public static float EaseInBounce(float x)
    {
        if (x <= 0f) return 0f; if (x >= 1f) return 1f; // Clamp 0-1
        return 1 - BounceOut(1 - x);

    }

    public static float EaseOutBounce(float x)
    {
        if (x <= 0f) return 0f; if (x >= 1f) return 1f; // Clamp 0-1
        return BounceOut(x);
    }

    #endregion

    private static float BounceOut(float x)
    {
        if (x < 1 / d1) return n1 * x * x;
        if (x < 2 / d1) return n1 * (x -= (1.5f / d1)) * x + .75f;
        if (x < 2.5 / d1) return n1 * (x -= (2.25f / d1)) * x + .9375f;
        return n1 * (x -= (2.625f / d1)) * x + .984375f;
    }


    /// <summary>
    /// Eases in and out in a bell curve
    /// </summary>

    public static float BellCurve(float x)
    {
        return (Mathf.Sin(2 * Mathf.PI * (x - 0.25f)) + 1) / 2;
    }
}