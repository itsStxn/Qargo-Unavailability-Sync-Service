using System;
using Serilog;

namespace Root.Core;

/// <summary>
/// Provides a base class with structured logging and message formatting capabilities.
/// All services that require consistent log output and message tagging should inherit from this class.
/// </summary>
public class Base {	
	
	private readonly ILogger Logger;

	/// <summary>The name of the service, used as a prefix in log messages.</summary>
	protected string _name;


	/// <summary>
	/// Initializes a new instance of <see cref="Base"/> with the service name set to <c>"None"</c>.
	/// </summary>
	public Base() {
		Logger = Log.ForContext(GetType());
		_name = "None";
	}

	/// <summary>
	/// Initializes a new instance of <see cref="Base"/> with a specified service name.
	/// </summary>
	/// <param name="name">The name of the service to include in log message prefixes.</param>
	protected Base(string name) {
		Logger = Log.ForContext(GetType());
		_name = name;
	}


	/// <summary>
	/// Formats a message with the service name prefix, unless the message is already formatted.
	/// </summary>
	/// <param name="text">The raw message text to format.</param>
	/// <returns>
	/// The original trimmed message if it already starts with <c>"(service: "</c>;
	/// otherwise, a formatted string in the form <c>"(service: {name}) >> {text}"</c>.
	/// </returns>
	public string Msg(string text) {
		string trimmed = text.Trim();
		if (trimmed.StartsWith("(service: ")) return trimmed;
		return $"(service: {_name}) >> {trimmed}";
	}

	/// <summary>
	/// Logs a formatted informational message using the current service context.
	/// </summary>
	/// <param name="text">The message to log.</param>
	public void Echo(string text) {
		Logger.Information(Msg(text));
	}

	/// <summary>
	/// Logs a formatted warning message using the current service context.
	/// </summary>
	/// <param name="text">The message to log.</param>
	public void Warn(string text) {
		Logger.Warning(Msg(text));
	}

	/// <summary>
	/// Logs a formatted error message using the current service context.
	/// </summary>
	/// <param name="text">The message to log.</param>
	public void Error(string text) {
		Logger.Error(Msg(text));
	}
}
