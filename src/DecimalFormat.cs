using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SasReader
{
    public class DecimalFormat : Format
    {
        private string pattern;

        public DecimalFormat(string pattern)
        {
            this.pattern = pattern;
        }

        public override string format(object value)
        {
            return Convert.ToDouble(value).ToString(this.pattern);
        }
    }
}
