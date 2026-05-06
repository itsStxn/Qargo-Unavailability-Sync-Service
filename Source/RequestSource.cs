using System;

namespace Root.Source;

public class RequestSource {
	public readonly HttpClient Client;
	public readonly CancellationToken Ct;
	public readonly int MaxAttempts;

	public RequestSource(HttpClient client, CancellationToken ct, int maxAttempts) {
		MaxAttempts = maxAttempts;
		Client = client;
		Ct = ct;
	}
}
