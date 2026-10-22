using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Qahr.Mobile
{
    public static class SharedExtensions
    {
        public static T GetRandom<T>(this T[] array)
        {
            int index = Pool.GetRandom.Next(array.Length);
            return array[index];
        }
        public static T Pick<T>(this IEnumerable<T> collection)
        {
            var enumerable = collection as T[] ?? collection.ToArray();
            int count = enumerable.Count();
            if (count == 0) return default(T);
            return enumerable.ElementAt(BaseFunc.RandGet(count, false));
        }
        public static string RemoveIllegalCharacters(this string str, bool path, bool file)
        {
            string myString = str;
            if (file)
                foreach (char c in System.IO.Path.GetInvalidFileNameChars())
                    myString = myString.Replace(c, '_');
            if (path)
                foreach (char c in System.IO.Path.GetInvalidPathChars())
                    myString = myString.Replace(c, '_');
            return myString;
        }
        public static int AllMilliseconds(this DateTime date)
        {
            int val = Environment.TickCount + (int)((date.Ticks - DateTime.Now.Ticks) / TimeSpan.TicksPerMillisecond);
            return val;
        }
        public static int AllSeconds(this DateTime date)
        {
            return date.AllMilliseconds() / 1000;
        }
        public static int Value(this DateTime date)
        {
            return date.AllMilliseconds();
        }
        public static int GetHashCode(this DateTime date)
        {
            return Value(date);
        }
        public static void Iterate<T>(this T[] collection, Action<T> action)
        {
            foreach (var item in collection)
                action(item);
        }
        public static void Iteratefun<T>(this T[] collection, Action<T> action, Func<T, bool> applied = null)
        {
            foreach (var item in collection)
            {
                if (applied != null)
                {
                    if (applied(item))
                    {
                        action(item);
                    }
                }
                else
                { action(item); }
            }
        }
        public static void Iteratefunbool<T>(this T[] collection, Action<T> action, Func<T, bool> applied = null)
        {
            foreach (var item in collection)
            {
                if (applied != null)
                {
                    if (applied(item))
                    {
                        action.Invoke(item);
                    }
                }
                else
                { action.Invoke(item); }
            }
        }
    }
}
