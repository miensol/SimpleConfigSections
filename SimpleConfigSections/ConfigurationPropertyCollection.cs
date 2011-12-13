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

        public ConfigurationPropertyCollection(Type interfaceType)
        {
            _properties = interfaceType.GetProperties().Select(CreateConfigurationProperty);
        }

        #region IEnumerable<ConfigurationProperty> Members

        public IEnumerator<ConfigurationProperty> GetEnumerator()
        {
            return _properties.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        #endregion

        private ConfigurationProperty CreateConfigurationProperty(PropertyInfo pi)
        {
            var propertyType = pi.PropertyType;
            var propertyName = pi.Name;
            if (propertyType.IsInterface)
            {
                if (propertyType.IsGenericType)
                {
                    var genericTypeDefinition = propertyType.GetGenericTypeDefinition();

                    if (genericTypeDefinition == typeof (IEnumerable<>))
                    {
                        var elementType = propertyType.GetGenericArguments()[0];
                        return new ConfigurationProperty(propertyName,
                                                         typeof (ConfigurationElementCollectionForInterface<>).
                                                             MakeGenericType(elementType));
                    }
                }
                return new ConfigurationProperty(propertyName,
                                                 typeof (ConfigurationElementForInterface<>).MakeGenericType(
                                                     propertyType));
            }
            return new ConfigurationProperty(propertyName, propertyType);
        }
    }
}