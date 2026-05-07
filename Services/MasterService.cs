using System;
using Root.Core;
using Root.Source;
using Root.Errors;
using Root.Services.Interfaces;

namespace Root.Services;

/// <summary>
/// Represents the master tenant service responsible for building
/// a resource-to-unavailability action map used for synchronization operations.
/// Inherits all authenticated tenant API functionality from <see cref="Tenant"/>.
/// </summary>
public class MasterService : Tenant, IResourseMap {

	/// <summary>
	/// Maps resource IDs to their associated unavailability actions.
	/// Each entry contains the create/update operations required for synchronization.
	/// </summary>
	public readonly Dictionary<string, UActions> ResourceMap;


	/// <summary>
	/// Initializes a new instance of <see cref="MasterService"/>
	/// using credentials loaded from the provided <see cref="Context"/>.
	/// </summary>
	/// <param name="ctx">
	/// The shared application context containing environment variables
	/// and the reusable <see cref="HttpClient"/>.
	/// </param>
	public MasterService(Context ctx) : base(
		name:     "Master",
		cli:      ctx.Cli,
		secret:   ctx.Env.Load("MASTER_SECRET"),
		clientId: ctx.Env.Load("MASTER_CLIENT_ID")
	) {
		ResourceMap = [];
	}


	/// <summary>
	/// Retrieves all master tenant resources and maps their existing unavailabilities
	/// into <see cref="ResourceMap"/> as create actions.
	/// Existing unavailabilities are assumed to require creation on downstream tenants.
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

			// ? Assume unavailabilities need to be created, not updated
			var unavails = await GetUnavailabilitiesAsync(r.Id);

			if (unavails.Count > 0) {
				var actions = new UActions();

				foreach (var u in unavails) {
					u.ExternalId = u.Id; // ? Update external id
					actions.ToCreate.Add(u.Id, u);
				}

				// ? Map resource id to actions
				ResourceMap[r.Id] = actions;
			}
		}
	}
}
