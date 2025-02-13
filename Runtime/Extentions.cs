using System;

public static class Extentions
{
    const double K = 1000;
    const double M = K * 1000;
    const double B = M * 1000;
    const double T = B * 1000;
    const double q = T * 1000;
    const double Q = q * 1000;
    const double s = Q * 1000;
    const double S = s * 1000;

    public static string ShortFormat(this int value, int digits = 0)
    {
        return ShortFormat((double)value, digits);
    }
    public static string ShortFormat(this float value, int digits = 0)
    {
        return ShortFormat((double)value, digits);
    }
    public static string ShortFormat(this double value, int digits = 0)
    {
        if (value >= S)
            return (value / S).ToString("0.##S");

        if (value >= s)
            return (value / s).ToString("0.##s");

        if (value >= Q)
            return (value / Q).ToString("0.##Q");

        if (value >= q)
            return (value / q).ToString("0.##q");

        if (value >= T)
            return (value / T).ToString("0.##T");

        if (value >= B)
            return (value / B).ToString("0.##B");

        if (value >= M)
            return (value / M).ToString("0.##M");

        if (value >= K)
            return (value / K).ToString("0.##K");

        return Math.Round(value, digits, MidpointRounding.ToEven).ToString();
    }
}
