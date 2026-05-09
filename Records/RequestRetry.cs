using System;

namespace Root.Records;

/// <summary>
/// Defines HTTP request retry behavior.
/// </summary>
public record struct HttpRetry {

	/// <summary>
	/// Maximum number of retry attempts for failed requests.
	/// </summary>
	public int MaxAttempts;

	/// <summary>
	/// Delay, in milliseconds, between retry attempts for failed requests.
	/// </summary>
	public int Timeout;
}
