using System;

namespace Chaow.Extensions
{
    public static class ArrayExt
    {
        public static T[] Create<T>(int length, T defaultValue)
        {
            var result = new T[length];
            for (var i = 0; i < result.Length; i++)
                result[i] = defaultValue;
            return result;
        }

        public static T[] Copy<T>(this T[] source)
        {
            return Copy(source, 0, source.Length);
        }

        public static T[] Copy<T>(this T[] source, int length)
        {
            return Copy(source, 0, length);
        }

        public static T[] Copy<T>(this T[] source, int index, int length)
        {
            var result = new T[length];
            Array.Copy(source, index, result, 0, length);
            return result;
        }

        public static T[] Append<T>(this T[] source, T item)
        {
            var length = source.Length;
            var result = new T[length + 1];
            Array.Copy(source, result, length);
            result[length] = item;
            return result;
        }

        public static T[] Enum<T>()
        {
            return (T[])System.Enum.GetValues(typeof(T));
        }
    }
}