using System;
using System.Collections.Generic;
using System.Reflection;

namespace SimpleConfigSections.BasicExtensions
{

    internal static class BasicExtensions
    {
        public static string PropertyName(this MethodInfo methodInfo)
        {
            return methodInfo.Name.Substring(4);
        }

        public static void Each<T>(this IEnumerable<T> sequence, Action<T> job)
        {
            foreach (var obj in sequence)
            {
                job(obj);
            }
        }

        public static string ToFormat(this string format, params object[] args)
        {
            return string.Format(format, args);
        }
    }
}