namespace Terrasoft.Configuration
{
	using System;
	using System.Collections.Generic;
	using System.Linq;
	using System.Reflection;
	using MoonSharp.Interpreter;
	using Terrasoft.Common;
	using Terrasoft.Configuration.Lua;
	using Terrasoft.Core;

	/// <summary>
	/// Class factory for Lua environment.
	/// It allows to manipulate assemblies, namespaces
	/// and to create CLR type instances from the Lua script context.
	/// </summary>
	public class LuaClassFactory
	{
		private readonly List<string> _namespaces = new List<string>();
		private readonly List<Assembly> _assemblies = new List<Assembly> {
			typeof(Environment).Assembly,	//System
			typeof(AppConnection).Assembly,	//Terrasoft.Core
			typeof(BaseEntity).Assembly,	//Terrasoft.Configuration
			typeof(BaseResource).Assembly	//Terrasoft.Common
		};

		private string GetFullTypeName(string @namespace, string shortTypeName) {
			return string.Join(".", @namespace, shortTypeName);
		}

		private Type GetType(string shortTypeName) {
			foreach (var assembly in _assemblies) {
				foreach (var @namespace in _namespaces) {
					string typeName = GetFullTypeName(@namespace, shortTypeName);
					Type type = assembly.GetType(typeName);
					if (type != null)
						return type;
				}
			}
			throw new ArgumentException(string.Format("Unable to find type {0}.", shortTypeName));
		}

		/// <summary>
		/// Constructor wrapper which may be used by Lua script to create a CLR type.
		/// </summary>
		/// <param name="shortTypeName">Class short name, e.g. "UserConnection".</param>
		/// <param name="args">Constructor arguments.</param>
		/// <returns>New CLR object instance.</returns>
		public object New(string shortTypeName, params object[] args) {
			Type type = GetType(shortTypeName);
			var instance = args.Any(a => a != null)
				? Activator.CreateInstance(type, args)
				: Activator.CreateInstance(type);
			MoonsharpExtensions.RegisterType(type);
			return instance;
		}

		/// <summary>
		/// Creates a static class 'instance' to get an access to static methods.
		/// </summary>
		/// <param name="shortTypeName">Class short name, e.g. "UserConnection"</param>
		/// <returns>Static class 'instance'.</returns>
		public DynValue Static(string shortTypeName) {
			Type type = GetType(shortTypeName);
			return UserData.CreateStatic(type);
		}

		/// <summary>
		/// Adds a namespace where types may be found for being created by <see cref="New"/>.
		/// </summary>
		/// <param name="namespace">Namespace.</param>
		public void AddNamespace(string @namespace) {
			_namespaces.AddIfNotExists(@namespace);
		}

		/// <summary>
		/// Registers an assembly where types may be found for being created by <see cref="New"/>.
		/// WARNING: Try to avoid large assemblies.
		/// </summary>
		/// <typeparam name="WKT">Well known type of an assembly.</param>
		public void RegisterAssembly<WKT>() {
			RegisterAssembly(typeof(WKT).Assembly);
		}

		/// <summary>
		/// Registers an assembly where types may be found for being created by <see cref="New"/>.
		/// WARNING: Try to avoid large assemblies.
		/// </summary>
		/// <param name="assembly">Assembly.</param>
		public void RegisterAssembly(Assembly assembly) {
			_assemblies.AddIfNotExists(assembly);
		}
	}
}