using System;
using Root.Records;

namespace Root.Source;

/// <summary>
/// Represents an HTTP client wrapper that manages the lifecycle of an HttpClient instance.
/// Implements the IDisposable pattern to ensure proper resource cleanup.
/// </summary>
/// <remarks>
/// This class encapsulates an HttpClient and a CancellationToken, providing a convenient
/// way to pass both together through the application while ensuring proper disposal.
/// </remarks>
public class HttpSource : IDisposable {

	/// <summary>
	/// Gets the underlying HttpClient instance used for making HTTP requests.
	/// </summary>
	public readonly HttpClient Cli;

	/// <summary>
	/// Gets the CancellationToken used to signal cancellation of asynchronous operations.
	/// </summary>
	public readonly CancellationToken CancToken;

	/// <summary>
	/// Defines the maximum retry attempts and delay between failed HTTP requests.
	/// </summary>
	public readonly HttpRetry Retry;

	/// <summary>
	/// Tracks whether this instance has already been disposed.
	/// </summary>
	private bool _disposed;

	/// <summary>
	/// Initializes a new instance of the <see cref="HttpSource"/> class.
	/// </summary>
	/// <param name="cli">The HttpClient instance to be managed by this wrapper.</param>
	/// <param name="cantToken">The CancellationToken to be used for cancellation signaling.</param>
	/// <param name="retry">Configuration that defines retry behavior for failed HTTP requests.</param>
	public HttpSource(HttpClient cli, CancellationToken cantToken, HttpRetry retry) {
		_disposed = false;
		Cli = cli;
		Retry = retry;
		CancToken = cantToken;
	}

	/// <summary>
	/// Disposes the managed HttpClient instance and marks this instance as disposed.
	/// This method is safe to call multiple times; subsequent calls will have no effect.
	/// </summary>
	public void Dispose() {
		if (_disposed) return;

		Cli.Dispose();
		_disposed = true;
	}
}
