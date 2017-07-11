namespace Terrasoft.Configuration.Lua
{
	using System;
	using System.IO;
	using MoonSharp.Interpreter;

	/// <summary>
	/// Lua script wrapper.
	/// </summary>
	/// <remarks>May be executed multiple times without side effects.</remarks>
	public class LuaScript: IScriptSession
	{

		private readonly Script _script;
		public LuaClassFactory Factory { get; private set; }

		/// <summary>
		/// Sets a variable with given name to the script context.
		/// </summary>
		/// <param name="name">Variable name.</param>
		/// <returns>Variable's value.</returns>
		public object this[string name] {
			get {
				return _script.Globals[name];
			}
			set {
				Set(name, value);
			}
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="LuaScript"/> class.
		/// </summary>
		/// <param name="modules">Lua core modules.</param>
		public LuaScript(CoreModules modules) {
			_script = new Script(modules);
			Factory = new LuaClassFactory(); 
			Set("_factory", Factory);

			//TODO: Store it in database.
			Execute(@"
			function new(className, ...)
				return _factory.New(className, ...)
			end

			function static(className)
				return _factory.Static(className);
			end

			function using(namespace)
				return _factory.AddNamespace(namespace);
			end

			function typenameof(object)
				local type = type(object);
				return type == 'userdata' and object.getCLRType().FullName or type
			end

			function foreach(collection, action)
				for i = 0, collection.Count - 1 do
					action(collection[i]);
				end
			end");
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="LuaScript"/> class.
		/// </summary>
		public LuaScript()
			: this(CoreModules.Preset_Default) {
		}

		private void RegisterType(Type type) {
			Factory.RegisterAssembly(type.Assembly);
			MoonsharpExtensions.RegisterType(type);
		}

		private void RegisterType<T>() {
			RegisterType(typeof(T));
		}

		/// <summary>
		/// Adds a namespace where types may be found for being created by <see cref="LuaClassFactory.New"/>.
		/// </summary>
		/// <param name="namespace">Namespace.</param>
		public void AddNamespace(string @namespace) {
			Factory.AddNamespace(@namespace);
		}

		/// <summary>
		/// Sets a variable with given name to the script context.
		/// Forces type registration.
		/// </summary>
		/// <typeparam name="T">Type.</typeparam>
		/// <param name="name">Name.</param>
		/// <param name="value">Value.</param>
		public void Set<T>(string name, T value) {
			RegisterType<T>();
			_script.Globals[name] = value;
		}

		/// <summary>
		/// Gets a variable by given name from the script context.
		/// </summary>
		/// <typeparam name="T">Type.</typeparam>
		/// <param name="name">Name.</param>
		/// <returns>Value.</returns>
		public T Get<T>(string name) {
			var value = _script.Globals[name];
			return (T)value;
		}

		/// <summary>
		/// Executes Lua code.
		/// </summary>
		/// <param name="code">Code.</param>
		public void Execute(string code) {
			_script.DoString(code);
		}

		/// <summary>
		/// Executes Lua code and returns a result.
		/// </summary>
		/// <typeparam name="T">Type of result.</typeparam>
		/// <param name="code">Code.</param>
		/// <returns>Script result.</returns>
		public T Execute<T>(string code) {
			var result = _script.DoString(code);
			return result.ToObject<T>();
		}

		/// <summary>
		/// Executes Lua code from a stream.
		/// </summary>
		/// <param name="stream">Stream.</param>
		public void Execute(Stream stream) {
			_script.DoStream(stream);
		}

		/// <summary>
		/// Executes Lua code from a stream.
		/// </summary>
		/// <typeparam name="T">Type of result.</typeparam>
		/// <param name="stream">Stream code.</param>
		/// <returns>Stream code result.</returns>
		public T Execute<T>(Stream stream) {
			var result = _script.DoStream(stream);
			return result.ToObject<T>();
		}

		/// <summary>
		/// Executes Lua code from a file.
		/// </summary>
		/// <param name="path">Path to the script file.</param>
		public void ExecuteFile(string path) {
			_script.DoFile(path);
		}

		/// <summary>
		/// Executes Lua code from a file.
		/// </summary>
		/// <typeparam name="T">Type of result.</typeparam>
		/// <param name="path">Path to the script file.</param>
		/// <returns>Script result.</returns>
		public T ExecuteFile<T>(string path) {
			var result = _script.DoFile(path);
			return result.ToObject<T>();
		}

		/// <summary>
		/// Disposes current script.
		/// </summary>
		public void Dispose() {
			// TODO: Implement disposing.
		}
	}
}
