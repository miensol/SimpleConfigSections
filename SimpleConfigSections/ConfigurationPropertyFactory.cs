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
        private static readonly Action<ConfigurationProperty, string> SetAddElementName = CreateFieldSetterDelegate("_addElementName");

        private static readonly Action<ConfigurationProperty, string> SetRemoveElementName= CreateFieldSetterDelegate("_removeElementName");

        private static readonly Action<ConfigurationProperty, string> SetClearElementName= CreateFieldSetterDelegate("_clearElementName");

        internal ConfigurationProperty Collection(PropertyInfo propertyInfo, Type elementType)
        {
            var propertyName = propertyInfo.Name;
            
            var collectionType = typeof (ConfigurationElementCollectionForInterface<>).MakeGenericType(elementType);
            
            var namingConvention = NamingConvention.Current;

            var property = new ConfigurationProperty(propertyName,
                                                     collectionType,
                                                     GetDefaultValue(propertyInfo),
                                                     null,
                                                     GetValidator(propertyInfo),
                                                     GetOptions(propertyInfo));

            SetAddElementName(property, namingConvention.AddToCollectionElementName(elementType, propertyName));
            SetRemoveElementName(property, namingConvention.RemoveFromCollectionElementName(elementType, propertyName));
            SetClearElementName(property, namingConvention.ClearCollectionElementName(elementType, propertyName));
            
            return property;
        }

        internal ConfigurationProperty Interface(PropertyInfo pi)
        {
            return NewConfigurationProperty(pi, 
                typeof(ConfigurationElementForInterface<>).MakeGenericType(pi.PropertyType));
          
        }

        internal ConfigurationProperty Simple(PropertyInfo pi)
        {
            return NewConfigurationProperty(pi, pi.PropertyType);
        }

        private ConfigurationProperty NewConfigurationProperty(PropertyInfo pi, Type elementType)
        {
            var options = GetOptions(pi);
            var defaultValue = GetDefaultValue(pi);
            var validator = GetValidator(pi);
            return new ConfigurationProperty(pi.Name, elementType, defaultValue,
                TypeDescriptor.GetConverter(elementType), validator, options);
        }

        private static ConfigurationPropertyOptions GetOptions(PropertyInfo pi)
        {
            var options = ConfigurationPropertyOptions.None;
            if (pi.HasAttribute<RequiredAttribute>())
            {
                options = ConfigurationPropertyOptions.IsRequired;
            }
            return options;
        }

        private static object GetDefaultValue(PropertyInfo pi)
        {
            object defaultValue = null;
            if (pi.HasAttribute<DefaultAttribute>())
            {
                defaultValue = pi.GetAttribute<DefaultAttribute>().Default();
            }
            return defaultValue;
        }

        private static ConfigurationValidatorBase GetValidator(PropertyInfo pi)
        {
            ConfigurationValidatorBase validator = new DefaultValidator();
            if (pi.HasAttribute<ValidationAttribute>())
            {
                validator = new CompositeConfigurationValidator(pi.GetAttributes<ValidationAttribute>(), pi.Name);
            }
            return validator;
        }

        private static Action<ConfigurationProperty, string> CreateFieldSetterDelegate(string fieldName)
        {
            return typeof (ConfigurationProperty).MakeSetterForPrivateField<ConfigurationProperty, string>(fieldName);
        }
    }
}