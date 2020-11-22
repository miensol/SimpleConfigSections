using System;
using System.Runtime.CompilerServices;
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
            if(invocation.Method.DeclaringType.IsInterface ||
                invocation.Method.IsDefined(typeof(CompilerGeneratedAttribute), false)
                )
            {
                var propertyInfo = invocation.Method.GetPropertyInfo();
                invocation.ReturnValue = _configValue.Value(propertyInfo);
            } else
            {
                invocation.Proceed();
            }
        }


        public object ClientValue(Type definingType)
        {
            if(definingType.IsInterface)
            {
                return ProxyGenerator.CreateInterfaceProxyWithoutTarget(definingType, this);    
            }
            if(definingType.IsArray)
            {
                return Array.CreateInstance(definingType.GetElementType(), 0);
            }
            return ProxyGenerator.CreateClassProxy(definingType, this);

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
