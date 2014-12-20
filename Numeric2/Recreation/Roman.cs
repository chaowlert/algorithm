using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Chaow.Extensions;

namespace Chaow.Numeric.Recreation
{
    public struct Roman
    {
        //fields
        readonly int _value;

        //properties

        //constructors
        public Roman(int value)
        {
            if (value < 1 || value > 4999)
                throw new ArgumentOutOfRangeException("value", "value should be between 1 and 4999");
            _value = value;
        }

        public int Value
        {
            get { return _value; }
        }

        //operators
        public static implicit operator Roman(int x)
        {
            return new Roman(x);
        }

        //public static methods
        public static Roman Parse(string roman)
        {
            const string letter = "IVXLCDM";
            var num = new[] {1, 5, 10, 50, 100, 500, 1000};
            var dict = letter.Zip(num).ToDictionary(n => n.Item1, n => n.Item2);

            int last = 0, sum = 0;
            foreach (var c in roman)
            {
                int value;
                if (!dict.TryGetValue(c, out value))
                    throw new ArgumentException("roman contains invalid characters", "roman");
                if (last < value)
                    sum -= last;
                else
                    sum += last;
                last = value;
            }
            sum += last;
            return new Roman(sum);
        }

        //public methods
        public override string ToString()
        {
            var numStr = _value.ToString("0000");
            const string one = "MCXI";
            string[] five = {"MMM", "D", "L", "V"};
            var sb = new StringBuilder();

            numStr.ForEach((c, i) =>
            {
                if (c < '4')
                    sb.Append(one[i], c - '0');
                else if (c == '4')
                    sb.Append(one[i]).Append(five[i]);
                else if (c < '9')
                    sb.Append(five[i]).Append(one[i], c - '5');
                else //9
                    sb.Append(one[i]).Append(one[i - 1]);
            });
            return sb.ToString();
        }

        public override bool Equals(object obj)
        {
            return obj is Roman && ((Roman)obj)._value == _value;
        }

        public override int GetHashCode()
        {
            return _value;
        }
    }
}