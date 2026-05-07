using System;

namespace Root.Errors;

/// <summary>
/// Represents an exception that occurs during parsing or deserialization operations.
/// Typically thrown when input data (such as JSON, cached files, or external payloads)
/// is malformed, invalid, or cannot be converted into the expected structure.
/// </summary>
public class ParseException : AppException {

	/// <summary>
	/// Initializes a new instance of <see cref="ParseException"/>
	/// with the specified error message.
	/// </summary>
	/// <param name="message">
	/// The error message describing the parsing failure.
	/// </param>
	public ParseException(string message) : base(message) { }

	/// <summary>
	/// Initializes a new instance of <see cref="ParseException"/>
	/// with the specified error message and inner exception.
	/// </summary>
	/// <param name="message">
	/// The error message describing the parsing failure.
	/// </param>
	/// <param name="inner">
	/// The exception that caused the current parsing exception.
	/// </param>
	public ParseException(string message, Exception? inner) : base(message, inner) { }
}
