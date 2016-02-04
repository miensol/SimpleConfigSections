using System;
using System.Reflection;
using System.Reflection.Emit;

namespace SimpleConfigSections
{
    internal static class ReflectionHelpers
    {
		private static readonly BindingFlags _privateFlags = BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.NonPublic;

		public static bool RunningOnMono
		{
			get
			{
				Type t = Type.GetType("Mono.Runtime");
				if (t != null)
					return true;

				return false;
			}
		}

		internal static FieldInfo GetPrivateField<TType>(string fieldName)
		{
			return GetPrivateField(typeof(TType), fieldName);
		}

		internal static FieldInfo GetPrivateField(Type type, string fieldName)
        {
            return type.GetField(fieldName, _privateFlags);
        }

		internal static PropertyInfo GetPrivateProperty<TType>(string propertyName)
		{
			return typeof(TType).GetProperty(propertyName, _privateFlags);
		}

		internal static Action<TOwner, TFieldType> MakeSetterForPrivateField<TOwner, TFieldType>(string fieldName)
        {
			var field = GetPrivateField<TOwner>(fieldName);
			return field != null ? (obj, value) => field.SetValue(obj, value) : (Action<TOwner, TFieldType>)null;
        }

		internal static Action<TOwner, TFieldType> MakeSetterForPrivateProperty<TOwner, TFieldType>(string propertyName)
		{
			var property = GetPrivateProperty<TOwner>(propertyName);
			return property != null ? (obj, value) => property.SetValue(obj, value) : (Action<TOwner, TFieldType>)null;
		}
	}
}