using System;

namespace Root.Errors;

public class StreamException : AppException {
	public StreamException(string message) : base(message) { }
	public StreamException(string message, Exception? inner) : base(message, inner) { }
}
