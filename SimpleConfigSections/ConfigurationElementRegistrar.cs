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

		private static void _DotNetRegister(Type ownerType, params ConfigurationProperty[] configurationProperties)
        {
			var properties = (System.Configuration.ConfigurationPropertyCollection)PropertyBagAccessor[ownerType]; 
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

		private static void _MonoRegister(Type ownerType, params ConfigurationProperty[] configurationProperties)
		{
			var log = typeof(ConfigurationSection).IsAssignableFrom(ownerType);

			if (log)
			{
				Console.Error.WriteLine("\n ==>>>>>>>>>> Registering Section: {0}", ownerType.FullName);
			}

			//lock (ElementMaps)
			{
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
				if (log && properties.OfType<ConfigurationProperty>().Any())
					Console.Error.WriteLine(" ==>>>>>>>>>> Creating properties!! ({0})", properties.OfType<ConfigurationProperty>().First().Validator);
				ElementMapPropertiesAccesor.SetValue(map, properties);

				configurationProperties.ToList().ForEach(x => properties.Add(x));
			}
		}
		#endregion

		private static void _Register(ConfigurationElement element, params ConfigurationProperty[] configurationProperties)
		{
			if (!ReflectionHelpers.RunningOnMono)
			{
				_DotNetRegister(element.GetType(), configurationProperties);
			}
			else
			{
				_MonoRegister(element.GetType(), configurationProperties);

#if false
				PropertyInformation propInfo = null;
				if (element is ConfigurationSection)
				{
					var configProp = ConfigurationPropertyFactory.Create().Section(element.GetType());
					propInfo = Activator.CreateInstance(typeof(PropertyInformation),
						BindingFlags.NonPublic | BindingFlags.Instance,
						null, new object[] { element, configProp }, null) as PropertyInformation;
				}
#endif
				var einfo = Activator.CreateInstance(typeof(ElementInformation),
					BindingFlags.NonPublic | BindingFlags.Instance,
					null, new object[] { element, /*propInfo*/ null }, null);
				typeof(ConfigurationElement).GetField("elementInfo",
					BindingFlags.NonPublic | BindingFlags.Instance)
					.SetValue(element, einfo);
				//typeof(ConfigurationElement).GetField("elementProperty",
				//	BindingFlags.NonPublic | BindingFlags.Instance)
				//	.SetValue(element, null); // Reset internal map field.
			}
		}

		public static void Register(ConfigurationElement element, Type @interface)
		{
			var type = element.GetType();

			if (ClassAlreadyRegistered(type))
				return;

			var collection = new ConfigurationPropertyCollection(@interface, type).ToArray();

			_Register(element, collection);
		}
	}
}