using System;
using Serilog;
using static Root.Constants.Constants;

namespace Root.Source;

/// <summary>
/// Represents the shared runtime context for the console application.
/// Responsible for configuring logging, loading environment variables,
/// managing application cancellation, and exposing a reusable <see cref="HttpClient"/> instance.
/// </summary>
public class Context : IDisposable {
	
	/// <summary>
	/// Provides access to environment variables and application configuration values.
	/// </summary>
	public readonly EnvSource Env;

	/// <summary>
	/// Shared HTTP client configured for all outbound API requests.
	/// </summary>
	public readonly HttpClient Cli;

	/// <summary>
	/// Cancellation token source used for graceful shutdown and request cancellation.
	/// </summary>
	private readonly CancellationTokenSource _ct;

	/// <summary>
	/// Tracks whether this instance has already been disposed.
	/// </summary>
	private bool _disposed;


	/// <summary>
	/// Initializes a new instance of <see cref="Context"/>.
	/// Configures Serilog logging, loads environment variables,
	/// registers graceful shutdown handlers, and initializes the shared HTTP client.
	/// </summary>
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

		Console.CancelKeyPress += (_, e) => {
			e.Cancel = true; // ? no immediate kill
			_ct.Cancel();    // ? graceful shutdown
		};

		// ? Gather reusable request resource
		Cli = new HttpClient() {
			BaseAddress = new Uri(BASEURL),
			Timeout = TimeSpan.FromSeconds(TIMEOUT)
		};
	}


	/// <summary>
	/// Initiates graceful application shutdown and releases all managed resources.
	/// </summary>
	public void ShutDown() {
		Log.Information("Console App shutting down");
		Dispose();
	}


	/// <summary>
	/// Releases managed resources associated with this context,
	/// including the cancellation token source and shared HTTP client.
	/// Ensures disposal only occurs once.
	/// </summary>
	public void Dispose() {
		if (_disposed) return;

		// ? Cancel remaining requests
		if (!_ct.IsCancellationRequested)
			_ct.Cancel();

		// ? Release both token and HTTP client
		_ct.Dispose();
		Cli.Dispose();

		Log.Information("Console app resources released");
		Log.CloseAndFlush();

		_disposed = true;
	}
}
