using System;

namespace Root.Errors;

public class ConfigException : AppException {
	public ConfigException(string message) : base(message) { }
	public ConfigException(string message, Exception? inner) : base(message, inner) { }
}
