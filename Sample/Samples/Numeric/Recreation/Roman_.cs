using System;

namespace Chaow.Numeric.Recreation
{
    //Roman represents Roman numeral system
    public static class Roman_
    {
        public static void Create()
        {
            //this example shows how to create Roman

            //you can directly assign value to Roman
            //value cannot exceed 4999
            Roman roman = 25;

            //show results
            Console.WriteLine(roman);
        }

        public static void Value()
        {
            //this example shows how to get Roman value

            //create Roman
            Roman roman = 25;

            //use roman.Value to get value
            Console.WriteLine(roman.Value);
        }

        public static void Paste()
        {
            //this example shows how to paste Roman

            //use Roman.Paste(str) to paste Roman
            var roman = Roman.Parse("XXV");

            //show result
            Console.WriteLine(roman.Value);
        }
    }
}