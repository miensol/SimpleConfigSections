using System;
using System.Configuration;
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

        protected override void Init()
        {
            new ConfigurationPropertyCollection(_interfaceType, GetType()).ToList();
            base.Init();
        }        
    }
}