namespace Terrasoft.Configuration.Lua
{
	using System;

	public interface IScriptSession : IDisposable
	{
		void AddNamespace(string @namespace);
		void Execute(string code);
		T Execute<T>(string code);
		T Get<T>(string name);
		void Set<T>(string name, T value);
	}
}