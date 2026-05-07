using System;
using Root.Core;
using Root.Source;
using Root.Errors;
using Root.Services.Interfaces;
using Root.DTOs.UnavailabilityListComponents;

namespace Root.Services;

/// <summary>
/// Represents the Qargo tenant service responsible for retrieving and mapping
/// resource unavailabilities that are already linked through external identifiers.
/// Inherits authenticated tenant API functionality from <see cref="Tenant"/>.
/// </summary>
public class QargoService : Tenant, IResourseMap {

	/// <summary>
	/// Maps resource IDs to their associated unavailabilities
	/// that contain valid external identifiers.
	/// </summary>
	public readonly Dictionary<string, List<Unavailability>> ResourceMap;


	/// <summary>
	/// Initializes a new instance of <see cref="QargoService"/>
	/// using credentials loaded from the provided <see cref="Context"/>.
	/// </summary>
	/// <param name="ctx">
	/// The shared application context containing environment variables
	/// and the reusable <see cref="HttpClient"/>.
	/// </param>
	public QargoService(Context ctx) : base(
		name:     "Qargo",
		cli:      ctx.Cli,
		secret:   ctx.Env.Load("QARGO_SECRET"),
		clientId: ctx.Env.Load("QARGO_CLIENT_ID")
	) {
		ResourceMap = [];
	}


	/// <summary>
	/// Retrieves all Qargo tenant resources and maps their unavailabilities
	/// that contain valid external identifiers into <see cref="ResourceMap"/>.
	/// </summary>
	/// <returns>A <see cref="Task"/> representing the asynchronous mapping operation.</returns>
	/// <exception cref="ConfigException">
	/// Thrown when duplicate resource IDs are encountered during mapping.
	/// </exception>
	public async Task MapResources() {

		// ? Get every resource's unavailability
		var resources = await GetResourcesAsync();

		foreach (var r in resources) {

			// ? Validate uniqueness
			if (ResourceMap.ContainsKey(r.Id))
				throw new ConfigException(Msg("resource IDs must be unique"));

			// ? Filter unavailabilities that have external id
			var filtered = (await GetUnavailabilitiesAsync(r.Id))
				.FindAll(u => !string.IsNullOrWhiteSpace(u.ExternalId));

			if (filtered.Count > 0)
				ResourceMap[r.Id] = filtered;
		}
	}
}
