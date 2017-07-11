namespace Terrasoft.Configuration.Lua
{
	using System;
	using MoonSharp.Interpreter;
	using MoonSharp.Interpreter.Interop;

	public static class MoonsharpExtensions
	{
		static MoonsharpExtensions() {
			UserData.RegistrationPolicy = InteropRegistrationPolicy.Automatic;
			UserData.RegisterExtensionType(typeof(UserDataExtensions));
		}

		public static T ToObject<T>(this DynValue value) {
			return (T)value.ToObject(typeof(T));
		}

		public static void RegisterType<T>() {
			RegisterType(typeof(T));
		}

		public static void RegisterType(Type type) {
			if (!UserData.IsTypeRegistered(type)) {
				UserData.RegisterType(type);
			}
		}

		public static void UnregisterType<T>() {
			UnregisterType(typeof(T));
		}

		public static void UnregisterType(Type type) {
			if (!UserData.IsTypeRegistered(type)) {
				UserData.UnregisterType(type);
			}
		}
	}
}
