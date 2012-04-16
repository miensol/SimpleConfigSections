using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Reflection;

namespace SimpleConfigSections
{
    internal class ConfigurationPropertyCollection : IEnumerable<ConfigurationProperty>
    {
        private readonly Type _ownerType;
        private readonly IEnumerable<ConfigurationProperty> _properties;
        private readonly ConfigurationPropertyFactory _configurationPropertyFactory;
        private readonly ConfigurationElementHiddenPropertyBagModifier _hiddenPropertyBagModifier;

        public ConfigurationPropertyCollection(Type interfaceType, Type ownerType)
        {
            _ownerType = ownerType;

            var propertyInfos = GetPublicProperties(interfaceType);                        
            _properties = propertyInfos.Select(CreateConfigurationProperty);
            _configurationPropertyFactory = new ConfigurationPropertyFactory();
            _hiddenPropertyBagModifier = new ConfigurationElementHiddenPropertyBagModifier();

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
            ConfigurationProperty result = null;
            if (propertyType.IsInterface)
            {
                if (propertyType.IsGenericType)
                {
                    var genericTypeDefinition = propertyType.GetGenericTypeDefinition();

                    if (genericTypeDefinition == typeof (IEnumerable<>))
                    {
                        var elementType = propertyType.GetGenericArguments()[0];
                        result = _configurationPropertyFactory.Collection(pi, elementType);
                    }
                }else
                {
                    result = _configurationPropertyFactory.Interface(pi);
                }                
            }else
            {
                if(propertyType.IsClass && false == TypeDescriptor.GetConverter(propertyType).CanConvertFrom(typeof(string)))
                {
                    result = _configurationPropertyFactory.Class(pi);
                }
                else
                {
                    result = _configurationPropertyFactory.Simple(pi);        
                }
            }

            _hiddenPropertyBagModifier.AddConfigurationProperty(result, _ownerType);
            return result;
        }

        public static PropertyInfo[] GetPublicProperties(Type type)
        {
            if (type.IsInterface)
            {
                var propertyInfos = new List<PropertyInfo>();

                var considered = new List<Type>();
                var queue = new Queue<Type>();
                considered.Add(type);
                queue.Enqueue(type);
                while (queue.Count > 0)
                {
                    var subType = queue.Dequeue();
                    foreach (var subInterface in subType.GetInterfaces())
                    {
                        if (considered.Contains(subInterface)) continue;

                        considered.Add(subInterface);
                        queue.Enqueue(subInterface);
                    }

                    var typeProperties = subType.GetProperties(
                        BindingFlags.FlattenHierarchy
                        | BindingFlags.Public
                        | BindingFlags.Instance);

                    var newPropertyInfos = typeProperties
                        .Where(x => !propertyInfos.Contains(x));

                    propertyInfos.InsertRange(0, newPropertyInfos);
                }

                return propertyInfos.ToArray();
            }

            return type.GetProperties(BindingFlags.FlattenHierarchy
                | BindingFlags.Public | BindingFlags.Instance);
        }
    }
}