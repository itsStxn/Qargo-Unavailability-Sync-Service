using System;

namespace Root.Core.Interfaces;

public interface IHttpRequest {
	public Task<T> SendAsync<T>(Func<HttpRequestMessage> reqFactory);
}
