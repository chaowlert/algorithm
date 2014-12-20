using System;
using System.IO;
using System.Text;

namespace Chaow.Sample
{
    public class CallbackWriter : TextWriter
    {
        //fields
        readonly Action<string> _action;

        //constructors
        public CallbackWriter(Action<string> action)
        {
            _action = action;
        }

        public override Encoding Encoding
        {
            get { throw new NotSupportedException(); }
        }

        //public methods
        public override void Write(char value)
        {
            _action(value.ToString());
        }

        public override void Write(string value)
        {
            _action(value);
        }

        public override void Write(char[] buffer, int index, int count)
        {
            _action(new string(buffer, index, count));
        }

        //properties
    }
}