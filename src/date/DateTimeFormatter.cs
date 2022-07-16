using System;
using System.Globalization;

namespace SasReader
{
    public class DateTimeFormatter : Format
    {
        public static readonly CultureInfo DefaultCulture = new CultureInfo("en-US");
        public CultureInfo Culture { get; set; } = DefaultCulture;
        public TimeZoneInfo TimeZone { get; set; } = TimeZoneInfo.Utc;
        public string Pattern { get; private set; }

        private static int GetDayOfWeekNumber(DateTime dateTime)
        {
            return ((int)dateTime.DayOfWeek + 6) % 7 + 1;
        }

        public DateTimeFormatter WithZone(TimeZoneInfo zoneInfo)
        {
            this.TimeZone = zoneInfo;
            return this;
        }

        public DateTimeFormatter WithCulture(CultureInfo culture)
        {
            this.Culture = culture;
            return this;
        }

        public static DateTimeFormatter OfPattern(string pattern)
        {
            return new DateTimeFormatter(pattern);
        }

        public string Format(DateTime dateTime)
        {
            dateTime = TimeZoneInfo.ConvertTime(dateTime, TimeZone);

            if (Pattern == "u")
            {
                return GetDayOfWeekNumber(dateTime).ToString();
            }

            if (Pattern == "D")
            {
                return dateTime.DayOfYear.ToString();
            }

            return dateTime.ToString(
                Pattern
                .Replace("u", GetDayOfWeekNumber(dateTime).ToString()), Culture)
                .Replace("D", dateTime.DayOfYear.ToString())
                .Replace("#", TimeZone.Id)
                .Replace("Z", dateTime.ToString("zzz").Replace(":", ""))
                .Replace("SSS", dateTime.Millisecond.ToString("D3"))
                .Replace("SS", dateTime.Millisecond.ToString("D2")
            );
        }

        public override string format(object dateTime)
        {
            return dateTime == null ? null : Format((DateTime)dateTime);
        }

        public DateTimeFormatter(string pattern)
        {
            Pattern = pattern.Replace("EEEE", "dddd").Replace("EEE", "ddd");
        }
    }
}