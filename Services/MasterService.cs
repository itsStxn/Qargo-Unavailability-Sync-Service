using System;
using Root.Core;
using Root.Source;

namespace Root.Services;

public class MasterService : Tenant {
	public MasterService(Context ctx) : base(
		name:     "Master",
		rs:		 ctx.Req,
		secret:   ctx.Env.Load("MASTER_SECRET"),
		clientId: ctx.Env.Load("MASTER_CLIENT_ID")
	) { }
}
