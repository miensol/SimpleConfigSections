using System;

namespace SimpleConfigSections
{
    internal class ClientValueResolver
    {
        private readonly CacheCallback<string,object> _cachedValues;
        private readonly IBaseValueProvider _source;
        private readonly Type _sourceInterfaceType;

        public ClientValueResolver(IBaseValueProvider source, Type sourceInterfaceType)
        {
            _cachedValues = new CacheCallback<string, object>(ClientValueCreate);
            _source = source;
            _sourceInterfaceType = sourceInterfaceType;
        }

        public object ClientValue(string propertyName)
        {
            return _cachedValues.Get(propertyName);
        }

        private object ClientValueCreate(string propertyName)
        {
            var obj = _source[propertyName];
            if (obj is IConfigValue)
            {
                return
                    new ConcreteConfiguration((IConfigValue)obj).ClientValue(
                        _sourceInterfaceType.GetProperty(propertyName).PropertyType);
            }
            return obj;
        }
    }
}