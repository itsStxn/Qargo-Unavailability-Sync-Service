using System;
using Root.Core;
using Root.Source;
using Root.Errors;
using Root.Services.Interfaces;

namespace Root.Services;

public class MasterService : Tenant, IMasterService {
	public MasterService(Context ctx) : base(
		name:     "Master",
		cli:		 ctx.Cli,
		secret:   ctx.Env.Load("MASTER_SECRET"),
		clientId: ctx.Env.Load("MASTER_CLIENT_ID")
	) { }

	public async Task<Dictionary<string, UActions>> MapResources() {
		var resourceMap = new Dictionary<string, UActions>();
		
		// ? Get every resource's unavailability
		var resources = await GetResourcesAsync();

		foreach (var r in resources) {
			// ? Validate uniqueness
			if (resourceMap.ContainsKey(r.Id))
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
				resourceMap[r.Id] = actions;
			}
		}

		return resourceMap;
	}
}
