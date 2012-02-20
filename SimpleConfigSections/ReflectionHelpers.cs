using System;
using System.Reflection;
using System.Reflection.Emit;

namespace SimpleConfigSections
{
    internal static class ReflectionHelpers
    {
        private static FieldInfo GetPrivateField(this Type type, string fieldName)
        {
            return type.GetField(fieldName, BindingFlags.Instance | BindingFlags.DeclaredOnly | BindingFlags.NonPublic);
        }

        private static Action<TOWner, TValue> MakeSetter<TOWner, TValue>(this FieldInfo field)
        {
            var m = new DynamicMethod(
                "Set" + field.Name, typeof(void), new Type[] { typeof(TOWner), typeof(TValue) }, typeof(TOWner));
            var cg = m.GetILGenerator();

            // arg0.<field> = arg1
            cg.Emit(OpCodes.Ldarg_0);
            cg.Emit(OpCodes.Ldarg_1);
            cg.Emit(OpCodes.Stfld, field);
            cg.Emit(OpCodes.Ret);

            return (Action<TOWner, TValue>)m.CreateDelegate(typeof(Action<TOWner, TValue>));
        }

        internal static Action<TOwner,TFieldType> MakeSetterForPrivateField<TOwner,TFieldType>(this Type ownerType, string fieldName)
        {
            return ownerType.GetPrivateField(fieldName).MakeSetter<TOwner, TFieldType>();
        }
    }
}