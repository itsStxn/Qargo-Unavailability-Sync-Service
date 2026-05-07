using System;

namespace Root.Core.Interfaces;

public interface IMyRequest {
	public Task<T> SendAsync<T>(Func<HttpRequestMessage> reqFactory);
}
