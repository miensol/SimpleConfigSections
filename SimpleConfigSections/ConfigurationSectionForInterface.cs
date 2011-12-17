using System;
using System.Configuration;
using SimpleConfigSections.BasicExtensions;

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
        }


        public new object this[string propertyName]
        {
            get { return base[propertyName]; }
        }

        public Type InterfaceType
        {
            get { return _interfaceType; }
        }

        public object Value(string propName)
        {
            return _clientValueResolver.ClientValue(propName);
        }
        
        protected override void Init()
        {
            new ConfigurationPropertyCollection(InterfaceType).Each(c => Properties.Add(c));
        }
    }
}