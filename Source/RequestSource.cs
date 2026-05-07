using System;
using Root.Core;

namespace Root.Source;

public class RequestSource : Base, IDisposable {
	public readonly HttpClient Client;
	public readonly CancellationToken Ct;
	public readonly int MaxAttempts;
	private bool _disposed;

	public RequestSource(HttpClient client, CancellationToken ct, int maxAttempts) {
		MaxAttempts = maxAttempts;
		Client = client;
		Ct = ct;
		_disposed = false;
	}

	public void Dispose() {
		if (_disposed) return;

		// ? Release http client
		Client.Dispose();
		_disposed = true;
	}
}
