using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using Castle.DynamicProxy;
using SimpleConfigSections.BasicExtensions;
using Castle.Core.Internal;

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
			if (invocation.Method.DeclaringType.IsInterface ||
                invocation.Method.HasAttribute<CompilerGeneratedAttribute>()
                )
            {
                var propertyInfo = invocation.Method.GetPropertyInfo();
                var name = propertyInfo == null ?
                    invocation.Method.PropertyName()
                    : NamingConvention.Current.AttributeName(propertyInfo);
                object obj = _configValue.Value(name);
                invocation.ReturnValue = obj;
            } else
            {
                invocation.Proceed();
            }
        }


        public object ClientValue(Type definingType)
        {
			if (definingType.IsInterface)
            {
                return ProxyGenerator.CreateInterfaceProxyWithoutTarget(definingType, this);    
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
