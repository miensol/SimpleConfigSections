using System;
using Castle.DynamicProxy;
using SimpleConfigSections.BasicExtensions;

namespace SimpleConfigSections
{
    internal class ConcreteConfiguration : IInterceptor
    {
        private readonly IConfigValue _configValue;
        private static readonly ProxyGenerator ProxyGenerator = new ProxyGenerator();

        public ConcreteConfiguration(IConfigValue configValue)
        {
            _configValue = configValue;
        }


        public void Intercept(IInvocation invocation)
        {
            object obj = _configValue.Value(invocation.Method.PropertyName());
            invocation.ReturnValue = obj;
        }


        public object ClientValue(Type interfaceType)
        {
            return ProxyGenerator.CreateInterfaceProxyWithoutTarget(interfaceType, this);
        }
    }

    internal class ConcreteConfiguration<T> : ConcreteConfiguration where T : class
    {
        public ConcreteConfiguration(ConfigurationSection<T> section)
            : base(section)
        {
        }

        public T ClientValue()
        {
            return (T)ClientValue(typeof(T));
        }
    }
}