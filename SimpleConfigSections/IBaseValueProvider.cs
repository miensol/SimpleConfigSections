using System.Reflection;

namespace SimpleConfigSections
{
    internal interface IBaseValueProvider
    {
        object this[PropertyInfo property] { get; }
    }
}