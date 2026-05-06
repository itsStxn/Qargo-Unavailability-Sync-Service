using System;
using static Root.Constants.Constants;

namespace Root.Source;

public class Context {
	public readonly EnvSource Env;
	public readonly RequestSource Req;
	private readonly CancellationTokenSource _ct;

	public Context() {
		// ? Load environment variables
		Env = new EnvSource();

		// ? Define cancellation token
		_ct = new CancellationTokenSource(TimeSpan.FromSeconds(CT_TIMEOUT));
		
		// ? Define request source
		Req = new RequestSource(
			ct: _ct.Token,
			maxAttempts: MAX_ATTEMPTS,
			client: new HttpClient() {
				BaseAddress = new Uri(BASEURL),
				Timeout = TimeSpan.FromSeconds(TIMEOUT)
			}
		);
	}
}
