using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;

namespace SimpleConfigSections.BasicExtensions
{

    internal static class BasicExtensions
    {
        internal static string PropertyName(this MethodInfo methodInfo)
        {
            return methodInfo.Name.Substring(4);
        }

        internal static void Each<T>(this IEnumerable<T> sequence, Action<T> job)
        {
            foreach (var obj in sequence)
            {
                job(obj);
            }
        }

        internal static string ToFormat(this string format, params object[] args)
        {
            return string.Format(format, args);
        }

        internal static bool IsNullOrEmptyOrWhiteSpace(this string value)
        {
            return string.IsNullOrEmpty(value) || value.IsWhiteSpace();
        }

        private static bool IsWhiteSpace(this string value)
        {
            return value.Any(Char.IsWhiteSpace);
        }
    }
}