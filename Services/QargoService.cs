using System;
using Root.Core;
using Root.Source;
using Root.Errors;
using Root.Services.Interfaces;
using Root.DTOs.UnavailabilityListComponents;

namespace Root.Services;

public class QargoService : Tenant, IResourseMap {
	public readonly Dictionary<string, List<Unavailability>> ResourceMap;

	public QargoService(Context ctx) : base(
		name:     "Qargo",
		cli:		 ctx.Cli,
		secret:   ctx.Env.Load("QARGO_SECRET"),
		clientId: ctx.Env.Load("QARGO_CLIENT_ID")
	) {
		ResourceMap = [];
	}

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
