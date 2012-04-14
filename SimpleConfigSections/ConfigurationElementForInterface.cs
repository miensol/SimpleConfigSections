using System;
using System.Configuration;
using SimpleConfigSections.BasicExtensions;
using System.Linq;

namespace SimpleConfigSections
{
    internal class ConfigurationElementForInterface<T> : ConfigurationElementForInterface
    {
        public ConfigurationElementForInterface():base(typeof(T))
        {
            
        }
    }

    internal class ConfigurationElementForInterface : ConfigurationElement, IConfigValue, IBaseValueProvider
    {
        private readonly Type _interfaceType;
        private readonly ClientValueResolver _clientValueResolver;

        protected ConfigurationElementForInterface(Type interfaceType)
        {
            _interfaceType = interfaceType;
            _clientValueResolver = new ClientValueResolver(this, _interfaceType);
        }

        public object Value(string propertyName)
        {
            return _clientValueResolver.ClientValue(propertyName);
        }

        public new object this[string propertyName]
        {
            get { return base[propertyName]; }
        }

        protected override void Init()
        {
            new ConfigurationPropertyCollection(_interfaceType, GetType()).ToList();
            base.Init();
        }
    }
}