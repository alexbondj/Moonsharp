namespace Terrasoft.Configuration.Lua
{
	using System;
	using System.Linq;
	using MoonSharp.Interpreter;

	public static class UserDataExtensions
	{
		public static object Generic(this object obj, string methodName,
			Type[] typeParameters, params object[] parameters) {
			var type = obj.GetType();
			var methodInfo = type.GetMethods().First(m =>
				   m.Name == methodName
				&& m.ContainsGenericParameters
				&& m.GetGenericArguments().Length == typeParameters.Length
				&& m.GetParameters().Length == parameters.Length
			);
			var genericMethod = methodInfo.MakeGenericMethod(typeParameters);
			return genericMethod.Invoke(obj, parameters);
		}

		public static Type GetCLRType(this object obj) {
			if (!UserData.IsTypeRegistered<Type>()) {
				UserData.RegisterType<Type>();
			}
			return obj.GetType();
		}
	}
}