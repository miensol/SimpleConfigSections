using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Reflection;
using Castle.Core.Internal;

namespace SimpleConfigSections
{
    internal class ConfigurationPropertyFactory
    {
        public ConfigurationProperty Collection(string propertyName, Type elementType)
        {
            return new ConfigurationProperty(propertyName,
                                             typeof (ConfigurationElementCollectionForInterface<>).
                                                 MakeGenericType(elementType));
        }

        public ConfigurationProperty Interface(PropertyInfo pi)
        {
            return NewConfigurationProperty(pi, typeof(ConfigurationElementForInterface<>).MakeGenericType(
                pi.PropertyType));
          
        }

        public ConfigurationProperty Simple(PropertyInfo pi)
        {
            return NewConfigurationProperty(pi, pi.PropertyType);

        }

        private ConfigurationProperty NewConfigurationProperty(PropertyInfo pi, Type elementType)
        {
            var options = ConfigurationPropertyOptions.None;
            object defaultValue = null;
            if(pi.HasAttribute<RequiredAttribute>())
            {
                options =  ConfigurationPropertyOptions.IsRequired;
            }
            if(pi.HasAttribute<DefaultAttribute>())
            {
                defaultValue  = pi.GetAttribute<DefaultAttribute>().Default();
            }
            ConfigurationValidatorBase validator = new DefaultValidator();
            if(pi.HasAttribute<ValidationAttribute>())
            {
                validator = new CompositeConfigurationValidator(pi.GetAttributes<ValidationAttribute>(), pi.Name);
            }
            return new ConfigurationProperty(pi.Name, elementType, defaultValue,
                TypeDescriptor.GetConverter(elementType), validator, options);
        }
    }
}