using System;
using System.Text.Json.Serialization;

namespace Root.DTOs;

public class CachedAccessToken {
	[JsonPropertyName("access_token")]
	public required string AccessToken { get; set; }
}
