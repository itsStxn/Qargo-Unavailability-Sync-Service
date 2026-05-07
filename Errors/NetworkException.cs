using System;
using System.Net;

namespace Root.Errors;

/// <summary>
/// Represents an exception that occurs during network or HTTP operations.
/// Includes optional metadata such as HTTP status code and retry timing
/// to support resilient request handling and diagnostics.
/// </summary>
public class NetworkException : AppException {

	/// <summary>
	/// The HTTP status code returned by the failed request, if available.
	/// </summary>
	public HttpStatusCode? StatusCode { get; }

	/// <summary>
	/// The suggested delay before retrying the failed operation, if provided.
	/// </summary>
	public TimeSpan? RetryAfter { get; }


	/// <summary>
	/// Initializes a new instance of <see cref="NetworkException"/>
	/// with the specified error message and optional HTTP metadata.
	/// </summary>
	/// <param name="message">
	/// The error message describing the network failure.
	/// </param>
	/// <param name="statusCode">
	/// The optional HTTP status code associated with the failure.
	/// </param>
	/// <param name="retryAfter">
	/// The optional retry delay suggested by the server or client logic.
	/// </param>
	public NetworkException(
		string message,
		HttpStatusCode? statusCode = null,
		TimeSpan? retryAfter = null
	) : base(message) {
		StatusCode = statusCode;
		RetryAfter = retryAfter;
	}


	/// <summary>
	/// Initializes a new instance of <see cref="NetworkException"/>
	/// with the specified error message, inner exception, and optional HTTP metadata.
	/// </summary>
	/// <param name="message">
	/// The error message describing the network failure.
	/// </param>
	/// <param name="inner">
	/// The exception that caused the current network exception.
	/// </param>
	/// <param name="statusCode">
	/// The optional HTTP status code associated with the failure.
	/// </param>
	/// <param name="retryAfter">
	/// The optional retry delay suggested by the server or client logic.
	/// </param>
	public NetworkException(
		string message,
		Exception? inner,
		HttpStatusCode? statusCode = null,
		TimeSpan? retryAfter = null
	) : base(message, inner) {
		StatusCode = statusCode;
		RetryAfter = retryAfter;
	}
}
