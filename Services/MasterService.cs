using System;
using Root.Core;
using Root.Source;
using Root.Services.Interfaces;

namespace Root.Services;

public class MasterService : Tenant, IResourceMapper {
	public readonly Dictionary<string, UActions> ResourceMap;

	public MasterService(Context ctx) : base(
		name:     "Master",
		rs:		 ctx.Req,
		secret:   ctx.Env.Load("MASTER_SECRET"),
		clientId: ctx.Env.Load("MASTER_CLIENT_ID")
	) {
		ResourceMap = [];
	}

	public Task MapResources() {
		// TODO: code...
		return default;
	}
}
