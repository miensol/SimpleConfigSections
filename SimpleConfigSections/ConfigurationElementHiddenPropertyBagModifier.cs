using System;
using System.Collections;
using System.Configuration;
using System.Reflection;

namespace SimpleConfigSections
{
    internal class ConfigurationElementHiddenPropertyBagModifier
    {
        private static readonly Hashtable PropertyBagAccessor = CreatePropertyBagAccessor();

        private static Hashtable CreatePropertyBagAccessor()
        {
            var propertyBagField = typeof (ConfigurationElement).GetField("s_propertyBags",
                                                                          BindingFlags.Static | BindingFlags.NonPublic);
            var value = (Hashtable)propertyBagField.GetValue(null);
            return value;
        }

        public void AddConfigurationProperty(ConfigurationProperty configurationProperty, Type ownerType)
        {
            var properties = (System.Configuration.ConfigurationPropertyCollection)PropertyBagAccessor[ownerType]; 
            if(properties == null)
            {
                properties = new System.Configuration.ConfigurationPropertyCollection();
                PropertyBagAccessor[ownerType] = properties;
            }
            properties.Add(configurationProperty);
        }
    }
}