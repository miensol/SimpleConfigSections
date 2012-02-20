using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;

namespace SimpleConfigSections
{
    internal class ConfigurationPropertyCollection : IEnumerable<ConfigurationProperty>
    {
        private readonly IEnumerable<ConfigurationProperty> _properties;
        private readonly ConfigurationPropertyFactory _configurationPropertyFactory;

        public ConfigurationPropertyCollection(Type interfaceType)
        {
            _properties = interfaceType.GetProperties().Select(CreateConfigurationProperty);
            _configurationPropertyFactory = new ConfigurationPropertyFactory();
        }


        public IEnumerator<ConfigurationProperty> GetEnumerator()
        {
            return _properties.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }


        private ConfigurationProperty CreateConfigurationProperty(PropertyInfo pi)
        {
            var propertyType = pi.PropertyType;
            if (propertyType.IsInterface)
            {
                if (propertyType.IsGenericType)
                {
                    var genericTypeDefinition = propertyType.GetGenericTypeDefinition();

                    if (genericTypeDefinition == typeof (IEnumerable<>))
                    {
                        var elementType = propertyType.GetGenericArguments()[0];
                        return _configurationPropertyFactory.Collection(pi,elementType);
                    }
                }
                return _configurationPropertyFactory.Interface(pi);
                  }
            return _configurationPropertyFactory.Simple(pi);
        }
    }
}