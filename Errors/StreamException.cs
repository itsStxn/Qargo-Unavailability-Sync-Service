using System;

namespace Root.Errors;

/// <summary>
/// Represents an exception that occurs during stream or file I/O operations.
/// Typically thrown when reading from or writing to files, streams, or cache storage
/// fails due to I/O errors or access issues.
/// </summary>
public class StreamException : AppException {

	/// <summary>
	/// Initializes a new instance of <see cref="StreamException"/>
	/// with the specified error message.
	/// </summary>
	/// <param name="message">
	/// The error message describing the stream operation failure.
	/// </param>
	public StreamException(string message) : base(message) { }

	/// <summary>
	/// Initializes a new instance of <see cref="StreamException"/>
	/// with the specified error message and inner exception.
	/// </summary>
	/// <param name="message">
	/// The error message describing the stream operation failure.
	/// </param>
	/// <param name="inner">
	/// The exception that caused the current stream exception.
	/// </param>
	public StreamException(string message, Exception? inner) : base(message, inner) { }
}
