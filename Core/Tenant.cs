using System;
using Root.DTOs;
using Root.Source;
using Root.Errors;
using Root.Core.Interfaces;
using System.Net.Http.Json;
using static Root.Constants.Constants;
using Root.DTOs.ResourceListComponents;
using Root.DTOs.UnavailabilityListComponents;

namespace Root.Core;

public class Tenant : Base, ITenant {
	private readonly MyAuthRequest _auth;

	public Tenant(string name, string clientId, string secret, RequestSource rs) {
		_auth = new MyAuthRequest(name, clientId, secret, rs);
		_name = name;
	}

	public async Task<List<Resource>> GetResourcesAsync() {
		Echo("Getting resources...");
		var resources = new List<Resource>();
		string? next = null;
		ResourceList data;
		
		try {
			do {
				// ? Prepare uri
				var endpoint = "resources/resource";
				endpoint += next == null ? string.Empty : $"?cursor={next}";
				
				// ? Fetch resources
				data = await _auth.SendAsync<ResourceList>(() =>
					new HttpRequestMessage(HttpMethod.Get, endpoint));
				
				// ? Store and run until no cursor is found
				resources.AddRange(data.Items);
				next = data.NextCursor;
			}
			while (next != null && data.Items.Count > 0);

			Echo("Successfully got resources");
			return resources;
		}
		catch (Exception ex) {
			Error("Failed to get resources");
			throw AppException.Label<AppException>(ex, Msg(ex.Message));
		}
	}

	public async Task<List<Unavailability>> GetUnavailabilitiesAsync(string resourceId) {
		Echo($"Getting unavailabilities for resource: {resourceId}...");
		var unavails = new List<Unavailability>();
		string? next = null;
		UnavailabilityList data;
		
		try {
			do {
				// ? Prepare uri
				var endoint = $"resources/resource/{resourceId}/unavailability";
				endoint += next == null ? string.Empty : $"?cursor={next}";
				
				// ? Fetch resources
				data = await _auth.SendAsync<UnavailabilityList>(() =>
					new HttpRequestMessage(HttpMethod.Get, endoint));
				
				// ? Filter by year and store
				unavails.AddRange(data.Items.Where(u => 
					u.StartTime.Contains(UNAVAIL_YEAR) && 
					(u.EndTime ?? string.Empty).Contains(UNAVAIL_YEAR)
				));

				// ? Run until no cursor is found
				next = data.NextCursor;
			}
			while (next != null && data.Items.Count > 0);

			Echo($"Successfully got unavailabilities for resource: {resourceId}");
			return unavails;
		}
		catch (Exception ex) {
			Error($"Failed to get unavailabilities for resource: {resourceId}");
			throw AppException.Label<AppException>(ex, Msg(ex.Message));
		}
	}

	public async Task<HttpResponseMessage> CreateUnavailabilitiesAsync(string resourceId, UActions actions) {
		Echo($"Creating unavailabilities for resource: {resourceId}...");
		HttpResponseMessage? lastRes = null;

		try {
			foreach (var unavail in actions.ToCreate.Values) {
				lastRes = await CreateUnavailabilityAsync(resourceId, unavail);
			}

			Echo($"Successfully created unavailabilities for resource: {resourceId}");
			return lastRes ?? throw new ParseException(Msg("HTTP response is null"));
		}
		catch (Exception ex) {
			Error($"Failed to create unavailabilities for resource: {resourceId}");
			throw AppException.Label<AppException>(ex, Msg(ex.Message));
		}
	}

	private async Task<HttpResponseMessage> CreateUnavailabilityAsync(string resourceId, Unavailability unavail) {
		string endpoint = $"resources/resource/{resourceId}/unavailability";
		var res = await _auth.SendAsync<HttpResponseMessage>(() => 
			new(HttpMethod.Post, endpoint) {
				Content = JsonContent.Create(unavail)
		});

		return res;
	}

	public async Task<HttpResponseMessage> UpdateUnavailabilitiesAsync(string resourceId, UActions actions) {
		Echo($"Updating unavailabilities for resource: {resourceId}...");
		HttpResponseMessage? lastRes = null;

		try {
			foreach (var unavail in actions.ToCreate.Values) {
				lastRes = await UpdateUnavailabilityAsync(resourceId, unavail);
			}

			Echo($"Successfully updated unavailabilities for resource: {resourceId}");
			return lastRes ?? throw new ParseException(Msg("HTTP response is null"));
		}
		catch (Exception ex) {
			Error($"Failed to update unavailabilities for resource: {resourceId}");
			throw AppException.Label<AppException>(ex, Msg(ex.Message));
		}
	}

	private async Task<HttpResponseMessage> UpdateUnavailabilityAsync(string resourceId, Unavailability unavail) {
		string endpoint = $"resources/resource/{resourceId}/unavailability/{unavail.Id}";
		var res = await _auth.SendAsync<HttpResponseMessage>(() => 
			new(HttpMethod.Put, endpoint) {
				Content = JsonContent.Create(unavail)
		});

		return res;
	}
}
