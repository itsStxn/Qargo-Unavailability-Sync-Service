using System;
using Root.DTOs;
using Root.Utils;
using System.Net;
using System.Text;
using Root.Source;
using System.Net.Http.Json;
using System.Net.Http.Headers;

namespace Root.Core;

public class MyAuthRequest : MyRequest {
	private readonly string _clientId;
	private readonly string _secret;
	private readonly string _name;
	private string _accessToken;
	private readonly AccessTokenCache _atc;

	public MyAuthRequest(string name, string clientId, string secret, RequestSource rs) : base(rs) {
		_atc = new AccessTokenCache(name);
		_accessToken = _atc.Read();
		_clientId = clientId;
		_secret = secret;
		_name = name;
	}

	private async Task ReNewAccessTokenAsync() {
		int attempt = 0;
		while (attempt < _rs.MaxAttempts) {
			// ? Build request
			var req = new HttpRequestMessage(HttpMethod.Post, "auth/token");
			Console.WriteLine($"(Attempt no {++attempt}) Fetching access token at {BuildFullUri(req.RequestUri)}");

			var cred = Convert.ToBase64String(Encoding.UTF8.GetBytes($"{_clientId}:{_secret}"));
			req.Headers.Authorization = new AuthenticationHeaderValue("Basic", cred);
			req.Content = JsonContent.Create(new { grant_type = "client_credentials" });

			// ? Get and validate response
			var res = await _rs.Client.SendAsync(req, _rs.Ct);

			if (res.StatusCode == HttpStatusCode.TooManyRequests) {
				var retryAfter = res.Headers.RetryAfter?.Delta ?? TimeSpan.FromSeconds(60);
				Console.WriteLine($"Rate limited, retrying after {retryAfter.TotalSeconds}s...");
				await Task.Delay(retryAfter, _rs.Ct);
				continue; // ? retry
			}

			res.EnsureSuccessStatusCode();

			// ? Get new access token
			var data = await res.Content.ReadFromJsonAsync<AccessTokenResponse>(_rs.Ct)
				?? throw new Exception("Failed to deserialize access token response");
			_accessToken = data.AccessToken.Trim();

			// ? Validate and cache access token
			if (_accessToken.Length == 0)
				throw new Exception("Access token is unexpectedly an empty string");
			_atc.Create(_accessToken);
			return;
		}
	}

	protected override async Task<HttpRequestException> OnRetryAsync(Exception ex, TimeSpan delay) {
		var httpEx = await base.OnRetryAsync(ex, delay);

		// ? Inject access token if necessary
		if (httpEx.StatusCode == HttpStatusCode.Unauthorized) {
			await ReNewAccessTokenAsync();
		}

		return httpEx;
	}

	protected override async Task<T?> TrySendAsync<T>(HttpRequestMessage req) where T: default {
		// ? Force pollying
		if (_accessToken == string.Empty) {
			throw new HttpRequestException(
				"Access token is empty",
				inner: null,
				statusCode: HttpStatusCode.Unauthorized
			);
		}

		// ? Set access token
		req.Headers.Authorization = 
			new AuthenticationHeaderValue("Bearer", _accessToken);

		return await base.TrySendAsync<T>(req);
	}
}
