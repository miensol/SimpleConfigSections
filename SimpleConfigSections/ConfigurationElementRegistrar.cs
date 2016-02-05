using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;

using SC = System.Configuration;

namespace SimpleConfigSections
{
    internal class ConfigurationElementRegistrar
    {
		private static readonly IDictionary<Type, int> ClassesAlreadyInitialized = new Dictionary<Type, int>();

		private static bool ClassAlreadyRegistered(Type type)
		{
			int num = 0;

			if (ClassesAlreadyInitialized.ContainsKey(type))
			{
				num = ClassesAlreadyInitialized[type];
			}

			ClassesAlreadyInitialized[type] = num++;

			return num > 1;
 		}

		private static Hashtable CreatePropertyBagAccessor()
        {
			var propertyBagField = typeof (ConfigurationElement).GetField("s_propertyBags", BindingFlags.Static | BindingFlags.NonPublic);

			if (PropertyBagAccessor == null) return null;

            var value = (Hashtable)propertyBagField.GetValue(null);
            if(value == null)
            {
                value = new Hashtable();
                propertyBagField.SetValue(null, value);
            }
            return value;
        }

		#region DotNet Register
		private static readonly Hashtable PropertyBagAccessor = CreatePropertyBagAccessor();

		private static void _DotNetRegister(ConfigurationElement element, params ConfigurationProperty[] configurationProperties)
        {
			var ownerType = element.GetType();
			var properties = (SC.ConfigurationPropertyCollection)PropertyBagAccessor[ownerType]; 
            if(properties == null)
            {
                properties = new System.Configuration.ConfigurationPropertyCollection();
                PropertyBagAccessor[ownerType] = properties;
            }
			configurationProperties.ToList().ForEach(x => properties.Add(x));
		}
		#endregion

		#region Mono Register
		private static readonly Type ElementMapType = ReflectElementMapType();
		private static readonly Hashtable ElementMaps = RetrieveElementMapsHashtable();
		private static readonly Func<Type, object> ElementMapCreator = x => Activator.CreateInstance(ElementMapType, new[] { x });
		private static readonly FieldInfo ElementMapPropertiesAccesor = CreateElementMapPropertiesAccessor();
		private static readonly Func<ConfigurationElement, ElementInformation> ElementInformationConstructor = CreateElementInformationConstructor();
		private static readonly FieldInfo ElementInformationAccessor = CreateElementInformationAccessor();

		private static Type ReflectElementMapType()
		{
			var cet = typeof(ConfigurationElement);
			return cet.Assembly.GetType(cet.Namespace + ".ElementMap");
		}

		private static Hashtable RetrieveElementMapsHashtable()
		{
			return ElementMapType.GetField("elementMaps", BindingFlags.Static | BindingFlags.NonPublic).GetValue(null) as Hashtable;
		}

		private static FieldInfo CreateElementMapPropertiesAccessor()
		{
			return ElementMapType.GetField("properties", BindingFlags.Instance | BindingFlags.NonPublic);
		}

		private static Func<ConfigurationElement, ElementInformation> CreateElementInformationConstructor()
		{
			var bflags = BindingFlags.NonPublic | BindingFlags.Instance;
			return x => Activator.CreateInstance(typeof(ElementInformation), bflags, null, new object[] { x, null }, null) as ElementInformation;
		}

		private static FieldInfo CreateElementInformationAccessor()
		{
			return typeof(ConfigurationElement).GetField("elementInfo", BindingFlags.NonPublic | BindingFlags.Instance);
		}

		private static void _MonoRegister(ConfigurationElement element, params ConfigurationProperty[] configurationProperties)
		{
			lock (ElementMaps)
			{
				var ownerType = element.GetType();
				var map = ElementMaps[ownerType];
				if (map == null)
				{
					map = ElementMaps[ownerType] = ElementMapCreator(ownerType);
				}
				var properties = ElementMapPropertiesAccesor.GetValue(map) as SC.ConfigurationPropertyCollection;
				if (properties == null)
				{
					properties = new SC.ConfigurationPropertyCollection();
				}
				ElementMapPropertiesAccesor.SetValue(map, properties);

				configurationProperties.ToList().ForEach(x => properties.Add(x));

				var einfo = ElementInformationConstructor(element);
				ElementInformationAccessor.SetValue(element, einfo);
			}
		}
		#endregion

		private static void _Register(ConfigurationElement element, params ConfigurationProperty[] configurationProperties)
		{
			if (!ReflectionHelpers.RunningOnMono)
			{
				_DotNetRegister(element, configurationProperties);
			}
			else
			{
				_MonoRegister(element, configurationProperties);
			}
		}

		public static void Register(ConfigurationElement element, Type @interface)
		{
			var type = element.GetType();

			if (ClassAlreadyRegistered(type))
				return;

			_Register(element, new ConfigurationPropertyCollection(@interface, type).ToArray());
		}
	}
}