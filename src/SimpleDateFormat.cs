using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SasReader
{
    public class SimpleDateFormat : Format
    {
        private string datePattern;
        private CultureInfo cultureInfo;
        public TimeZoneInfo Zone { get; set; }
        public SimpleDateFormat(string datePattern, CultureInfo cultureInfo)
        {
            this.datePattern = datePattern;
            this.cultureInfo = cultureInfo;
        }

        public string Format(DateTime dateTime)
        {
            return dateTime.ToString(datePattern, this.cultureInfo);
        }

        public override string format(object value)
        {
            return value == null ? "" : Format((DateTime)value);
        }
    }
}
