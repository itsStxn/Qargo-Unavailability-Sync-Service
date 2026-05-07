using System;
using Root.Core;
using Root.Source;
using Root.Services.Interfaces;
using Root.DTOs.UnavailabilityListComponents;

namespace Root.Services;

public class QargoService : Tenant, IResourceMapper {
	public readonly Dictionary<string, List<Unavailability>> ResourceMap;

	public QargoService(Context ctx) : base(
		name:     "Qargo",
		rs:		 ctx.Req,
		secret:   ctx.Env.Load("QARGO_SECRET"),
		clientId: ctx.Env.Load("QARGO_CLIENT_ID")
	) {
		ResourceMap = [];
	}

	public Task MapResources() {
		// TODO: code...
		return default;
	}
}
