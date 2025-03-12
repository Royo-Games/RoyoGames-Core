using System;
using System.Text;

public static class TimeExtentions
{
    public static string DayShort = "d";
    public static string HoursShort = "h";
    public static string MinuntesShort = "m";
    public static string SecondsShort = "s";

    public static string ToTimeString(this int seconds, int maxUnit = 4)
    {
        return ToTimeString(new TimeSpan(0, 0, 0, seconds), maxUnit);
    }
    public static string ToTimeString(this TimeSpan timeSpan, int maxUnit = 4)
    {
        StringBuilder builder = new StringBuilder();

        int count = 0;

        if (count < maxUnit && timeSpan.Days >= 1)
        {
            builder.AppendFormat("{0}{1}", timeSpan.Days, DayShort);
            count++;
        }

        if (count < maxUnit && timeSpan.Hours >= 1)
        {
            if (timeSpan.Days >= 1)
                builder.Append(" ");

            builder.AppendFormat(" {0}{1}", timeSpan.Hours, HoursShort);
            count++;
        }

        if (count < maxUnit && timeSpan.Minutes >= 1)
        {
            if (timeSpan.Hours >= 1)
                builder.Append(" ");

            builder.AppendFormat(" {0}{1}", timeSpan.Minutes, MinuntesShort);
            count++;
        }

        if (count < maxUnit && timeSpan.Seconds >= 1)
        {
            if (timeSpan.Minutes >= 1)
                builder.Append(" ");

            builder.AppendFormat(" {0}{1}", timeSpan.Seconds, SecondsShort);
        }

        if (builder.Length == 0)
        {
            builder.AppendFormat("0{0}", SecondsShort);
        }

        return builder.ToString();
    }
}
