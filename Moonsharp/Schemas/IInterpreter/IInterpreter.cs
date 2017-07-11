namespace Terrasoft.Configuration.Assistant
{
	using Terrasoft.Configuration.Lua;

	public interface IInterpreter {
		IScriptSession CreateSession();
	}
}