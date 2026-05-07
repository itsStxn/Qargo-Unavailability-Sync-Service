using System;
using Root.DTOs;
using Root.Source;
using Root.Errors;
using Root.Core.Interfaces;

namespace Root.Core;

public class Tenant : Base, ITenant {
	private readonly MyAuthRequest _auth;

	public Tenant(string name, string clientId, string secret, RequestSource rs) {
		_auth = new MyAuthRequest(name, clientId, secret, rs);
		_name = name;
	}

	public async Task<ResourceList> GetResourcesAsync() {
		try {
			var data = await _auth.SendAsync<ResourceList>(() =>
				new HttpRequestMessage(HttpMethod.Get, "resources/resource"));
			return data;
		}
		catch (Exception ex) {
			throw AppException.Label<AppException>(ex, Msg(ex.Message));
		}
	}
	public Task<T> GetUnavailabilitiesAsync<T>(string resourceId) {
		// TODO: code...
		return default;
	}
}
