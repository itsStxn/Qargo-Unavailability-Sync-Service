using Polly;
using System;
using Root.Source;
using Polly.Retry;
using System.Text.Json;
using Root.Core.Interfaces;
using System.Net.Http.Json;

namespace Root.Core;

public class MyRequest : IMyRequest {
	protected readonly RequestSource _rs;
	private readonly AsyncRetryPolicy _retryPolicy;

	public MyRequest(RequestSource rs) {
		_rs = rs;
		_retryPolicy = Policy
			.Handle<HttpRequestException>()
			.WaitAndRetryAsync(
				retryCount: _rs.MaxAttempts,
				onRetryAsync: async (ex, delay) => 
					await OnRetryAsync(ex, delay),
				sleepDurationProvider: attempt => {
					var delayMs = Math.Min(1000, 50 * Math.Pow(2, attempt - 1)); // ? Capped at 1s
					return TimeSpan.FromMilliseconds(delayMs);
				}
			);
	}

	protected string BuildFullUri(Uri? path) {
		if (_rs.Client.BaseAddress == null)
			throw new Exception("The base address of HttpClient is null");

		if (path == null || path.ToString().Trim() == string.Empty)
			throw new Exception("The URI in HttpRequestMessage is empty or null");

		return new Uri(_rs.Client.BaseAddress, path).ToString().Trim();
	}

	protected virtual Task<HttpRequestException> OnRetryAsync(Exception ex, TimeSpan delay) {
		if (ex is not HttpRequestException httpEx)
			throw new Exception("Polly nuget package issue: expected HttpRequestException was not caught");

		Console.WriteLine($"SendAsync HTTP error, retrying after {delay.TotalSeconds}s: {ex.Message}");
		return Task.FromResult(httpEx);
	}

	protected virtual async Task<T?> TrySendAsync<T>(HttpRequestMessage req) {
		string fullUri() => BuildFullUri(req.RequestUri);
		HttpResponseMessage res;

		// ? Get http response
		try {
			res = await _rs.Client.SendAsync(req, _rs.Ct);
			res.EnsureSuccessStatusCode();
		}
		catch (HttpRequestException ex) {
			throw new Exception(
				$"Request to {fullUri()} failed: {ex.StatusCode} {ex.Message}", ex);
		}

		// ? Extract json data
		try {
			var data = await res.Content.ReadFromJsonAsync<T>()
				?? throw new Exception($"Response from {fullUri()} deserialized to null");
			Console.WriteLine($"Successful response from {fullUri()}");
			return data;
		}
		catch (JsonException ex) {
			throw new Exception($"Failed to deserialize response from {fullUri()}", ex);
		}
	}

	public async Task<T?> SendAsync<T>(HttpRequestMessage req) {
		return await _retryPolicy.ExecuteAsync(async () => await TrySendAsync<T>(req));
	}
}
