using System;

namespace Root.Errors;

/// <summary>
/// Represents an exception that occurs due to invalid or missing configuration values.
/// Typically thrown when required environment variables, settings, or runtime configuration
/// data are not present or are incorrectly formatted.
/// </summary>
public class ConfigException : AppException {

	/// <summary>
	/// Initializes a new instance of <see cref="ConfigException"/>
	/// with the specified error message.
	/// </summary>
	/// <param name="message">
	/// The error message describing the configuration issue.
	/// </param>
	public ConfigException(string message) : base(message) { }

	/// <summary>
	/// Initializes a new instance of <see cref="ConfigException"/>
	/// with the specified error message and inner exception.
	/// </summary>
	/// <param name="message">
	/// The error message describing the configuration issue.
	/// </param>
	/// <param name="inner">
	/// The exception that caused the current configuration exception.
	/// </param>
	public ConfigException(string message, Exception? inner) : base(message, inner) { }
}
