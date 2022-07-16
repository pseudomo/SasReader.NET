using Deveel.Math;

namespace SasReader
{
    /**
     * Date and time related constants.
     */
    public static class SasTemporalConstants
    {

        /**
         * Milliseconds in one second.
         */
        public const int MILLIS_IN_SECOND = 1000;

        /**
         * Seconds in one minute.
         */
        public const int SECONDS_IN_MINUTE = 60;

        /**
         * Seconds in one minute as BigDecimal.
         */
        public static readonly BigDecimal BIG_SECONDS_IN_MINUTE = BigDecimal.Create(SECONDS_IN_MINUTE);

        /**
         * Minutes in one hour.
         */
        public const int MINUTES_IN_HOUR = 60;

        /**
         * Minutes in one minute as BigDecimal.
         */
        public static readonly BigDecimal BIG_MINUTES_IN_HOUR = BigDecimal.Create(MINUTES_IN_HOUR);

        /**
         * Seconds in one hour.
         */
        public const int SECONDS_IN_HOUR = SECONDS_IN_MINUTE * MINUTES_IN_HOUR;

        /**
         * Seconds in one hour as BigDecimal.
         */
        public static readonly BigDecimal BIG_SECONDS_IN_HOUR = BigDecimal.Create(SECONDS_IN_HOUR);

        /**
         * Seconds in one day.
         */
        public const int SECONDS_IN_DAY = SECONDS_IN_HOUR * 24;

        /**
         * Seconds in one day as BigDecimal.
         */
        public static readonly BigDecimal BIG_SECONDS_IN_DAY = BigDecimal.Create(SECONDS_IN_DAY);

        /**
         * Nanoseconds in a second.
         */
        public const int NANOS_IN_SECOND = 1_000_000_000;

        /**
         * Nanoseconds in a second as BigDecimal.
         */
        public static readonly BigDecimal BIG_NANOS_IN_SECOND = BigDecimal.Create(NANOS_IN_SECOND);

        /**
         * Single nanosecond in scope of second.
         */
        public static readonly BigDecimal BIG_NANOSECOND_FRACTION = new BigDecimal(0.000000001);

        /**
         * The difference in days between 01/01/1960 (the dates starting point in SAS)
         * and 01/01/1970 (the dates starting point in Java).
         */
        public const int SAS_VS_EPOCH_DIFF_DAYS = 365 * 10 + 3;

        /**
         * The difference in seconds between 01/01/1960 (the dates starting point in SAS)
         * and 01/01/1970 (the dates starting point in Java).
         */
        public const int SAS_VS_EPOCH_DIFF_SECONDS = SAS_VS_EPOCH_DIFF_DAYS * SECONDS_IN_DAY;

        /**
         * First time when a leap day is removed from the SAS calendar.
         * In days since 1960-01-01
         */
        public const double SAS_DAYS_29FEB4000 = 745_154D;

        /**
         * First time when a leap day is removed from the SAS calendar.
         * In seconds since 1960-01-01
         */
        public const double SAS_SECONDS_29FEB4000 = SAS_DAYS_29FEB4000 * SECONDS_IN_DAY;

        /**
         * Second time when a leap day is removed from the SAS calendar.
         * In days since 1960-01-01
         */
        public const double SAS_DAYS_29FEB8000 = 2_206_123;

        /**
         * Second time when a leap day is removed from the SAS calendar.
         * In seconds since 1960-01-01
         */
        public const double SAS_SECONDS_29FEB8000 = SAS_DAYS_29FEB8000 * SECONDS_IN_DAY;
    }
}