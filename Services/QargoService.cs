using System;
using Root.Core;
using Root.Source;
using Root.Services.Interfaces;

namespace Root.Services;

public class QargoService : Tenant, IQargoService {
	public QargoService(Context ctx) : base(
		name:     "Qargo",
		rs:		 ctx.Req,
		secret:   ctx.Env.Load("QARGO_SECRET"),
		clientId: ctx.Env.Load("QARGO_CLIENT_ID")
	) { }

	public Task UpdateUnavailabilitiesAsync(string resourceId, MUActions actions) {
		// TODO: code...
		return default;
	}
	
	public Task CreateUnavailabilitiesAsync(string resourceId, MUActions actions) {
		// TODO: code...
		return default;
	}
}
