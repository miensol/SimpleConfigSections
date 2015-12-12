using System;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using Castle.DynamicProxy.Contributors;

namespace SimpleConfigSections
{
    internal static class ReflectionHelpers
    {
        private static FieldInfo GetPrivateField(this Type type, string fieldName)
        {
            return type.GetField(fieldName, BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.NonPublic);
        }

        private delegate void ByRefStructAction<TTarget, TValue>(ref TTarget instance, TValue value);

        private static Action<TTarget, TValue> MakeSetter<TTarget, TValue>(this FieldInfo field)
        {
            var instance = Expression.Parameter(typeof(TTarget).MakeByRefType(), "instance");
            var value = Expression.Parameter(typeof(TValue), "value");

            var binaryExpression = Expression.Assign(
                Expression.Field(instance, field),
                Expression.Convert(value, field.FieldType)
                );
            var refStructAction = Expression.Lambda<ByRefStructAction<TTarget, TValue>>(binaryExpression, instance, value).Compile();
            var byRefStructAction = refStructAction;
            return (owner, value1) =>
            {
                TTarget target = owner; 
                byRefStructAction.Invoke(ref target, value1); 
            };
        }

        internal static Action<TOwner,TFieldType> MakeSetterForPrivateField<TOwner,TFieldType>(this Type ownerType, string fieldName)
        {
            return ownerType.GetPrivateField(fieldName).MakeSetter<TOwner, TFieldType>();
        }
    }
}