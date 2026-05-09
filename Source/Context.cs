using System;
using Serilog;
using Root.Records;
using static Root.Constants;

namespace Root.Source;

/// <summary>
/// Represents the shared runtime context for the console application.
/// Responsible for configuring logging, loading environment variables,
/// managing application cancellation, and exposing a reusable <see cref="HttpSource"/> instance.
/// </summary>
public class Context : IDisposable {
	
	/// <summary>
	/// Provides access to environment variables and application configuration values.
	/// </summary>
	public readonly EnvSource Env;

	/// <summary>
	/// Provides HTTP-based access to remote APIs with timeout, retry, and cancellation support.
	/// </summary>
	public readonly HttpSource Http;

	/// <summary>
	/// Set of filter criteria used to query and filter unavailability records.
	/// </summary>
	public readonly UnavailabilityFilters UFilters;

	/// <summary>
	/// Cancellation token source used for graceful shutdown and request cancellation.
	/// </summary>
	private readonly CancellationTokenSource _cts;

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

		// ? Define cancellation token source
		_cts = new CancellationTokenSource(TimeSpan.FromMinutes(CT_TIMEOUT));

		Console.CancelKeyPress += (_, e) => {
			e.Cancel = true; 	// ? no immediate kill
			_cts.Cancel();    // ? graceful shutdown
		};

		// ? Define reusable request resource
		Http = new HttpSource(
			cantToken: _cts.Token,
			retry: new HttpRetry {
				Timeout = REQ_RETRY_TIMEOUT,
				MaxAttempts = REQ_MAX_ATTEMPTS
			},
			cli: new HttpClient() {
				BaseAddress = new Uri(BASEURL),
				Timeout = TimeSpan.FromSeconds(REQ_TIMEOUT)
			}
		);

		// ? Define unavailability filters
		UFilters.Year = UNAVAIL_YEAR;
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
		if (!_cts.IsCancellationRequested)
			_cts.Cancel();

		// ? Release both token and HTTP source
		_cts.Dispose();
		Http.Dispose();

		Log.Information("Console app resources released");
		Log.CloseAndFlush();

		_disposed = true;
	}
}
