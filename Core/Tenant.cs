using System;
using Root.DTOs;
using Root.Errors;
using Root.Core.Interfaces;
using System.Net.Http.Json;
using static Root.Constants.Constants;
using Root.DTOs.ResourceListComponents;
using Root.DTOs.UnavailabilityListComponents;

namespace Root.Core;

/// <summary>
/// Represents a tenant context and provides operations for managing resources and their unavailabilities
/// via the authenticated API. Inherits logging utilities from <see cref="Base"/> and delegates
/// all HTTP communication to <see cref="MyAuthRequest"/>.
/// </summary>
public class Tenant : Base, ITenant {

	/// <summary>The authenticated HTTP client used for all API calls made by this tenant.</summary>
	private readonly MyAuthRequest _auth;


	/// <summary>
	/// Initializes a new instance of <see cref="Tenant"/> with the given credentials and HTTP client.
	/// </summary>
	/// <param name="name">The tenant's service name, used for log message prefixing.</param>
	/// <param name="clientId">The OAuth2 client ID for authentication.</param>
	/// <param name="secret">The OAuth2 client secret for authentication.</param>
	/// <param name="cli">The <see cref="HttpClient"/> instance to use for all requests.</param>
	public Tenant(string name, string clientId, string secret, HttpClient cli) {
		_auth = new MyAuthRequest(name, clientId, secret, cli);
		_name = name;
	}


	/// <summary>
	/// Retrieves all resources for this tenant by paginating through the API until no further cursor is returned.
	/// </summary>
	/// <returns>A <see cref="Task"/> resolving to a flat <see cref="List{T}"/> of all <see cref="Resource"/> items.</returns>
	/// <exception cref="AppException">Thrown if any error occurs during fetching, wrapping the original exception.</exception>
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

	/// <summary>
	/// Retrieves all unavailabilities for a given resource by paginating through the API,
	/// filtered to only include entries where both <c>StartTime</c> and <c>EndTime</c> fall within <c>UNAVAIL_YEAR</c>.
	/// </summary>
	/// <param name="resourceId">The ID of the resource to fetch unavailabilities for.</param>
	/// <returns>A <see cref="Task"/> resolving to a flat <see cref="List{T}"/> of matching <see cref="Unavailability"/> items.</returns>
	/// <exception cref="AppException">Thrown if any error occurs during fetching, wrapping the original exception.</exception>
	public async Task<List<Unavailability>> GetUnavailabilitiesAsync(string resourceId) {
		Echo($"Getting unavailabilities for resource: {resourceId}...");
		var unavails = new List<Unavailability>();
		string? next = null;
		UnavailabilityList data;

		try {
			do {
				// ? Prepare uri
				var endpoint = $"resources/resource/{resourceId}/unavailability";
				endpoint += next == null ? string.Empty : $"?cursor={next}";

				// ? Fetch resources
				data = await _auth.SendAsync<UnavailabilityList>(() =>
					new HttpRequestMessage(HttpMethod.Get, endpoint));

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

	/// <summary>
	/// Creates all unavailabilities in the provided <see cref="UActions.ToCreate"/> map for a given resource.
	/// Each entry is dispatched individually via <see cref="CreateUnavailabilityAsync"/>.
	/// </summary>
	/// <param name="resourceId">The ID of the resource to create unavailabilities for.</param>
	/// <param name="actions">
	/// The action map containing unavailabilities to create. Logs a warning and exits early if <see cref="UActions.ToCreate"/> is empty.
	/// </param>
	/// <returns>
	/// A <see cref="Task"/> resolving to the <see cref="HttpResponseMessage"/> from the last successful creation request.
	/// </returns>
	/// <exception cref="ParseException">Thrown if no requests were made and the response is <c>null</c>.</exception>
	/// <exception cref="AppException">Thrown if any error occurs during creation, wrapping the original exception.</exception>
	public async Task<HttpResponseMessage> CreateUnavailabilitiesAsync(string resourceId, UActions actions) {
		// ? Early exit
		if (actions.ToCreate.Count == 0)
			Warn("Creation map is empty, early exit...");

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

	/// <summary>
	/// Sends a single POST request to create one <see cref="Unavailability"/> entry for a given resource.
	/// </summary>
	/// <param name="resourceId">The ID of the resource to attach the unavailability to.</param>
	/// <param name="unavail">The <see cref="Unavailability"/> object to serialize and POST.</param>
	/// <returns>A <see cref="Task"/> resolving to the <see cref="HttpResponseMessage"/> from the API.</returns>
	private async Task<HttpResponseMessage> CreateUnavailabilityAsync(string resourceId, Unavailability unavail) {
		string endpoint = $"resources/resource/{resourceId}/unavailability";
		var res = await _auth.SendAsync<HttpResponseMessage>(() =>
			new(HttpMethod.Post, endpoint) {
				Content = JsonContent.Create(unavail)
		});

		return res;
	}

	/// <summary>
	/// Updates all unavailabilities in the provided <see cref="UActions.ToUpdate"/> map for a given resource.
	/// Each entry is dispatched individually via <see cref="UpdateUnavailabilityAsync"/>.
	/// </summary>
	/// <param name="resourceId">The ID of the resource to update unavailabilities for.</param>
	/// <param name="actions">
	/// The action map containing unavailabilities to update. Logs a warning and exits early if <see cref="UActions.ToUpdate"/> is empty.
	/// </param>
	/// <returns>
	/// A <see cref="Task"/> resolving to the <see cref="HttpResponseMessage"/> from the last successful update request.
	/// </returns>
	/// <exception cref="ParseException">Thrown if no requests were made and the response is <c>null</c>.</exception>
	/// <exception cref="AppException">Thrown if any error occurs during updating, wrapping the original exception.</exception>
	public async Task<HttpResponseMessage> UpdateUnavailabilitiesAsync(string resourceId, UActions actions) {
		// ? Early exit
		if (actions.ToUpdate.Count == 0)
			Warn("Update map is empty, early exit...");

		Echo($"Updating unavailabilities for resource: {resourceId}...");
		HttpResponseMessage? lastRes = null;

		try {
			foreach (var (unavailId, newUnavail) in actions.ToUpdate) {
				lastRes = await UpdateUnavailabilityAsync(resourceId, unavailId, newUnavail);
			}

			Echo($"Successfully updated unavailabilities for resource: {resourceId}");
			return lastRes ?? throw new ParseException(Msg("HTTP response is null"));
		}
		catch (Exception ex) {
			Error($"Failed to update unavailabilities for resource: {resourceId}");
			throw AppException.Label<AppException>(ex, Msg(ex.Message));
		}
	}

	/// <summary>
	/// Sends a single PUT request to replace one <see cref="Unavailability"/> entry for a given resource.
	/// </summary>
	/// <param name="resourceId">The ID of the resource the unavailability belongs to.</param>
	/// <param name="unavailId">The ID of the unavailability entry to replace.</param>
	/// <param name="newUnavail">The updated <see cref="Unavailability"/> object to serialize and PUT.</param>
	/// <returns>A <see cref="Task"/> resolving to the <see cref="HttpResponseMessage"/> from the API.</returns>
	private async Task<HttpResponseMessage> UpdateUnavailabilityAsync(string resourceId, string unavailId, Unavailability newUnavail) {
		string endpoint = $"resources/resource/{resourceId}/unavailability/{unavailId}";
		var res = await _auth.SendAsync<HttpResponseMessage>(() =>
			new(HttpMethod.Put, endpoint) {
				Content = JsonContent.Create(newUnavail)
		});

		return res;
	}
}
