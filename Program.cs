using System;
using Root.Core;
using Root.Source;
using Root.Services;

namespace Root;

class Program {
	static public async Task Main(string[] args) {
		// ? Get context: env variables, http handle, etc...
		var ctx = new Context();

		// ? Define tenants
		var qargo = new QargoService(ctx);
		var master = new MasterService(ctx);

		// DONE: Implement access token cache
		// TODO: Implement a logger with tenant identity feature
		// TODO: Implement tenant operations
		// TODO: Implement parallel flow logic
		// TODO: Implement inter-environment operations
	}
}
