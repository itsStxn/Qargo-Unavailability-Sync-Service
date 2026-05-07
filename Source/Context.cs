using System;
using Serilog;
using static Root.Constants.Constants;

namespace Root.Source;

public class Context : IDisposable {
	public readonly EnvSource Env;
	public readonly RequestSource Req;
	private readonly CancellationTokenSource _ct;
	private bool _disposed;

	public Context() {
		_disposed = false;

		// ? Set up logger
		Log.Logger = new LoggerConfiguration()
			.MinimumLevel.Debug()
			.WriteTo.Console()
			.WriteTo.File(
				path: "Logs/app-.log",
				rollingInterval: RollingInterval.Day,
				retainedFileCountLimit: LOGS_TTL
			)
			.Enrich.WithMachineName()
			.Enrich.WithThreadId()
			.CreateLogger();
		Log.Information("Console App initialized");

		// ? Load env variables
		Env = new EnvSource();

		// ? Define cancellation token
		_ct = new CancellationTokenSource(TimeSpan.FromSeconds(CT_TIMEOUT));

		// ? Gather reusable request resource
		Req = new RequestSource(
			ct: _ct.Token,
			maxAttempts: MAX_ATTEMPTS,
			client: new HttpClient() {
				BaseAddress = new Uri(BASEURL),
				Timeout = TimeSpan.FromSeconds(TIMEOUT)
			}
		);

	}

	public void ShutDown() {
		Log.Information("Console App shutting down");
		Dispose();
	}

	public void Dispose() {
		if (_disposed) return;

		// ? Cancel remaining requests
		if (!_ct.IsCancellationRequested)
				_ct.Cancel();

		// ? Release both token and HTTP client
		_ct.Dispose();
		Req.Dispose();

		Log.Information("Console app resources released");
		Log.CloseAndFlush();
		
		_disposed = true;
	}
}
