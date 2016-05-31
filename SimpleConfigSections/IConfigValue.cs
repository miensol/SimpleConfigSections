using System.Reflection;

namespace SimpleConfigSections
{
    internal interface IConfigValue
    {
        object Value(PropertyInfo property);
    }
}