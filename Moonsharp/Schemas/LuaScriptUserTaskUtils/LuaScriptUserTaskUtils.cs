using Terrasoft.Common;

namespace Terrasoft.Configuration.Lua {
	using System;
	using System.Collections.Concurrent;
	using System.Linq;
	using System.Text;
	using System.Reflection;
	using Terrasoft.Core;
	using Terrasoft.Core.Process;

	public class ProcessModel
	{
		private readonly Process _process;

		private static readonly ConcurrentDictionary<Type, MethodInfo> _setValueMethods =
			new ConcurrentDictionary<Type, MethodInfo>();
		private static readonly ConcurrentDictionary<Type, MethodInfo> _getValueMethods =
			new ConcurrentDictionary<Type, MethodInfo>();

		private static MethodInfo GetSetValueMethod(Type type) {
			MethodInfo method;
			if (!_setValueMethods.TryGetValue(type, out method)) {
				method = FindProcessMethod(type, "SetParameterValue");
				_setValueMethods[type] = method;
			}
			return method;
		}
		private static MethodInfo GetGetValueMethod(Type type) {
			MethodInfo method;
			if (!_getValueMethods.TryGetValue(type, out method)) {
				method = FindProcessMethod(type, "GetParameterValue");
				_getValueMethods[type] = method;
			}
			return method;
		}

		private static MethodInfo FindProcessMethod(Type type, string methodName) {
			MethodInfo method = typeof(Process)
				.GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
				.Where(mi=>mi.Name.Equals(methodName, StringComparison.OrdinalIgnoreCase))
				.First(mi=>mi.IsGenericMethod);
			return method.MakeGenericMethod(type);
		}

		public ProcessModel(Process process) {
			_process = process;
		}

		public object get(string parameterName) {
			MethodInfo method = GetGetValueMethod(typeof(object));
			return method.Invoke(_process, new[] { parameterName});
		}

		public void set(string parameterName, object parameterValue) {
			var schema = (BaseProcessSchema)_process.Schema;
			ProcessSchemaParameter parameter = schema.Parameters.GetByName(parameterName);
			Type valueType = parameter.DataValueType.ValueType;
			object value = DataTypeUtilities.ValueAsType(parameterValue, valueType);
			MethodInfo method = GetSetValueMethod(valueType);
			method.Invoke(_process, new[] { parameterName, value });
		}
	}

	public class LuaScriptExecutor
	{
		public static bool Execute(string base64Script, Process owner) {
			var session = new LuaScript();
			foreach (SchemaUsing item in owner.Schema.Usings) {
				session.AddNamespace(item.Name);
			}
			var model = new ProcessModel(owner);
			session.Set("Process", model);
			session.Set("UserConnection", owner.UserConnection);
			string script = Encoding.UTF8.GetString(Convert.FromBase64String(base64Script));
			return session.Execute<bool>(script);
		}

	}

}