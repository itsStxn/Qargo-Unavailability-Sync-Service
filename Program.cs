using System;
using Serilog;
using Root.Source;
using Root.Errors;
using Root.Services;

namespace Root;

class Program {
	static public async Task Main(string[] args) {
		// ? Get context: env variables, http handle, etc...
		using var ctx = new Context();

		try {
			// ? Define services
			var qargo = new QargoService(ctx);
			var master = new MasterService(ctx);

			// ? Sync qargo to master
			var s2s = new Service2Service(qargo, master);
			await s2s.SyncUnavailabilities();
		}
		catch (NetworkException ex) {
			Log.Fatal(ex, "Network failure {StatusCode} after all retry attempts — {Message}",
				ex.StatusCode.HasValue ? $" ({(int)ex.StatusCode} {ex.StatusCode})" : "",
				ex.Message
			);
		}
		catch (ConfigException ex) {
			Log.Fatal(ex, "Invalid or missing configuration — {Message}", ex.Message);
		}
		catch (ParseException ex) {
			Log.Fatal(ex, "Failed to parse response — {Message}", ex.Message);
		}
		catch (StreamException ex) {
			Log.Fatal(ex, "Stream error — {Message}", ex.Message);
		}
		catch (Exception ex) {
			Log.Fatal(ex, "Unexpected uncaught error — {Message}", ex.Message);
		}
		finally {
			ctx.ShutDown();
		}
	}
}
