using System;
using Root.DTOs;

namespace Root.Core.Interfaces;

public interface ITenant {
	public Task<Resource?> GetResourcesAsync();
	public Task<T?> GetUnavailabilitiesAsync<T>(string resourceId);
}
