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

        public ConfigurationElementForInterface(Type interfaceType)
        {
            _interfaceType = interfaceType;
            _clientValueResolver = new ClientValueResolver(this, _interfaceType);
        }

        public object Value(string propertyName)
        {
            return _clientValueResolver.ClientValue(propertyName);
        }

        protected override void Init()
        {
            var colection = new ConfigurationPropertyCollection(_interfaceType);
            colection.Each(c => Properties.Add(c));
        }

        public new object this[string propertyName]
        {
            get { return base[propertyName]; }
        }

        public ConfigurationProperty Get(string propertyName)
        {
            return Properties.Cast<ConfigurationProperty>().Single(p => p.Name == propertyName);
        }

    }
}