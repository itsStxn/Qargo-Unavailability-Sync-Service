using System;
using System.IO;
using Root.DTOs;
using System.Text.Json;
using Root.Utils.Interfaces;

namespace Root.Utils;

public class AccessTokenCache : IAccessTokenCache {
	private readonly string _filePath;
	public JsonSerializerOptions Options;

	public AccessTokenCache(string fileName) {
		_filePath = $"./Cache/{fileName.ToLower()}-access-token.json";
		Options = new JsonSerializerOptions { WriteIndented = true };
	}

	public string Read() {
		// ? Early exit
		if (!File.Exists(_filePath)) {
			return string.Empty;
		}

		// ? Get json string data
		string json;
		try {
			json = File.ReadAllText(_filePath);
		}
		catch (Exception ex) {
			throw new IOException("Failed to read cache file.", ex);
		}

		// ? Parse string data into DTO
		CachedAccessToken? data;
		try {
			data = JsonSerializer.Deserialize<CachedAccessToken>(json);
		}
		catch (Exception ex) {
			throw new InvalidDataException("Cache file contains invalid JSON.", ex);
		}

		// ? Validate data
		if (data == null)
			throw new InvalidDataException("Cache file deserialized to null.");
		return data.AccessToken;
	}

	public void Create(string token) {
		// ? Mount access token
		var data = new CachedAccessToken {
			AccessToken = token
		};

		// ? Rewrite cache
		var json = JsonSerializer.Serialize(data, Options);
		File.WriteAllText(_filePath, json);
	}
}
