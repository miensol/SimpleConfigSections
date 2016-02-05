using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace SimpleConfigSections
{
    internal class ConfigurationElementCollectionForInterface : ConfigurationElementCollection, IConfigValue
    {
        private readonly Type _elementType;
        private readonly Type _listType;
        private readonly CacheCallback<int, IList> _list;
		private readonly ConfigurationElementRegistrar _registrar = ConfigurationElementRegistrar.Instance;

		protected ConfigurationElementCollectionForInterface(Type elementType)
        {
			_elementType = elementType;
            
            _listType = typeof (List<>).MakeGenericType(new[]
                                                            {
                                                                _elementType
                                                            });
            _list = new CacheCallback<int, IList>(ignored=> CreateElements());
        }


        public object Value(string propertyName)
        {
            return _list.Get(0).GetEnumerator();
        }

        private IList CreateElements()
        {
            var elementList = (IList) Activator.CreateInstance(_listType);
            foreach (IConfigValue configValue in this)
            {
                var obj = new ConcreteConfiguration(configValue).ClientValue(_elementType);
                elementList.Add(obj);
            }
            return elementList;
        }


        protected override ConfigurationElement CreateNewElement()
        {            
            var configurationElementForInterface = (ConfigurationElement)Activator.CreateInstance(typeof(ConfigurationElementForInterface<>).MakeGenericType(_elementType));            
            return configurationElementForInterface;
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return Guid.NewGuid();
        }

		protected override void Reset(ConfigurationElement parentElement)
		{
			// XXX: Avoid infinite loop on mono.
			if (!ReflectionHelpers.RunningOnMono)
				base.Reset(parentElement);
		}

		protected override void Init()
		{
			_registrar.Register(this, _elementType);

			base.Init();
		}
	}   

    internal class ConfigurationElementCollectionForInterface<T> : ConfigurationElementCollectionForInterface
    {
        public ConfigurationElementCollectionForInterface()
            : base(typeof (T))
        {
        }
    }
}