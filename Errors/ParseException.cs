using System;

namespace Root.Errors;

public class ParseException : AppException {
	public ParseException(string message) : base(message) { }
	public ParseException(string message, Exception? inner) : base(message, inner) { }
}
