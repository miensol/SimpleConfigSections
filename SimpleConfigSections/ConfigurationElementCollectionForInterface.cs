using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;

namespace SimpleConfigSections
{
    internal class ConfigurationElementCollectionForInterface : ConfigurationElementCollection, IConfigValue
    {
        private readonly Type _elementType;

        protected ConfigurationElementCollectionForInterface(Type elementType)
        {
            _elementType = elementType;
        }


        public object Value(string proprName)
        {
            var list = (IList) Activator.CreateInstance(typeof (List<>).MakeGenericType(new[]
                                                                                              {
                                                                                                  _elementType
                                                                                              }));
            foreach (IConfigValue configValue in this)
            {
                var obj = new ConcreteConfiguration(configValue).ClientValue(_elementType);
                list.Add(obj);
            }
            return list.GetEnumerator();
        }


        protected override ConfigurationElement CreateNewElement()
        {
            return new ConfigurationElementForInterface(_elementType);
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return Guid.NewGuid();
        }
    }

    internal class ConfigurationElementCollectionForInterface<T> : ConfigurationElementCollectionForInterface
    {
        public ConfigurationElementCollectionForInterface()
            : base(typeof (T))
        {
        }
    }
}