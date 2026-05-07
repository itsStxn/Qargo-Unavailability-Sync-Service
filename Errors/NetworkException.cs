using System;
using System.Net;

namespace Root.Errors;

public class NetworkException : AppException {
	public HttpStatusCode? StatusCode { get; }
	public TimeSpan? RetryAfter { get; }

	public NetworkException(string message, HttpStatusCode? statusCode = null, TimeSpan? retryAfter = null)
	: base(message) {
		StatusCode = statusCode;
		RetryAfter = retryAfter;
	}

	public NetworkException(string message, Exception? inner, HttpStatusCode? statusCode = null, TimeSpan? retryAfter = null)
	: base(message, inner) {
		StatusCode = statusCode;
		RetryAfter = retryAfter;
	}
}
