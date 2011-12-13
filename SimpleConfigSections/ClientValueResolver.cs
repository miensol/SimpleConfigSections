using System;

namespace SimpleConfigSections
{
    internal class ClientValueResolver
    {
        private readonly IBaseValueProvider _source;
        private readonly Type _sourceInterfaceType;

        public ClientValueResolver(IBaseValueProvider source, Type sourceInterfaceType)
        {
            _source = source;
            _sourceInterfaceType = sourceInterfaceType;
        }

        public object ClientValue(string propertyName)
        {
            object obj = _source[propertyName];
            if (obj is IConfigValue)
                return
                    new ConcreteConfiguration((IConfigValue) obj).ClientValue(
                        _sourceInterfaceType.GetProperty(propertyName).PropertyType);
            else
                return obj;
        }
    }
}