using System;
using System.Configuration;
using System.Xml;
using System.Linq;

namespace SimpleConfigSections
{
    public class ConfigurationSectionForInterface : ConfigurationSection, IConfigValue, IBaseValueProvider
    {
        private readonly ClientValueResolver _clientValueResolver;
        private readonly Type _interfaceType;

        public ConfigurationSectionForInterface(Type interfaceType)
        {
			_interfaceType = interfaceType;
            _clientValueResolver = new ClientValueResolver(this, InterfaceType);

			Init();
        }


        public new object this[string propertyName]
        {
            get { return base[propertyName]; }
        }

        public Type InterfaceType
        {
            get { return _interfaceType; }
        }

        public object Value(string propertyName)
        {
            return _clientValueResolver.ClientValue(propertyName);
        }

		protected override void Reset(ConfigurationElement parentElement)
		{
			if (!ReflectionHelpers.RunningOnMono)
				base.Reset(parentElement);
		}

		protected override void Init()
		{
			ConfigurationElementRegistrar.Register(this, _interfaceType);

			base.Init();
		}

		protected override void DeserializeElement(XmlReader reader, bool serializeCollectionKey)
		{
			base.DeserializeElement(reader, serializeCollectionKey);

			// XXX: Mono has a bug were it does not correctly
			//		asserts IsRequired on section's attributes.
			if (ReflectionHelpers.RunningOnMono)
			{
				var missingProperties = ElementInformation.Properties
					.OfType<PropertyInformation>()
					.Where(x => x.IsRequired && x.Value == null)
					.Select(x => x.Name);

				if (missingProperties.Any())
					OnRequiredPropertyNotFound(missingProperties.First());
			}
		}
	}
}