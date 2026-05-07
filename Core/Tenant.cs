using System;
using Root.DTOs;
using Root.Source;
using Root.Errors;
using Root.Core.Interfaces;
using Root.DTOs.ResourceListComponents;

namespace Root.Core;

public class Tenant : Base, ITenant {
	private readonly MyAuthRequest _auth;

	public Tenant(string name, string clientId, string secret, RequestSource rs) {
		_auth = new MyAuthRequest(name, clientId, secret, rs);
		_name = name;
	}

	public async Task<List<Resource>> GetResourcesAsync() {
		var resources = new List<Resource>();
		string? next = null;
		ResourceList data;
		
		try {
			do {
				// ? Prepare uri
				var path = "resources/resource";
				path += next == null ? string.Empty : $"?cursor={next}";
				
				// ? Fetch resources
				data = await _auth.SendAsync<ResourceList>(() =>
					new HttpRequestMessage(HttpMethod.Get, path));
				
				// ? Store and run until no cursor is found
				resources.AddRange(data.Items);
				next = data.NextCursor;
			}
			while (next != null && data.Items.Count > 0);
			return resources;
		}
		catch (Exception ex) {
			throw AppException.Label<AppException>(ex, Msg(ex.Message));
		}
	}
	public async Task<T> GetUnavailabilitiesAsync<T>(string resourceId) {
		// TODO: code...
		// try {
		// 	var data = await _auth.SendAsync<ResourceList>(() =>
		// 		new HttpRequestMessage(HttpMethod.Get, "resources/resource"));
		// 	return data;
		// }
		// catch (Exception ex) {
		// 	throw AppException.Label<AppException>(ex, Msg(ex.Message));
		// }
		return default;
	}
}
